#if FUSION_ENABLED
using HarmonyLib;
using UnityEngine;

namespace FusionNetworkingPlus.Patches.Fusion
{
    [HarmonyPatch]
    public static class ConnectionPatches
    {
        // Example: patch a hypothetical Fusion "NetworkRunner" class's Connect method
        [HarmonyPatch(typeof(NetworkRunner), "Connect")]
        [HarmonyPrefix]
        public static bool Connect_Prefix(ref string address, ref ushort port)
        {
            Debug.Log($"[ConnectionPatches] Intercepting Connect -> Address: {address}, Port: {port}");
            // You could modify address/port or block the call entirely by returning false.
            return true; 
        }

        [HarmonyPatch(typeof(NetworkRunner), "Disconnect")]
        [HarmonyPostfix]
        public static void Disconnect_Postfix()
        {
            Debug.Log("[ConnectionPatches] Post-disconnect patch triggered.");
        }
    }
}
#endif
