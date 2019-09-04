using Harmony12;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.Group;
using ModMaker;
using ModMaker.Utility;
using System.Reflection;
using UIExtensions.Utility;
using UnityEngine;
using UnityEngine.UI;
using static UIExtensions.Main;

namespace UIExtensions.Features
{
    public class DisplayPetsHpBarOnMastersPortrait : IModEventHandler
    {
        const string PET_HP_SLIDER_NAME = "PetHealth";

        public int Priority => 800;

        public static bool LeftToggle {
            get => Mod.Settings.toggleDisplayPetsHpBarOnMastersPortraitLeft;
            set {
                if (Mod.Settings.toggleDisplayPetsHpBarOnMastersPortraitLeft != value)
                {
                    if (value)
                    {
                        RightToggle = false;
                    }
                    Mod.Settings.toggleDisplayPetsHpBarOnMastersPortraitLeft = value;
                    UpdateGroup(value, value);
                }
            }
        }

        public static bool RightToggle {
            get => Mod.Settings.toggleDisplayPetsHpBarOnMastersPortraitRight;
            set {
                if (Mod.Settings.toggleDisplayPetsHpBarOnMastersPortraitRight != value)
                {
                    if (value)
                    {
                        LeftToggle = false;
                    }
                    Mod.Settings.toggleDisplayPetsHpBarOnMastersPortraitRight = value;
                    UpdateGroup(value, value);
                }
            }
        }

        public static bool IsEnabled()
        {
            return Mod.Enabled && (LeftToggle || RightToggle);
        }

        public static void Update()
        {
            UpdateGroup(false, false);
            if (LeftToggle || RightToggle)
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

        static void UpdateCharacter(CharacterBase character, bool updateValue)
        {
            UnitEntityData pet = character.Unit?.Descriptor.Pet;
            if (pet != null)
            {
                Slider petHpSlider = Attach(character);
                if (updateValue)
                {
                    UpdateSlider(character, pet, petHpSlider);
                }
            }
            else
            {
                Detach(character);
            }
        }

        static void UpdateSlider(CharacterBase character, UnitEntityData pet, Slider slider)
        {
            float value = (float)pet.HPLeft / pet.MaxHP;
            if (slider.value == value)
                return;

            slider.value = value;
            slider.targetGraphic.color = 
                value < 0.5f ? Color.Lerp(Color.red, Color.yellow, value) : Color.Lerp(Color.yellow, Color.green, value);
        }

        static Slider Attach(CharacterBase character)
        {
            GameObject masterHP = character.Health.gameObject;
            GameObject petHp = masterHP.transform.Find(PET_HP_SLIDER_NAME)?.gameObject;
            if (petHp)
                return petHp.GetComponent<Slider>();

            Mod.Debug(MethodBase.GetCurrentMethod(), character.name);

            petHp = Object.Instantiate(masterHP, masterHP.transform, true);
            petHp.name = PET_HP_SLIDER_NAME;
            petHp.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.8f);

            RectTransform rectPortrait = (RectTransform)character.Portrait.gameObject.transform;
            RectTransform rectPetHp = (RectTransform)petHp.transform;

            if (LeftToggle)
            {
                rectPetHp.position = new Vector3(
                    rectPortrait.position.x,
                    rectPetHp.position.y,
                    rectPetHp.position.z);
            }
            else
            {
                rectPetHp.pivot = new Vector2(1f - rectPetHp.pivot.x, rectPetHp.pivot.y);
                rectPetHp.position = new Vector3(
                    rectPortrait.position.x + rectPortrait.rect.width * rectPortrait.lossyScale.x,
                    rectPetHp.position.y,
                    rectPetHp.position.z);
            }

            return petHp.GetComponent<Slider>();
        }

        static void Detach(CharacterBase character)
        {
            GameObject petHp;
            while (petHp = character.Health.gameObject.transform.Find(PET_HP_SLIDER_NAME)?.gameObject)
            {
                Mod.Debug(MethodBase.GetCurrentMethod(), character.name);

                petHp.SafeDestroy();
            }
        }

        public void HandleModEnable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (LeftToggle || RightToggle)
                UpdateGroup(true, true);
        }

        public void HandleModDisable()
        {
            Mod.Debug(MethodBase.GetCurrentMethod());

            if (LeftToggle || RightToggle)
                UpdateGroup(false, false);
        }

        [HarmonyPatch(typeof(CharacterBase), "Update")]
        static class CharacterBase_Update_Patch
        {
            [HarmonyPostfix]
            static void Postfix(CharacterBase __instance)
            {
                if (IsEnabled())
                {
                    UpdateCharacter(__instance, true);
                }
            }
        }
    }
}