using LabFusion.Data;
using LabFusion.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiptideNetworkLayer.Utilities
{
    internal class NetstandardLoader
    {
        internal static void Load()
        {
            FusionLogger.Log("Loading netstandard");

            string sdkPath = PersistentData.GetPath($"netstandard.dll");
            File.WriteAllBytes(sdkPath, EmbeddedResource.LoadFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), ResourcePaths.netstandardPath));

            MelonLoader.MelonAssembly.LoadMelonAssembly(sdkPath);
        }
    }
}
