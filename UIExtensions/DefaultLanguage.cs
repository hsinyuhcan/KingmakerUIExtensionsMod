using Kingmaker.EntitySystem.Persistence.JsonUtility;
using ModMaker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace UIExtensions
{
    [JsonObject(MemberSerialization.OptOut)]
    public class DefaultLanguage : ILanguage
    {
        [JsonProperty]
        public string Language { get; set; } = "English (Default)";

        [JsonProperty]
        public Version Version { get; set; }

        [JsonProperty]
        public string Contributors { get; set; }

        [JsonProperty]
        public string HomePage { get; set; }

        [JsonProperty]
        public Dictionary<string, string> Strings { get; set; } = new Dictionary<string, string>()
        {
            { "Menu_Tab_MainFeature", "Main Feature" },
            { "Menu_Btn_ResetSettings", "Reset Settings" },
            { "Menu_Sub_GroupBar", "Group Bar" },
            { "Menu_Opt_AlwaysDisplayPetsPortrait", "Always Display Pet's Portrait" },
            { "Menu_Opt_AlwaysDisplayHpText", "Always Display HP Text" },
            { "Menu_Opt_DisplayPetsHpTextOnMastersPortrait", "Display Pets HP Text On Master's Portrait" },
            { "Menu_Opt_DisplayPetsHpBarOnMastersPortraitLeft", "Display Pets HP Bar On Master's Portrait (Left)" },
            { "Menu_Opt_DisplayPetsHpBarOnMastersPortraitRight", "Display Pets HP Bar On Master's Portrait (Right)" },
            { "Menu_Sub_PlayerInfomation", "Party Infomation In Combat" },
            { "Menu_Opt_OverridePlayerInfomation", "Override Base Game Settings Using Following Settings" },
            { "Menu_Opt_ShowNamesForParty", "Show names of party members" },
            { "Menu_Opt_ShowPartyHP", "Show party member HP" },
            { "Menu_Opt_ShowPartyActions", "Show party members' action" },
            { "Menu_Opt_ShowPartyAttackIntentions", "Show attack intentions of party members" },
            { "Menu_Opt_ShowPartyCastIntentions", "Show spellcasting intentions of party members" },
            { "Menu_Opt_ShowNumericCooldownParty", "Show cooldown timers for party members" },
            { "Menu_Sub_EnemyInfomation", "Enemy Infomation In Combat" },
            { "Menu_Opt_OverrideEnemyInfomation", "Override Base Game Settings Using Following Settings" },
            { "Menu_Opt_ShowNamesForEnemies", "Show enemy name" },
            { "Menu_Opt_ShowEnemyHP", "Show enemy HP" },
            { "Menu_Opt_ShowEnemyActions", "Show enemy actions" },
            { "Menu_Opt_ShowEnemyIntentions", "Show enemy intentions" },
            { "Menu_Opt_ShowNumericCooldownEnemies", "Show cooldown timers for enemies" },
            { "Menu_Btn_None", "None" },
            { "Menu_Btn_Never", "Never" },
            { "Menu_Btn_Idleness", "When Idle" },
            { "Menu_Btn_InteractionPlayer", "On Selection" },
            { "Menu_Btn_InteractionEnemy", "On Hover" },
            { "Menu_Btn_ForcedShow", "On Highlight Hotkey" },
            { "Menu_Btn_OnPause", "On Pause" },
            { "Menu_Btn_Always", "Always" },
            { "Menu_Sub_Intentions", "Intentions" },
            { "Menu_Opt_CanShowWhenNotHovering", "Can Show Unit Intentions When Not Hovering" },
            { "Menu_Cmt_CanShowWhenNotHovering", " (In base game the attack lines can be shown only on hovering regardless settings)" },
            { "Menu_Opt_CanShowWhenNotPausing", "Can Show Unit Intentions When Not Pausing" },
            { "Menu_Cmt_CanShowWhenNotPausing", " (In base game the attack lines can be shown only on pausing regardless settings)" },
            { "Menu_Sub_UIScale", "UI Scale" },
            { "Menu_Opt_UIScaleModifier", "Overall Modifier: {0:f2}x" },
            { "Menu_Sub_Hotkey", "Hotkey" },
            { "Menu_Btn_Set", "Set" },
            { "Menu_Btn_Clear", "Clear" },
            { "Menu_Txt_Duplicated", "Duplicated!!" },
            { "Menu_Tab_Language", "Language" },
            { "Menu_Sub_Current", "Current" },
            { "Menu_Txt_Language", "Language: {0}" },
            { "Menu_Txt_Version", "Version: {0}" },
            { "Menu_Txt_Contributors", "Contributors: {0}" },
            { "Menu_Txt_HomePage", "Home Page:" },
            { "Menu_Btn_Export", "Export: {0}" },
            { "Menu_Btn_SortAndExport", "Sort And Export: {0}" },
            { "Menu_Cmt_SortAndExport", " (Warning: it will delete all unused entries, too)" },
            { "Menu_Txt_FaildToExport", "Faild to export: {0}" },
            { "Menu_Sub_Import", "Import" },
            { "Menu_Btn_RefreshFileList", "Refresh File List" },
            { "Menu_Btn_DefaultLanguage", "Default Language" },
            { "Menu_Txt_FaildToImport", "Faild to import: {0}" },
            { "Hotkey_Toggle_HUD", "Toggle Hide / Restore HUD" },
            { "Hotkey_Toggle_Player_Info", "Toggle Override Party Info Settings" },
            { "Hotkey_Toggle_Enemy_Info", "Toggle Override Enemy Info Settings" },
            { "Hotkey_Toggle_Stealth", "Toggle Stealth" },
            { "Hotkey_Toggle_AI", "Toggle AI" }
        };

        public T Deserialize<T>(TextReader reader)
        {
            DefaultJsonSettings.Initialize();
            return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
        }

        public void Serialize<T>(TextWriter writer, T obj)
        {
            DefaultJsonSettings.Initialize();
            writer.Write(JsonConvert.SerializeObject(obj, Formatting.Indented));
        }
    }
}