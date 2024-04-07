using MelonLoader;
using RiptideNetworkLayer.Utilities;

namespace RiptideNetworkLayer
{
    internal partial class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            TideContentLoader.OnBundleLoad();
        }
    }
}
