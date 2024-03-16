using LabFusion.Data;
using LabFusion.Utilities;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiptideNetworkLayer.Utilities
{
    internal class RiptideNetworkingLoader
    {
        internal static void Load()
        {
            string sdkPath = PersistentData.GetPath($"RiptideNetworking.dll");
            File.WriteAllBytes(sdkPath, EmbeddedResource.LoadFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), RiptideNetworkLayer.Utilities.ResourcePaths.RiptideNetworkingPath));

            MelonLoader.MelonAssembly.LoadMelonAssembly(sdkPath);
        }
    }
}
