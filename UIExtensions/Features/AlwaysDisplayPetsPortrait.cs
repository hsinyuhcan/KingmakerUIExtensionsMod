using DG.Tweening;
using Harmony12;
using Kingmaker;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.Common;
using Kingmaker.UI.Group;
using ModMaker;
using ModMaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UIExtensions.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using static ModMaker.Utility.ReflectionCache;
using static UIExtensions.Main;

namespace UIExtensions.Features
{
    public class AlwaysDisplayPetsPortrait : IModEventHandler
    {
        public int Priority => 400;

        public static bool Toggle {
            get => Mod.Settings.toggleAlwaysDisplayPetsPortrait;
            set {
                if (Mod.Settings.toggleAlwaysDisplayPetsPortrait != value)
                {
                    Mod.Settings.toggleAlwaysDisplayPetsPortrait = value;
                    UpdateGroup();
                }
            }
        }

        public static bool IsEnabled()
        {
            return Mod.Enabled && Toggle;
        }

        public static void Update()
        {
            UpdateGroup();
        }

        static void UpdateGroup()
        {
            if (GroupController.Instance == null)
                return;

            Mod.Debug(MethodBase.GetCurrentMethod());

            UpdateNaviBlock();
            GroupController.Instance.SetGroup();
        }

        static void UpdateNaviBlock()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            bool withRemote = GroupController.Instance.GetWithRemote();
            bool withPet = GroupController.Instance.GetWithPet();
            if ((withRemote || withPet) && UIUtility.GetGroup(withRemote, withPet).Count > 6)
            {
                GroupController.Instance.GetNaviBlock().SetActive(true);
                GroupController.Instance.SetArrowsInteracteble();
            }
            else
            {
                GroupController.Instance.GetNaviBlock().SetActive(false);
            }
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (Toggle)
                UpdateGroup();
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (Toggle)
                UpdateGroup();
        }

        // force the pets th be displayed
        [HarmonyPatch(typeof(GroupController), "WithPet", MethodType.Getter)]
        static class GroupController_get_WithPet_Patch
        {
            [HarmonyPostfix]
            static void Postfix(ref bool __result)
            {
                if (!__result && IsEnabled())
                {
                    __result = true;
                }
            }
        }

        // fix the navigation block isn't refreshed after loading a save
        [HarmonyPatch(typeof(GroupController), "Kingmaker.UI.IUIElement.Initialize")]
        static class GroupController_Initialize_Patch
        {
            [HarmonyPostfix]
            static void Postfix(GroupController __instance)
            {
                if (IsEnabled())
                {
                    Mod.Debug(MethodBase.GetCurrentMethod());

                    UpdateNaviBlock();
                }
            }
        }

