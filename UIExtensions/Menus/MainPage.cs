using Kingmaker.UI.SettingsUI;
using ModMaker;
using ModMaker.Utility;
using System.Collections.Generic;
using UIExtensions.Features;
using UnityEngine;
using UnityModManagerNet;
using static Kingmaker.UI.SettingsUI.SettingsEntityDropdownState;
using static ModMaker.Utility.RichTextExtensions;
using static UIExtensions.Features.OverrideInformation;
using static UIExtensions.Main;

namespace UIExtensions.Menus
{
    public class MainPage : IMenuSelectablePage
    {
        private string _waitingHotkeyName;

        GUIStyle _buttonStyle;
        GUIStyle _downButtonStyle;
        GUIStyle _labelStyle;

        public string Name => Local["Menu_Tab_MainFeature"];

        public int Priority => 0;

        public void OnGUI(UnityModManager.ModEntry modEntry)
        {
            if (Mod == null || !Mod.Enabled)
                return;

            if (_buttonStyle == null)
            {
                _buttonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft };
                _downButtonStyle = new GUIStyle(_buttonStyle)
                {
                    focused = _buttonStyle.active,
                    normal = _buttonStyle.active,
                    hover = _buttonStyle.active
                };
                _downButtonStyle.active.textColor = Color.gray;
                _labelStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft, padding = _buttonStyle.padding };
            }

