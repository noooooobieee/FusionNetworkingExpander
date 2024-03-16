using BoneLib.BoneMenu.Elements;
using HarmonyLib;
using LabFusion.BoneMenu;
using LabFusion.Data;
using LabFusion.Extensions;
using LabFusion.Network;
using LabFusion.Preferences;
using LabFusion.Representation;
using LabFusion.Utilities;
using SLZ.Bonelab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RiptideNetworkLayer.Patching
{
    [HarmonyPatch(typeof(LabFusion.BoneMenu.BoneMenuCreator))]
    public static class PlayerListPatches
    {
        /// <summary>
        /// Removes the Ban button in the Tide layer.
        /// </summary>
        [HarmonyPostfix]
        [HarmonyPatch("RefreshPlayerList")]
        public static void ModifyPlayerMenu()
        {
            if (NetworkLayerDeterminer.LoadedTitle != "Riptide")
                return;

            Type type = typeof(BoneMenuCreator);
            FieldInfo fieldInfo = type.GetField("_playerListCategory", BindingFlags.NonPublic | BindingFlags.Static);
            MenuCategory playerListCategory = (MenuCategory)fieldInfo.GetValue(null);

            if (playerListCategory == null)
            {
                return;
            }

            List<MenuCategory> playerMenus = playerListCategory.Elements.Where(x => x is MenuCategory).Cast<MenuCategory>().ToList();
            if (playerMenus == null || playerMenus.Count == 0)
            {
                return;
            }

            foreach (var menu in playerMenus)
            {
                var moderationCategory = menu.Elements.Where(x => x is MenuCategory).Cast<MenuCategory>().FirstOrDefault(x => x.Name == "Moderation");
                if (moderationCategory == null)
                {
                    continue;
                }

                var banElement = moderationCategory.Elements.FirstOrDefault(x => x.Name == "Ban");

                if (banElement != null)
                {
                    moderationCategory.Elements.Remove(banElement);
                }
            }
        }
    }
}
