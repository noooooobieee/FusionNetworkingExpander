namespace FusionNetworkAddons.Utilities
{
    internal class RiptideNetworkingLoader
    {
        internal static void Load()
        {
            string sdkPath = PersistentData.GetPath($"RiptideNetworking.dll");
            File.WriteAllBytes(sdkPath, EmbeddedResource.LoadFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), ResourcePaths.RiptideNetworkingPath));

            MelonAssembly.LoadMelonAssembly(sdkPath);
        }
    }
}