            using (new GUISubScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    if (GUILayout.Button(Local["Menu_Btn_ResetSettings"], _buttonStyle, GUILayout.ExpandWidth(false)))
                    {
                        Mod.Core.ResetSettings();
                    }
                }
            }

            using (new GUISubScope(Local["Menu_Sub_GroupBar"]))
                OnGUIGroupBar();

            using (new GUISubScope(Local["Menu_Sub_PlayerInfomation"]))
                OnGUIPlayerInfomation();

            using (new GUISubScope(Local["Menu_Sub_EnemyInfomation"]))
                OnGUIEnemyInfomation();

            using (new GUISubScope(Local["Menu_Sub_Intentions"]))
                OnGUIIntentions();

            using (new GUISubScope(Local["Menu_Sub_UIScale"]))
                OnGUIUIScale();

            using (new GUISubScope(Local["Menu_Sub_Hotkey"]))
                OnGUIHotkey();
        }

        private void OnGUIGroupBar()
        {
            AlwaysDisplayPetsPortrait.Toggle =
                GUIHelper.ToggleButton(AlwaysDisplayPetsPortrait.Toggle,
                Local["Menu_Opt_AlwaysDisplayPetsPortrait"], _buttonStyle, GUILayout.ExpandWidth(false));

            AlwaysDisplayHpText.Toggle =
                GUIHelper.ToggleButton(AlwaysDisplayHpText.Toggle,
                Local["Menu_Opt_AlwaysDisplayHpText"], _buttonStyle, GUILayout.ExpandWidth(false));

            DisplayPetsHpTextOnMastersPortrait.Toggle =
                GUIHelper.ToggleButton(DisplayPetsHpTextOnMastersPortrait.Toggle,
                Local["Menu_Opt_DisplayPetsHpTextOnMastersPortrait"], _buttonStyle, GUILayout.ExpandWidth(false));

            DisplayPetsHpBarOnMastersPortrait.LeftToggle =
                GUIHelper.ToggleButton(DisplayPetsHpBarOnMastersPortrait.LeftToggle,
                Local["Menu_Opt_DisplayPetsHpBarOnMastersPortraitLeft"], _buttonStyle, GUILayout.ExpandWidth(false));

            DisplayPetsHpBarOnMastersPortrait.RightToggle =
                GUIHelper.ToggleButton(DisplayPetsHpBarOnMastersPortrait.RightToggle,
                Local["Menu_Opt_DisplayPetsHpBarOnMastersPortraitRight"], _buttonStyle, GUILayout.ExpandWidth(false));
        }

        private void OnGUIPlayerInfomation()
        {
            PlayerToggle = 
                GUIHelper.ToggleButton(PlayerToggle,
                Local["Menu_Opt_OverridePlayerInfomation"], _buttonStyle, GUILayout.ExpandWidth(false));

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    Option(ShowNamesForParty, Local["Menu_Opt_ShowNamesForParty"]);
                    Option(ShowPartyHP, Local["Menu_Opt_ShowPartyHP"]);
                    Option(ShowPartyActions, Local["Menu_Opt_ShowPartyActions"]);
                    Option(ShowPartyAttackIntentions, Local["Menu_Opt_ShowPartyAttackIntentions"]);
                    Option(ShowPartyCastIntentions, Local["Menu_Opt_ShowPartyCastIntentions"]);
                    Option(ShowNumericCooldownParty, Local["Menu_Opt_ShowNumericCooldownParty"]);
                }

                using (new GUILayout.VerticalScope())
                {
                    ShowNamesForParty = DropdownStateSelection(ShowNamesForParty);
                    ShowPartyHP = DropdownStateSelection(ShowPartyHP);
                    ShowPartyActions = DropdownStateSelection(ShowPartyActions, false, true);
                    ShowPartyAttackIntentions = DropdownStateSelection(ShowPartyAttackIntentions);
                    ShowPartyCastIntentions = DropdownStateSelection(ShowPartyCastIntentions);
                    ShowNumericCooldownParty = DropdownStateSelection(ShowNumericCooldownParty);
                }
            }

            void Option(DropdownState value, string name)
            {
                GUIHelper.ToggleButton(value != DropdownState.None, name, _labelStyle, GUILayout.ExpandWidth(false));
            }
        }

        private void OnGUIEnemyInfomation()
        {
            EnemyToggle =
                GUIHelper.ToggleButton(EnemyToggle,
                Local["Menu_Opt_OverrideEnemyInfomation"], _buttonStyle, GUILayout.ExpandWidth(false));

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    Option(ShowNamesForEnemies, Local["Menu_Opt_ShowNamesForEnemies"]);
                    Option(ShowEnemyHP, Local["Menu_Opt_ShowEnemyHP"]);
                    Option(ShowEnemyActions, Local["Menu_Opt_ShowEnemyActions"]);
                    Option(ShowEnemyIntentions, Local["Menu_Opt_ShowEnemyIntentions"]);
                    Option(ShowNumericCooldownEnemies, Local["Menu_Opt_ShowNumericCooldownEnemies"]);
                }

                using (new GUILayout.VerticalScope())
                {
                    ShowNamesForEnemies = DropdownStateSelection(ShowNamesForEnemies, true);
                    ShowEnemyHP = DropdownStateSelection(ShowEnemyHP, true);
                    ShowEnemyActions = DropdownStateSelection(ShowEnemyActions, true);
                    ShowEnemyIntentions = DropdownStateSelection(ShowEnemyIntentions, true);
                    ShowNumericCooldownEnemies = DropdownStateSelection(ShowNumericCooldownEnemies, true);
                }
            }

            void Option(DropdownState value, string name)
            {
                GUIHelper.ToggleButton(value != DropdownState.None, name, _labelStyle, GUILayout.ExpandWidth(false));
            }
        }

        private DropdownState DropdownStateSelection(DropdownState value, bool isEnemy = false, bool showIdle = false)
        {
            using (new GUILayout.HorizontalScope())
            {
                DropdownState result = value;

                Button(Local["Menu_Btn_None"], DropdownState.None);
                Button(Local["Menu_Btn_Never"], DropdownState.Never);
                Button(Local[!isEnemy ? "Menu_Btn_InteractionPlayer" : "Menu_Btn_InteractionEnemy"], DropdownState.Interaction);
                Button(Local["Menu_Btn_ForcedShow"], DropdownState.ForcedShow);
                Button(Local["Menu_Btn_OnPause"], DropdownState.OnPause);
                Button(Local["Menu_Btn_Always"], DropdownState.Always);
                if (showIdle)
                {
                    Button(Local["Menu_Btn_Idleness"], DropdownState.Idleness);
                }

                void Button(string text, DropdownState state)
                {
                    if (GUILayout.Button(text, value == state ? _downButtonStyle : _buttonStyle, GUILayout.ExpandWidth(false)))
                    {
                        result = state;
                    }
                }

                return result;
            }
        }

        private void OnGUIIntentions()
        {
            Intentions.CanShowWhenNotHovering =
                GUIHelper.ToggleButton(Intentions.CanShowWhenNotHovering,
                Local["Menu_Opt_CanShowWhenNotHovering"] +
                Local["Menu_Cmt_CanShowWhenNotHovering"].Color(RGBA.silver), _buttonStyle, GUILayout.ExpandWidth(false));

            Intentions.CanShowWhenNotPausing =
                GUIHelper.ToggleButton(Intentions.CanShowWhenNotPausing,
                Local["Menu_Opt_CanShowWhenNotPausing"] +
                Local["Menu_Cmt_CanShowWhenNotPausing"].Color(RGBA.silver), _buttonStyle, GUILayout.ExpandWidth(false));
        }

        private void OnGUIUIScale()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUIHelper.ToggleButton(true,
                   string.Format(Local["Menu_Opt_UIScaleModifier"], UIScale.Modifier), _labelStyle, GUILayout.ExpandWidth(false));
                UIScale.Modifier =
                    GUIHelper.RoundedHorizontalSlider(UIScale.Modifier, 2, 0.75f, 1.25f, GUILayout.Width(100f), GUILayout.ExpandWidth(false));
            }
        }

        private void OnGUIHotkey()
        {
            if (!string.IsNullOrEmpty(_waitingHotkeyName) && HotkeyHelper.ReadKey(out BindingKeysData newKey))
            {
                Mod.Core.Hotkeys.SetHotkey(_waitingHotkeyName, newKey);
                _waitingHotkeyName = null;
            }

            IDictionary<string, BindingKeysData> hotkeys = Mod.Core.Hotkeys.Hotkeys;

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    foreach (KeyValuePair<string, BindingKeysData> item in hotkeys)
                    {
                        GUIHelper.ToggleButton(item.Value != null, Local[item.Key], _labelStyle, GUILayout.ExpandWidth(false));
                    }
                }

                GUILayout.Space(10f);

                using (new GUILayout.VerticalScope())
                {
                    foreach (BindingKeysData key in hotkeys.Values)
                    {
                        GUILayout.Label(HotkeyHelper.GetKeyText(key));
                    }
                }

                GUILayout.Space(10f);

                using (new GUILayout.VerticalScope())
                {
                    foreach (string name in hotkeys.Keys)
                    {
                        bool waitingThisHotkey = _waitingHotkeyName == name;
                        if (GUILayout.Button(Local["Menu_Btn_Set"], waitingThisHotkey ? _downButtonStyle : _buttonStyle))
                        {
                            if (waitingThisHotkey)
                                _waitingHotkeyName = null;
                            else
                                _waitingHotkeyName = name;
                        }
                    }
                }

                using (new GUILayout.VerticalScope())
                {
                    string hotkeyToClear = default;
                    foreach (string name in hotkeys.Keys)
                    {
                        if (GUILayout.Button(Local["Menu_Btn_Clear"], _buttonStyle))
                        {
                            hotkeyToClear = name;

                            if (_waitingHotkeyName == name)
                                _waitingHotkeyName = null;
                        }
                    }
                    if (!string.IsNullOrEmpty(hotkeyToClear))
                        Mod.Core.Hotkeys.SetHotkey(hotkeyToClear, null);
                }

                using (new GUILayout.VerticalScope())
                {
                    foreach (KeyValuePair<string, BindingKeysData> item in hotkeys)
                    {
                        if (item.Value != null && !HotkeyHelper.CanBeRegistered(item.Key, item.Value))
                        {
                            GUILayout.Label(Local["Menu_Txt_Duplicated"].Color(RGBA.yellow));
                        }
                        else
                        {
                            GUILayout.Label(string.Empty);
                        }
                    }
                }

                GUILayout.FlexibleSpace();
            }
        }
    }
}