using Kingmaker;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UI.Tooltip;
using ModMaker;
using System.Reflection;
using static UIExtensions.Main;

namespace UIExtensions.Features
{
    public class ChangePositionOfInspector :
        IModEventHandler,
        ISceneHandler
    {
        public int Priority => 800;


        public static bool LeftToggle {
            get => Mod.Settings.toggleChangePositionOfInspectorLeft;
            set {
                if (Mod.Settings.toggleChangePositionOfInspectorLeft != value)
                {
                    if (value)
                    {
                        TurnBasedToggle = false;
                    }
                    Mod.Settings.toggleChangePositionOfInspectorLeft = value;
                    UpdateTooltip(value);
                }
            }
        }

        public static bool TurnBasedToggle {
            get => Mod.Settings.toggleChangePositionOfInspectorTurnBased;
            set {
                if (Mod.Settings.toggleChangePositionOfInspectorTurnBased != value)
                {
                    if (value)
                    {
                        LeftToggle = false;
                    }
                    Mod.Settings.toggleChangePositionOfInspectorTurnBased = value;
                    UpdateTooltip(value);
                }
            }
        }

        public static bool IsEnabled()
        {
            return Mod.Enabled && (LeftToggle || TurnBasedToggle);
        }

        static void UpdateTooltip(bool isEnabled)
        {
            UICommon uiCommon = Game.Instance.UI.Common;
            TooltipTrigger tooltip = uiCommon?.transform.Find("Inspect")?.gameObject.GetComponent<TooltipTrigger>();

            if (!tooltip)
                return;

            tooltip.PositionTransform = isEnabled ?
                LeftToggle ? uiCommon.transform.Find("HUDLayout/ActionBarAdditional/Background/Toggle/Background") :
                TurnBasedToggle ? uiCommon.transform.Find("HUDLayout/TurnBasedCombatTracker/Body") :
                null : null;

        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (LeftToggle || TurnBasedToggle)
                UpdateTooltip(true);

            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            EventBus.Unsubscribe(this);

            if (LeftToggle || TurnBasedToggle)
                UpdateTooltip(false);
        }

        public void OnAreaBeginUnloading() { }

        public void OnAreaDidLoad()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (LeftToggle || TurnBasedToggle)
                UpdateTooltip(true);
        }
    }
}
