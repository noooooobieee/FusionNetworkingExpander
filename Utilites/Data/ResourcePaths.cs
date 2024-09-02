namespace FNPlus.Utilities
{
    internal static class ResourcePaths
    {
        // Libs
        internal const string RiptideNetworkingPath = "FNPlus.Resources.lib.RiptideNetworking.dll";
        internal const string netstandardPath = "FNPlus.Resources.lib.netstandard.dll";

        // Data saving
        internal static string FNPlusDataPath { get; } = $"{MelonEnvironment.UserDataDirectory}/UserData/FNPlus/";
    }
}
