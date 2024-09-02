using HarmonyLib;
using LabFusion.Scene;

namespace FusionNetworkAddons
{
    internal partial class Mod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            NetStandardLoader.Load();
            RiptideNetworkingLoader.Load();

            NetworkLayer.RegisterLayersFromAssembly(Assembly.GetExecutingAssembly());

            LoggerInstance.Msg("FusionNetworkAddons Initialized!");
        }

        public override void OnLateInitializeMelon()
        {
            PlayerInfo.Initialize();
        }
    }
}
