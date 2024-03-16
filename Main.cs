using MelonLoader;
using RiptideNetworkLayer.Utilities;

namespace RiptideNetworkLayer
{
    internal partial class Main : MelonMod
    {
        public override void OnEarlyInitializeMelon()
        {
            NetstandardLoader.Load();
            RiptideNetworkingLoader.Load();

            LabFusion.Network.NetworkLayer.RegisterLayersFromAssembly(System.Reflection.Assembly.GetExecutingAssembly());

            TideContentLoader.OnBundleLoad();
        }
    }
}
