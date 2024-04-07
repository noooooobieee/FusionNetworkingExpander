using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using LabFusion;
using RiptideNetworkLayer.Utilities;

namespace RiptideNetworkLayer.Patching
{
    [HarmonyPatch(typeof(FusionMod))]
    public static class FusionModPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(FusionMod.OnInitializeNetworking))]
        public static void LoadNetworkLayer()
        {
            NetstandardLoader.Load();
            RiptideNetworkingLoader.Load();

            LabFusion.Network.NetworkLayer.RegisterLayersFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());
        }
    }
}
