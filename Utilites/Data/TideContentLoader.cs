using BoneLib;
using LabFusion.Data;
using LabFusion.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RiptideNetworkLayer.Utilities
{
    /// <summary>
    /// Used for loading Tide assets from the embedded resources.
    /// </summary>
    public static class TideContentLoader
    {
        public static T LoadPersistentAsset<T>(this AssetBundle bundle, string name) where T : UnityEngine.Object
        {
            var asset = bundle.LoadAsset(name);

            if (asset != null)
            {
                asset.hideFlags = HideFlags.DontUnloadUnusedAsset;
                return asset.TryCast<T>();
            }

            return null;
        }

        public static AssetBundle LoadAssetBundle(string name)
        {
            // Android
            if (HelperMethods.IsAndroid())
            {
                return EmbeddedAssetBundle.LoadFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), ResourcePaths.AndroidBundlePrefix + name);
            }
            // Windows
            else
            {
                return EmbeddedAssetBundle.LoadFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), ResourcePaths.WindowsBundlePrefix + name);
            }
        }

        public static AssetBundle TideBundle { get; private set; }

        public static GameObject KeyboardPrefab { get; private set; }

        public static void OnBundleLoad()
        {
            TideBundle = LoadAssetBundle(ResourcePaths.TideBundle);

            if (TideBundle != null)
            {
                KeyboardPrefab = TideBundle.LoadPersistentAsset<GameObject>(RiptideNetworkLayer.Utilities.ResourcePaths.KeyboardPrefab);
            }
        }
    }
}
