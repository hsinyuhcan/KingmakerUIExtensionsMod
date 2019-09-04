using Harmony12;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.Common;
using Kingmaker.UI.Group;
using ModMaker;
using ModMaker.Utility;
using System.Reflection;
using TMPro;
using UIExtensions.Utility;
using UnityEngine;
using static UIExtensions.Main;

namespace UIExtensions.Features
{
    public class DisplayPetsHpTextOnMastersPortrait : IModEventHandler
    {
        const string PET_HP_CONTAINER_NAME = "PetHitPoints";

        public int Priority => 800;

        public static bool Toggle {
            get => Mod.Settings.toggleDisplayPetsHpTextOnMastersPortrait;
            set {
                if (Mod.Settings.toggleDisplayPetsHpTextOnMastersPortrait != value)
                {
                    Mod.Settings.toggleDisplayPetsHpTextOnMastersPortrait = value;
                    UpdateGroup(value, value);
                }
            }
        }

        public static bool IsEnabled()
        {
            return Mod.Enabled && Toggle;
        }

        public static void Update()
        {
            UpdateGroup(false, false);
            if (Toggle)
                UpdateGroup(true, true);
        }

        static void UpdateGroup(bool isEnabled, bool updateValue)
        {
            if (GroupController.Instance == null)
                return;

            Mod.Debug(MethodBase.GetCurrentMethod(), isEnabled, updateValue);

            foreach (GroupCharacter character in GroupController.Instance.GetCharacters())
            {
                if (isEnabled)
                    UpdateCharacter(character, updateValue);
                else
                    Detach(character);
            }
        }

        static void UpdateCharacter(CharacterBase character, bool updateValue = true)
        {
            UnitEntityData pet = character.Unit?.Descriptor.Pet;
            if (pet != null)
            {
                TextMeshProUGUI petHpText = Attach(character);
                if (updateValue)
                {
                    UpdateText(character, pet, petHpText);
                }
            }
            else
            {
                Detach(character);
            }
        }

        static void UpdateText(CharacterBase character, UnitEntityData pet, TextMeshProUGUI text)
        {
            bool isDead = pet.Descriptor.State.IsFinallyDead;
            text.alignment = TextAlignmentOptions.BottomRight;
            text.text = UIUtility.GetHpText(pet, isDead, 80);
            text.color = (!isDead) ? character.Portrait.NormalHpColor : character.Portrait.DeathHpColor;
        }

        static TextMeshProUGUI Attach(CharacterBase character)
        {
            GameObject masterHP = character.Portrait.HitPointsContainer.gameObject;
            GameObject petHp = masterHP.transform.Find(PET_HP_CONTAINER_NAME)?.gameObject;
            if (petHp)
                return petHp.GetComponentInChildren<TextMeshProUGUI>();

            Mod.Debug(MethodBase.GetCurrentMethod(), character.name);

            // Portrait ─ <GameObject> HitPointsContainer ┬ <Image> HpBackground
            //                                            ├ <GameObject> Hp ─ <TextMeshProUGUI> HpText
            //                                            └ <GameObject> PetHpContainer ┬ <Image> PetHpBackground
            //                                                                          └ <GameObject> PetHp ─ <TextMeshProUGUI> PetHpText

            petHp = Object.Instantiate(masterHP, masterHP.transform);
            petHp.name = PET_HP_CONTAINER_NAME;
            petHp.GetComponentInChildren<CanvasGroup>().alpha = 1f;
            TextMeshProUGUI petHpText = petHp.GetComponentInChildren<TextMeshProUGUI>();

            // the pivot of the portrait is in the upper left corner, but we need the center point
            RectTransform rectPortrait = (RectTransform)character.Portrait.gameObject.transform;
            float xPortraitCenter =
                rectPortrait.position.x +
                rectPortrait.rect.width * (0.5f - rectPortrait.pivot.x) * rectPortrait.lossyScale.x;
            float yPortraitCenter =
                rectPortrait.position.y +
                rectPortrait.rect.height * (0.5f - rectPortrait.pivot.y) * rectPortrait.lossyScale.y;

            // transform of the container object
            RectTransform rectHp = (RectTransform)masterHP.transform;
            RectTransform rectPetHp = (RectTransform)petHp.transform;
            rectPetHp.anchorMin = new Vector2(0.0f, 0.0f);
            rectPetHp.anchorMax = new Vector2(1.0f, 1.0f);
            rectPetHp.sizeDelta = new Vector2(0.0f, 0.0f);
            rectPetHp.pivot = new Vector2(1f - rectHp.pivot.x, 1f - rectHp.pivot.y);
            rectPetHp.position = new Vector3(
                xPortraitCenter * 2f - rectHp.position.x,
                yPortraitCenter * 2f - rectHp.position.y,
                rectHp.position.z);

            // transform of the text object
            RectTransform rectHpText = (RectTransform)character.Portrait.HitPoints.gameObject.transform;
            RectTransform rectPetHpText = (RectTransform)petHpText.gameObject.transform;
            rectPetHpText.pivot = new Vector2(1f - rectHpText.pivot.x, 1f - rectHpText.pivot.y);
            rectPetHpText.position = new Vector3(
                xPortraitCenter * 2f - rectHpText.position.x,
                yPortraitCenter * 2f - rectHpText.position.y,
                rectHpText.position.z);

            return petHpText;
        }

        static void Detach(CharacterBase character)
        {
            GameObject petHp;
            while (petHp = character.Portrait.HitPointsContainer.transform.Find(PET_HP_CONTAINER_NAME)?.gameObject)
            {
                Mod.Debug(MethodBase.GetCurrentMethod(), character.name);

                petHp.SafeDestroy();
            }
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (Toggle)
                UpdateGroup(true, true);
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (Toggle)
                UpdateGroup(false, false);
        }

        [HarmonyPatch(typeof(CharacterBase), "Update")]
        static class CharacterBase_Update_Patch
        {
            [HarmonyPostfix]
            static void Postfix(CharacterBase __instance)
            {
                if (IsEnabled() && __instance.Portrait.HitPointsContainer.alpha > 0f)
                {
                    UpdateCharacter(__instance, true);
                }
            }
        }
    }
}