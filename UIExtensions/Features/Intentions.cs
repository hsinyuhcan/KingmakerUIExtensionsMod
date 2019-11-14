using Harmony12;
using Kingmaker;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.Selection;
using ModMaker;
using ModMaker.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using static ModMaker.Utility.ReflectionCache;
using static UIExtensions.Main;

namespace UIExtensions.Features
{
    public class Intentions : IModEventHandler
    {
        public int Priority => 800;

        public static bool CanShowWhenNotHovering {
            get => Mod.Settings.toggleCanShowIntentionsWhenNotHovering;
            set {
                if (Mod.Settings.toggleCanShowIntentionsWhenNotHovering != value)
                {
                    Mod.Settings.toggleCanShowIntentionsWhenNotHovering = value;
                    Update();
                }
            }
        }

        public static bool CanShowWhenNotPausing {
            get => Mod.Settings.toggleCanShowIntentionsWhenNotPausing;
            set {
                if (Mod.Settings.toggleCanShowIntentionsWhenNotPausing != value)
                {
                    Mod.Settings.toggleCanShowIntentionsWhenNotPausing = value;
                    Update();
                }
            }
        }

        public static void Update()
        {
            EventBus.RaiseEvent<IInteractionHighlightUIHandler>
                (h => h.HandleHighlightChange(Game.Instance.InteractionHighlightController.IsHighlighting));
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (CanShowWhenNotPausing || CanShowWhenNotHovering)
                Update();
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (CanShowWhenNotPausing || CanShowWhenNotHovering)
                Update();
        }

        // check all units if target unit is not highlighted
        [HarmonyPatch(typeof(UIDecal), nameof(UIDecal.HoverUnit), typeof(UnitEntityData))]
        static class UIDecal_HoverUnit_Patch
        {
            [HarmonyPrefix]
            static void Prefix(UIDecal __instance, ref UnitEntityData unit)
            {
                if (Mod.Enabled && CanShowWhenNotHovering && (unit == null || !unit.View.MouseHighlighted))
                {
                    unit = __instance.Unit;
                }
            }
        }

        // fix "on selection"
        [HarmonyPatch(typeof(UIDecal), "CheckAttackIntention", typeof(bool))]
        static class UIDecal_CheckAttackIntention_Patch
        {
            [HarmonyPrefix]
            static void Prefix(UIDecal __instance, ref bool additionalLine)
            {
                if (Mod.Enabled && CanShowWhenNotHovering)
                {
                    additionalLine = __instance.IsSingleSelected;
                }
            }
        }
        
        // fix an error which happens on exit the game (or load a save) in combat
        [HarmonyPatch(typeof(UIDecal), nameof(UIDecal.IsEnemy), MethodType.Getter)]
        static class UIDecal_IsEnemy_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(UIDecal __instance, ref bool __result)
            {
                if (Mod.Enabled && CanShowWhenNotHovering)
                {
                    __result = __instance.Unit.IsPlayersEnemy;
                    return false;
                }
                return true;
            }
        }

        // it's useless
        [HarmonyPatch(typeof(UIDecal), "ShowAdditionalLine", typeof(UnitEntityData))]
        static class UIDecal_ShowAdditionalLine_Patch
        {
            [HarmonyPrefix]
            static bool Prefix()
            {
                if (Mod.Enabled && CanShowWhenNotHovering)
                {
                    return false;
                }
                return true;
            }
        }

        // fix pause
        [HarmonyPatch]
        static class UIDecal_CheckIntentions_Patch
        {
            [HarmonyTargetMethods]
            static IEnumerable<MethodBase> TargetMethods(HarmonyInstance instance)
            {
                yield return GetMethodInfo<UIDecal, Func<UIDecal, bool, bool>>("CheckAttackIntention");
                yield return GetMethodInfo<UIDecal, Func<UIDecal, bool>>("CheckCastIntention");
            }

            [HarmonyTranspiler]
            static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> codes, ILGenerator il)
            {
                // ---------------- before 1 ----------------
                // if (IsPaused)
                // ---------------- after  1 ----------------
                // if (IsPaused || (Mod.Enabled && CanShowWithoutPause))
                // ---------------- before 2 ----------------
                // return ForceHotkeyPressed || ObjectIsHovered || IsPaused || IsSingleSelected;
                // ---------------- after  2 ----------------
                // return IsPaused;
                CodeInstruction[] findingCodes_1 = new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call,
                        GetPropertyInfo<UIDecal, bool>(nameof(UIDecal.IsPaused)).GetGetMethod(true))
                };
                CodeInstruction[] findingCodes_2 = new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld,
                        GetFieldInfo<UIDecal, bool>(nameof(UIDecal.ForceHotkeyPressed))),
                    new CodeInstruction(OpCodes.Brtrue),
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Ldfld,
                        GetFieldInfo<UIDecal, bool>(nameof(UIDecal.ObjectIsHovered))),
                    new CodeInstruction(OpCodes.Brtrue)
                };
                int startIndex_1 = codes.FindCodes(findingCodes_1);
                int startIndex_2 = codes.FindCodes(startIndex_1 + findingCodes_1.Length, findingCodes_2);
                if (startIndex_1 >= 0 && startIndex_2 >= 0)
                {
                    CodeInstruction[] patchingCodes_2 = new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Call,
                            GetPropertyInfo<UIDecal, bool>(nameof(UIDecal.IsPaused)).GetGetMethod(true)),
                        new CodeInstruction(OpCodes.Ret),
                    };
                    return codes
                        .ReplaceRange(startIndex_2, findingCodes_2.Length, patchingCodes_2,true)
                        .Insert(startIndex_1 + findingCodes_1.Length, new CodeInstruction(OpCodes.Call,
                            new Func<bool, bool>(IsPausedPostGetter).Method), false)
                        .Complete();
                }
                else
                {
                    Core.FailedToPatch(MethodBase.GetCurrentMethod());
                    return codes;
                }
            }

            static bool IsPausedPostGetter(bool isPaused)
            {
                return isPaused || (Mod.Enabled && CanShowWhenNotPausing);
            }
        }
    }
}
