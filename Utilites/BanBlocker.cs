using FNPlus.Network;

namespace FNPlus.Utilites
{
    internal class BanBlocker
    {
        public static List<Type> BlockedLayers = new()
        {
            { typeof(RiptideNetworkLayer) },
        };

        [HarmonyLib.HarmonyPrefix]
        [HarmonyLib.HarmonyPatch(typeof(BanList), nameof(BanList.Ban))]
        public static bool BlockBan(ulong longId, string username, string reason)
        {
            if (BlockedLayers.Contains(NetworkLayerDeterminer.LoadedLayer.Type))
            {
                FusionNotifier.Send(new FusionNotification()
                {
                    showTitleOnPopup = false,
                    type = NotificationType.WARNING,
                    message = "You cannot ban players on this Network Layer.",
                });
                return false;
            }

            return true;
        }
    }
}
