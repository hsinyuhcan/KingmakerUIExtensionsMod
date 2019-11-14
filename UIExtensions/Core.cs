using Kingmaker;
using Kingmaker.GameModes;
using Kingmaker.PubSubSystem;
using Kingmaker.UI;
using Kingmaker.UI.ActionBar;
using Kingmaker.UI.Common;
using Kingmaker.UI.Group;
using ModMaker;
using ModMaker.Utility;
using System;
using System.Reflection;
using UIExtensions.Controllers;
using UIExtensions.Features;
using static UIExtensions.Main;
using static UIExtensions.Utility.SettingsWrapper;

namespace UIExtensions
{
    public class Core :
        IModEventHandler,
        ISceneHandler
    {
        public int Priority => 200;

        public HotkeyController Hotkeys { get; internal set; }

        public OverrideInformation Information { get; internal set; }

        public static void FailedToPatch(MethodBase patch)
        {
            Type type = patch.DeclaringType;
            Mod.Warning($"Failed to patch '{type.DeclaringType?.Name}.{type.Name}.{patch.Name}'");
        }

        public void ResetSettings()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            Mod.ResetSettings();
            Mod.Settings.lastModVersion = Mod.Version.ToString();
            LocalizationFileName = Local.FileName;
            Hotkeys?.Update(true, true);
            AlwaysDisplayPetsPortrait.Update();
            AlwaysDisplayHpText.Update();
            DisplayPetsHpBarOnMastersPortrait.Update();
            DisplayPetsHpTextOnMastersPortrait.Update();
            OverrideInformation.Update();
        }

        private void HandleToggleHUD()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            UISectionHUDController hudController = Game.Instance.UI.Canvas?.HUDController;
            if (hudController)
            {
                switch (Game.Instance.CurrentMode)
                {
                    case GameModeType.EscMode:
                    case GameModeType.FullScreenUi:
                    case GameModeType.Dialog:
                    case GameModeType.Cutscene:
                        break;
                    default:
                        PauseToggle pauseToggle =
                            Game.Instance.UI.Common?.transform.Find("Pause")?.gameObject.GetComponent<PauseToggle>();
                        if (hudController.CurrentState == UISectionHUDController.HUDState.Hidden)
                        {
                            Game.Instance.UI.Canvas.SetHUDVisible();
                            pauseToggle.PlayPause(Game.Instance.IsPaused);
                        }
                        else
                        {
                            hudController.SetState(UISectionHUDController.HUDState.Hidden);
                            GroupController.Instance.HideAnimation(true);
                            pauseToggle.PlayPause(false);
                        }
                        break;
                }
            }
        }

        private void HandleToggleStealth()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            StealthSwitchButton button = new StealthSwitchButton();
            button.OnClick();
            button.SafeDestroy();
        }

        private void HandleToggleAI()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            AiSwitchButton button = new AiSwitchButton();
            button.OnClick();
            button.SafeDestroy();
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (!string.IsNullOrEmpty(LocalizationFileName))
            {
                Local.Import(LocalizationFileName, e => Mod.Error(e));
                LocalizationFileName = Local.FileName;
            }

            if (!Version.TryParse(Mod.Settings.lastModVersion, out Version version) || version < new Version(1, 2, 0))
                ResetSettings();
            else
                Mod.Settings.lastModVersion = Mod.Version.ToString();

            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_HUD, HandleToggleHUD);
            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_STEALTH, HandleToggleStealth);
            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_AI, HandleToggleAI);
            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            EventBus.Unsubscribe(this);
            HotkeyHelper.Unbind(HOTKEY_FOR_TOGGLE_HUD, HandleToggleHUD);
            HotkeyHelper.Unbind(HOTKEY_FOR_TOGGLE_STEALTH, HandleToggleStealth);
            HotkeyHelper.Unbind(HOTKEY_FOR_TOGGLE_AI, HandleToggleAI);
        }

        public void OnAreaBeginUnloading() { }

        public void OnAreaDidLoad()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_HUD, HandleToggleHUD);
            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_STEALTH, HandleToggleStealth);
            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_AI, HandleToggleAI);
        }
    }
}