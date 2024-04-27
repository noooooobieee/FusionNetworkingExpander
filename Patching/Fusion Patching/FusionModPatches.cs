using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LabFusion;
using LabFusion.Riptide.Utilities;
using RiptideNetworkLayer.Preferences;
using RiptideNetworkLayer.Utilities;

namespace RiptideNetworkLayer.Patching
{
    [HarmonyPatch(typeof(FusionMod))]
    public static class FusionModPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnInitializeNetworking")]
        public static void LoadNetworkLayer()
        {
            NetstandardLoader.Load();
            RiptideNetworkingLoader.Load();

            LabFusion.Network.NetworkLayer.RegisterLayersFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(FusionMod.OnInitializeMelon))]
        public static void OnInitializeMelon()
        {
            RiptidePreferences.OnInitializePreferences();

            PlayerInfo.InitializeUsername();
            PlayerInfo.InitializePlayerIPAddress();

            Layer.RiptideNetworkLayer.Instance.InitLANDiscovery();
        }
    }
}
