using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiptideNetworkLayer.Utilities
{
    /// <summary>
    /// Contains different paths to embedded resources within the TIDE layer.
    /// </summary>
    internal static class ResourcePaths
    {
        internal const string RiptideNetworkingPath = "RiptideNetworkLayer.Resources.lib.RiptideNetworking.dll";
        internal const string netstandardPath = "RiptideNetworkLayer.Resources.lib.netstandard.dll";
        internal const string NetDiscoveryPath = "RiptideNetworkLayer.Resources.lib.NetDiscovery.dll";
        internal const string NetDiscoveryUdpPath = "RiptideNetworkLayer.Resources.lib.NetDiscovery.Udp.dll";

        public const string WindowsBundlePrefix = "RiptideNetworkLayer.Resources.bundles.StandaloneWindows64.";
        public const string AndroidBundlePrefix = "RiptideNetworkLayer.Resources.bundles.Android.";

        // Bundle
        public const string TideBundle = "tidebundle.fusion";

        public const string KeyboardPrefab = "ui_Keyboard";
    }
}
