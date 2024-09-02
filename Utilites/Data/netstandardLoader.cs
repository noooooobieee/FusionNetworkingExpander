namespace FNPlus.Utilities
{
    internal class NetStandardLoader
    {
        internal static void Load()
        {
            string sdkPath = PersistentData.GetPath($"netstandard.dll");
            File.WriteAllBytes(sdkPath, EmbeddedResource.LoadFromAssembly(System.Reflection.Assembly.GetExecutingAssembly(), ResourcePaths.netstandardPath));

            MelonAssembly.LoadMelonAssembly(sdkPath);
        }
    }
}
