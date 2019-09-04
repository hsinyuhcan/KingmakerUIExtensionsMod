using static UIExtensions.Main;

namespace UIExtensions.Utility
{
    public static class SettingsWrapper
    {
        #region Constants

        // hotkeys
        public const string HOTKEY_PREFIX = "Hotkey_";
        public const string HOTKEY_FOR_TOGGLE_HUD = HOTKEY_PREFIX + "Toggle_HUD";
        public const string HOTKEY_FOR_TOGGLE_PLAYER_INFO = HOTKEY_PREFIX + "Toggle_Player_Info";
        public const string HOTKEY_FOR_TOGGLE_ENEMY_INFO = HOTKEY_PREFIX + "Toggle_Enemy_Info";
        public const string HOTKEY_FOR_TOGGLE_STEALTH = HOTKEY_PREFIX + "Toggle_Stealth";
        public const string HOTKEY_FOR_TOGGLE_AI = HOTKEY_PREFIX + "Toggle_AI";

        #endregion

        #region Localization

        public static string LocalizationFileName {
            get => Mod.Settings.localizationFileName;
            set => Mod.Settings.localizationFileName = value;
        }

        #endregion
    }
}