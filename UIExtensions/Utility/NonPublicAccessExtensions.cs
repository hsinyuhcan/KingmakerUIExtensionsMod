using Kingmaker.EntitySystem.Entities;
using Kingmaker.UI.Group;
using System;
using System.Collections.Generic;
using UnityEngine;
using static ModMaker.Utility.ReflectionCache;

namespace UIExtensions.Utility
{
    public static class NonPublicAccessExtensions
    {
        public static List<GroupCharacter> GetCharacters(this GroupController group)
        {
            return group.GetFieldValue<GroupController, List<GroupCharacter>>("m_Characters");
        }

        public static GameObject GetNaviBlock(this GroupController group)
        {
            return group.GetFieldValue<GroupController, GameObject>("m_NaviBlock");
        }

        public static int GetStartIndex(this GroupController group)
        {
            return group.GetFieldValue<GroupController, int>("m_StartIndex");
        }

        public static bool GetWithPet(this GroupController group)
        {
            return group.GetPropertyValue<GroupController, bool>("WithPet");
        }

        public static bool GetWithRemote(this GroupController group)
        {
            return group.GetPropertyValue<GroupController, bool>("WithRemote");
        }

        public static void SetArrowsInteracteble(this GroupController group)
        {
            GetMethod<GroupController, Action<GroupController>>("SetArrowsInteracteble")(group);
        }

        public static void SetCharacter(this GroupController group, UnitEntityData character, int index)
        {
            GetMethod<GroupController, Action<GroupController, UnitEntityData, int>>("SetCharacter")(group, character, index);
        }

        public static void SetGroup(this GroupController group)
        {
            GetMethod<GroupController, Action<GroupController>>("SetGroup")(group);
        }
    }
}