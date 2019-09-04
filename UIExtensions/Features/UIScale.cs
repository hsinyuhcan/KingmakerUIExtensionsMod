using Harmony12;
using Kingmaker.UI;
using static UIExtensions.Main;

namespace UIExtensions.Features
{
    public class UIScale
    {
        public static float Modifier {
            get => Mod.Settings.uiScaleModifier;
            set {
                if (Mod.Settings.uiScaleModifier != value)
                {
                    Mod.Settings.uiScaleModifier = value;
                }
            }
        }

        // apply the modifier to Canvas.scaleFactor
        [HarmonyPatch(typeof(CanvasScalerWorkaround), "SetScaleFactor", typeof(float))]
        static class CanvasScalerWorkaround_SetScaleFactor_Patch
        {
            [HarmonyPrefix]
            static void Prefix(CanvasScalerWorkaround __instance, ref float scaleFactor)
            {
                if (Mod.Enabled)
                {
                    scaleFactor *= Modifier;
                }
            }
        }
    }
}