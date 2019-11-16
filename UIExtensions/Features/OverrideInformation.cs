using Harmony12;
using Kingmaker;
using Kingmaker.Controllers.MapObjects;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.Overtip;
using Kingmaker.UI.SettingsUI;
using ModMaker;
using ModMaker.Utility;
using System.Reflection;
using static Kingmaker.UI.SettingsUI.SettingsEntityDropdownState;
using static UIExtensions.Main;
using static UIExtensions.Utility.SettingsWrapper;

namespace UIExtensions.Features
{
    public class OverrideInformation :
        IModEventHandler,
        ISceneHandler
    {
        public int Priority => 800;

        public static bool PlayerToggle {
            get => Mod.Settings.toggleOverrideInfoForPlayer;
            set {
                if (Mod.Settings.toggleOverrideInfoForPlayer != value)
                {
                    Mod.Settings.toggleOverrideInfoForPlayer = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowNamesForParty {
            get => Mod.Settings.overrideShowNamesForParty;
            set {
                if (Mod.Settings.overrideShowNamesForParty != value)
                {
                    Mod.Settings.overrideShowNamesForParty = value;
                    Update();
                }
            } 
        }

        public static DropdownState ShowPartyHP {
            get => Mod.Settings.overrideShowPartyHP;
            set {
                if (Mod.Settings.overrideShowPartyHP != value)
                {
                    Mod.Settings.overrideShowPartyHP = value;
                    Update();
                }
            }
        }

        public static bool? PartyHPIsShort {
            get => Mod.Settings.overridePartyHPIsShort;
            set {
                if (Mod.Settings.overridePartyHPIsShort != value)
                {
                    Mod.Settings.overridePartyHPIsShort = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowPartyActions {
            get => Mod.Settings.overrideShowPartyActions;
            set {
                if (Mod.Settings.overrideShowPartyActions != value)
                {
                    Mod.Settings.overrideShowPartyActions = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowPartyAttackIntentions {
            get => Mod.Settings.overrideShowPartyAttackIntentions;
            set {
                if (Mod.Settings.overrideShowPartyAttackIntentions != value)
                {
                    Mod.Settings.overrideShowPartyAttackIntentions = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowPartyCastIntentions {
            get => Mod.Settings.overrideShowPartyCastIntentions;
            set {
                if (Mod.Settings.overrideShowPartyCastIntentions != value)
                {
                    Mod.Settings.overrideShowPartyCastIntentions = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowNumericCooldownParty {
            get => Mod.Settings.overrideShowNumericCooldownParty;
            set {
                if (Mod.Settings.overrideShowNumericCooldownParty != value)
                {
                    Mod.Settings.overrideShowNumericCooldownParty = value;
                    Update();
                }
            }
        }

        public static bool EnemyToggle {
            get => Mod.Settings.toggleOverrideInfoForEnemy;
            set {
                if (Mod.Settings.toggleOverrideInfoForEnemy != value)
                {
                    Mod.Settings.toggleOverrideInfoForEnemy = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowNamesForEnemies {
            get => Mod.Settings.overrideShowNamesForEnemies;
            set {
                if (Mod.Settings.overrideShowNamesForEnemies != value)
                {
                    Mod.Settings.overrideShowNamesForEnemies = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowEnemyHP {
            get => Mod.Settings.overrideShowEnemyHP;
            set {
                if (Mod.Settings.overrideShowEnemyHP != value)
                {
                    Mod.Settings.overrideShowEnemyHP = value;
                    Update();
                }
            }
        }

        public static bool? EnemiesHPIsShort {
            get => Mod.Settings.overrideEnemiesHPIsShort;
            set {
                if (Mod.Settings.overrideEnemiesHPIsShort != value)
                {
                    Mod.Settings.overrideEnemiesHPIsShort = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowEnemyActions {
            get => Mod.Settings.overrideShowEnemyActions;
            set {
                if (Mod.Settings.overrideShowEnemyActions != value)
                {
                    Mod.Settings.overrideShowEnemyActions = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowEnemyIntentions {
            get => Mod.Settings.overrideShowEnemyIntentions;
            set {
                if (Mod.Settings.overrideShowEnemyIntentions != value)
                {
                    Mod.Settings.overrideShowEnemyIntentions = value;
                    Update();
                }
            }
        }

        public static DropdownState ShowNumericCooldownEnemies {
            get => Mod.Settings.overrideShowNumericCooldownEnemies;
            set {
                if (Mod.Settings.overrideShowNumericCooldownEnemies != value)
                {
                    Mod.Settings.overrideShowNumericCooldownEnemies = value;
                    Update();
                }
            }
        }

        public static void Update()
        {
            InteractionHighlightController controller = 
                Game.Instance.InteractionHighlightController ??
                Game.Instance.GetController<InteractionHighlightController>(true);
            if (controller != null)
                EventBus.RaiseEvent<IInteractionHighlightUIHandler>(h => h.HandleHighlightChange(controller.IsHighlighting));
        }

        private void HandleTogglePlayer()
        {
            PlayerToggle = !PlayerToggle;
        }

        private void HandleToggleEnemy()
        {
            EnemyToggle = !EnemyToggle;
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            Mod.Core.Information = this;
            if (PlayerToggle || EnemyToggle)
                Update();

            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_PLAYER_INFO, HandleTogglePlayer);
            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_ENEMY_INFO, HandleToggleEnemy);
            EventBus.Subscribe(this);
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            EventBus.Unsubscribe(this);
            HotkeyHelper.Unbind(HOTKEY_FOR_TOGGLE_PLAYER_INFO, HandleTogglePlayer);
            HotkeyHelper.Unbind(HOTKEY_FOR_TOGGLE_ENEMY_INFO, HandleToggleEnemy);

            if (PlayerToggle || EnemyToggle)
                Update();
            Mod.Core.Information = null;
        }

        public void OnAreaBeginUnloading() { }

        public void OnAreaDidLoad()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_PLAYER_INFO, HandleTogglePlayer);
            HotkeyHelper.Bind(HOTKEY_FOR_TOGGLE_ENEMY_INFO, HandleToggleEnemy);
        }

        // override settings
        [HarmonyPatch(typeof(SettingsEntityDropdownState), nameof(SettingsEntityDropdownState.CurrentState), MethodType.Getter)]
        static class SettingsEntityDropdownState_get_CurrentState_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(SettingsEntityDropdownState __instance, ref DropdownState __result)
            {
                if (Mod.Enabled)
                {
                    SettingsRoot.SettingsListScreen settings = SettingsRoot.Instance;
                    DropdownState result = default;

                    if (PlayerToggle)
                    {
                        if (__instance == settings.ShowNamesForParty)
                            result = ShowNamesForParty;
                        else if(__instance == settings.ShowPartyHP)
                            result = ShowPartyHP;
                        else if (__instance == settings.ShowPartyActions)
                            result = ShowPartyActions;
                        else if (__instance == settings.ShowPartyAttackIntentions)
                            result = ShowPartyAttackIntentions;
                        else if (__instance == settings.ShowPartyCastIntentions)
                            result = ShowPartyCastIntentions;
                        else if (__instance == settings.ShowNumericCooldownParty)
                            result = ShowNumericCooldownParty;

                        if (result != default)
                        {
                            __result = result;
                            return false;
                        }
                    }

                    if (EnemyToggle)
                    {
                        if (__instance == settings.ShowNamesForEnemies)
                            result = ShowNamesForEnemies;
                        else if (__instance == settings.ShowEnemyHP)
                            result = ShowEnemyHP;
                        else if (__instance == settings.ShowEnemyActions)
                            result = ShowEnemyActions;
                        else if (__instance == settings.ShowEnemyIntentions)
                            result = ShowEnemyIntentions;
                        else if (__instance == settings.ShowNumericCooldownEnemies)
                            result = ShowNumericCooldownEnemies;

                        if (result != default)
                        {
                            __result = result;
                            return false;
                        }
                    }
                }
                return true;
            }
        }

        // override settings
        [HarmonyPatch(typeof(OvertipController), "IsHPShort", MethodType.Getter)]
        static class OvertipController_get_IsHPShort_Patch
        {
            [HarmonyPrefix]
            static bool Prefix(OvertipController __instance, ref bool __result)
            {
                if (Mod.Enabled)
                {
                    if (__instance.IsEnemy)
                    {
                        if (EnemyToggle && EnemiesHPIsShort.HasValue)
                        {
                            __result = EnemiesHPIsShort.Value;
                            return false;
                        }
                    }
                    else if (PlayerToggle && PartyHPIsShort.HasValue)
                    {
                        __result = PartyHPIsShort.Value;
                        return false;
                    }
                }
                return true;
            }
        }
    }
}