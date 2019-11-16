using DG.Tweening;
using Harmony12;
using Kingmaker.UI.Group;
using ModMaker;
using ModMaker.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UIExtensions.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using static ModMaker.Utility.ReflectionCache;
using static UIExtensions.Main;

namespace UIExtensions.Features
{
    public class AlwaysDisplayHpText : IModEventHandler
    {
        public int Priority => 600;

        public static bool Toggle {
            get => Mod.Settings.toggleAlwaysDisplayHpText;
            set {
                if (Mod.Settings.toggleAlwaysDisplayHpText != value)
                {
                    Mod.Settings.toggleAlwaysDisplayHpText = value;
                    UpdateGroup(value);
                }
            }
        }

        public static bool IsEnabled()
        {
            return Mod.Enabled && Toggle;
        }

        public static void Update()
        {
            UpdateGroup(Toggle);
        }

        static void UpdateGroup(bool isEnabled)
        {
            if (GroupController.Instance == null)
                return;

            Mod.Debug(MethodBase.GetCurrentMethod(), isEnabled);

            foreach (GroupCharacter character in GroupController.Instance.GetCharacters())
            {
                GroupCharacterPortraitController portrait;
                if (character.Unit != null && !(portrait = character.Portrait).IsMouseOver)
                {
                    if (isEnabled)
                        portrait.SetHpText();
                    else
                        portrait.HitPointsContainer.DOFade(0f, 0.1f).SetUpdate(true);
                }
            }
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (Toggle)
                UpdateGroup(true);
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (Toggle)
                UpdateGroup(false);
        }

        [HarmonyPatch(typeof(GroupCharacterPortraitController), nameof(GroupCharacterPortraitController.SetHpText))]
        static class GroupCharacterPortraitController_SetHpText_Patch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> codes, ILGenerator il)
            {
                // ---------------- before ----------------
                // IsMouseOver
                // ---------------- after  ----------------
                // IsEnabled() || IsMouseOver
                CodeInstruction[] findingCodes = new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, 
                        GetPropertyInfo<GroupCharacterPortraitController, bool>(nameof(GroupCharacterPortraitController.IsMouseOver)).GetGetMethod(true)),
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

        [HarmonyPatch(typeof(GroupCharacterPortraitController), nameof(GroupCharacterPortraitController.OnPointerExit), typeof(PointerEventData))]
        static class GroupCharacterPortraitController_OnPointerExit_Patch
        {
            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> codes, ILGenerator il)
            {
                // ----------------before----------------
                // HitPointsContainer.DOFade(0f, 0.1f).SetUpdate(true);
                // ----------------after----------------
                // if (!IsEnabled())
                //     HitPointsContainer.DOFade(0f, 0.1f).SetUpdate(true);
                CodeInstruction[] findingCodes = new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld,
                        GetFieldInfo<GroupCharacterPortraitController, CanvasGroup>(nameof(GroupCharacterPortraitController.HitPointsContainer))),
                    new CodeInstruction(OpCodes.Ldc_R4, 0f),
                    new CodeInstruction(OpCodes.Ldc_R4, 0.1f),
                    new CodeInstruction(OpCodes.Call),
                    new CodeInstruction(OpCodes.Ldc_I4_1),
                    new CodeInstruction(OpCodes.Call),
                    new CodeInstruction(OpCodes.Pop)
                };
                int startIndex = codes.FindCodes(findingCodes);
                if (startIndex >= 0)
                {
                    CodeInstruction[] patchingCodes = new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Call, 
                            new Func<bool>(IsEnabled).Method),
                        new CodeInstruction(OpCodes.Brtrue, codes.NewLabel(startIndex + findingCodes.Length, il))
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
    }
}