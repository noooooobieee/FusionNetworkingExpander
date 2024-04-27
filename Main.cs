using LabFusion;
using LabFusion.BoneMenu;
using MelonLoader;
using RiptideNetworkLayer.Utilities;
using System;
using System.Reflection;

namespace RiptideNetworkLayer
{
    internal partial class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            BoneLib.Hooking.OnLevelInitialized += (levelInfo) =>
            {
                Type type = FusionMod.FusionAssembly.GetType("LabFusion.Utilities.PersistentAssetCreator");
                MethodInfo methodInfo = type.GetMethod("CreateTextFont", BindingFlags.NonPublic | BindingFlags.Static);
                methodInfo.Invoke(null, null);
            };

            TideContentLoader.OnBundleLoad();
        }
    }
}
