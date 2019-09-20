using Kingmaker.UI.SettingsUI;
using ModMaker.Utility;
using UnityModManagerNet;
using static Kingmaker.UI.SettingsUI.SettingsEntityDropdownState;

namespace UIExtensions
{
    public class Settings : UnityModManager.ModSettings
    {
        public string lastModVersion;

        // group bar
        public bool toggleAlwaysDisplayPetsPortrait;
        public bool toggleAlwaysDisplayHpText = true;
        public bool toggleDisplayPetsHpTextOnMastersPortrait = true;
        public bool toggleDisplayPetsHpBarOnMastersPortraitLeft;
        public bool toggleDisplayPetsHpBarOnMastersPortraitRight = true;

        // inspector
        public bool toggleChangePositionOfInspectorLeft;
        public bool toggleChangePositionOfInspectorTurnBased;

        // override information in combat
        public bool toggleOverrideInfoForPlayer;
        public DropdownState overrideShowNamesForParty = DropdownState.Interaction;
        public DropdownState overrideShowPartyHP = DropdownState.Always;
        public DropdownState overrideShowPartyActions = DropdownState.Always;
        public DropdownState overrideShowPartyAttackIntentions = DropdownState.ForcedShow;
        public DropdownState overrideShowPartyCastIntentions = DropdownState.ForcedShow;
        public DropdownState overrideShowNumericCooldownParty = DropdownState.Always;

        public bool toggleOverrideInfoForEnemy;
        public DropdownState overrideShowNamesForEnemies = DropdownState.Interaction;
        public DropdownState overrideShowEnemyHP = DropdownState.Always;
        public DropdownState overrideShowEnemyActions = DropdownState.Always;
        public DropdownState overrideShowEnemyIntentions = DropdownState.ForcedShow;
        public DropdownState overrideShowNumericCooldownEnemies = DropdownState.Always;

        // intentions
        public bool toggleCanShowIntentionsWhenNotHovering = true;
        public bool toggleCanShowIntentionsWhenNotPausing = true;

        // ui scale
        public float uiScaleModifier = 1f;

        // hotkeys
        public SerializableDictionary<string, BindingKeysData> hotkeys = new SerializableDictionary<string, BindingKeysData>();

        // localization
        public string localizationFileName;
    }
}