        // fix it's unable to scroll the portraits with the mouse wheel in combat
        [HarmonyPatch(typeof(GroupController), nameof(GroupController.OnScroll), typeof(PointerEventData))]
        static class GroupController_OnScroll_Patch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> codes, ILGenerator il)
            {
                // ---------------- before ----------------
                // FullScreenEnabled
                // ---------------- after  ----------------
                // IsEnabled() || FullScreenEnabled
                CodeInstruction[] findingCodes = new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        GetPropertyInfo<GroupController, bool>(nameof(GroupController.FullScreenEnabled)).GetGetMethod(true)),
                    new CodeInstruction(OpCodes.Brtrue)
                };
                int startIndex = codes.FindCodes(findingCodes);
                if (startIndex >= 0)
                {
                    CodeInstruction[] patchingCodes = new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Call,
                            new Func<bool>(IsEnabled).Method),
                        new CodeInstruction(OpCodes.Brtrue, codes.Item(startIndex + 2).operand)
                    };
                    return codes.InsertRange(startIndex, patchingCodes, true).Complete();
                }
                else
                {
                    Core.FailedToPatch(MethodBase.GetCurrentMethod());
                    return codes;
                }
            }
        }

        // update after gain a pet (after leveling up)
        [HarmonyPatch(typeof(SceneEntitiesState), nameof(SceneEntitiesState.AddEntityData), typeof(EntityDataBase))]
        static class SceneEntitiesState_AddEntityData_Patch
        {
            [HarmonyPostfix]
            static void Postfix(EntityDataBase data)
            {
                if (IsEnabled())
                {
                    if (data is UnitEntityData unit && unit.IsPlayerFaction && unit.Descriptor.IsPet)
                    {
                        Mod.Debug(MethodBase.GetCurrentMethod(), data);

                        UpdateGroup();
                    }
                }
            }
        }

        // fix the game could duplicately bind a hotkey to character slots multiple times
        [HarmonyPatch(typeof(GroupCharacter), nameof(GroupCharacter.Initialize), typeof(UnitEntityData), typeof(int))]
        static class GroupCharacter_Initialize_Patch
        {
            [HarmonyPrefix]
            static void Prefix(GroupCharacter __instance)
            {
                __instance.Bind(false);
            }
        }

        // fixed bugs on draging portraits
        [HarmonyPatch(typeof(GroupController), nameof(GroupController.DragCharacter), typeof(GroupCharacter))]
        static class GroupController_DragCharacter_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(GroupController __instance, GroupCharacter groupCharacter)
            {
                if (IsEnabled())
                {
                    List<GroupCharacter> characers = __instance.GetCharacters();
                    UnitEntityData targetUnit = GetTargetToSwap(groupCharacter, characers);
                    if (targetUnit != null)
                    {
                        SwapUnitsIndex(groupCharacter.Unit, targetUnit);
                        SortCharacters(groupCharacter, characers);
                        EventBus.RaiseEvent<IPartyChangedUIHandler>(h => h.HandlePartyChanged());
                    }
                    return false;
                }
                return true;
            }

            static UnitEntityData GetTargetToSwap(GroupCharacter source, List<GroupCharacter> characters)
            {
                float halfWidth = source.TargetPanel.sizeDelta.x / 2f;
                Vector3 sourcePosition = source.TargetPanel.localPosition;
                bool sourceIsPet = source.Unit.Descriptor.IsPet;
                bool IsValidTarget(GroupCharacter targetCharacter)
                {
                    UnitEntityData target = targetCharacter.Unit;
                    return target != null && (sourceIsPet ? target.Descriptor.Pet == null : !target.Descriptor.IsPet);
                }

                // check the next unit
                if (source.Index < 5)
                {
                    GroupCharacter targetCharacter =characters.Skip(source.Index + 1).FirstOrDefault(IsValidTarget);
                    if (sourcePosition.x > targetCharacter.BasePosition.x - halfWidth)
                    {
                        return targetCharacter.Unit;
                    }
                }

                // check the prior unit
                if (source.Index > 0)
                {
                    GroupCharacter targetCharacter = characters.Take(source.Index).LastOrDefault(IsValidTarget);
                    if (sourcePosition.x < targetCharacter.BasePosition.x + halfWidth)
                    {
                        return targetCharacter.Unit;
                    }
                }

                return null;
            }

            static void SwapUnitsIndex(UnitEntityData source, UnitEntityData target)
            {
                void EnsureNotPet(UnitEntityData unit)
                    => unit = unit.Descriptor.IsPet ? unit.Descriptor.Master.Value : unit;
                EnsureNotPet(source);
                EnsureNotPet(target);

                // get index
                List<UnitReference> characters = Game.Instance.Player.PartyCharacters;
                int sourceIndex = characters.FindIndex((UnitReference pc) => pc.Value == source);
                int targetIndex = characters.FindIndex((UnitReference pc) => pc.Value == target);

                // swap
                UnitReference sourceUnit = characters[sourceIndex];
                characters[sourceIndex] = characters[targetIndex];
                characters[targetIndex] = sourceUnit;
                Game.Instance.Player.InvalidateCharacterLists();
            }

            static void SortCharacters(GroupCharacter source, List<GroupCharacter> characters)
            {
                List<UnitEntityData> sortedUnits = 
                    UIUtility.GetGroup(GroupController.Instance.GetWithRemote(), GroupController.Instance.GetWithPet())
                    .Skip(GroupController.Instance.GetStartIndex()).Take(6).ToList();
                List<Vector3> positions = characters.Select(item => item.BasePosition).ToList();

                for (int i = 0; i < 6 && i < sortedUnits.Count; i++)
                {
                    UnitEntityData unit = sortedUnits[i];
                    GroupCharacter character = characters.Find(c => c.Unit == unit);
                    if (character == null)
                    {
                        character = characters.Find(c => !sortedUnits.Contains(c.Unit));
                        GroupController.Instance.SetCharacter(unit, character.Index);
                    }
                    if (character.Index != i)
                    {
                        DoMoveCharacter(character, i, positions[i], character != source);
                    }
                }

                characters.Sort((x, y) => x.Index - y.Index);
            }

            static void DoMoveCharacter(GroupCharacter character, int newIndex, Vector3 newPosition, bool doLocalMove)
            {
                character.Bind(false);
                character.Index = newIndex;
                character.Bind(true);
                character.BasePosition = newPosition;
                if (doLocalMove)
                {
                    character.transform.DOLocalMove(newPosition, 0.2f, false).SetUpdate(true);
                }
            }
        }
    }
}