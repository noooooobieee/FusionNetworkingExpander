using LabFusion.Preferences;
using LabFusion.Riptide.Utilities;
using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiptideNetworkLayer.Preferences
{
    /// <summary>
    /// Class for creating FusionPreferences in a separate MelonPreferences category.
    /// </summary>
    public static class RiptidePreferences
    {
        internal static MelonPreferences_Category TideCategory = MelonPreferences.CreateCategory("BONELAB TideFusion");

        public class ServerSettings
        {
            public IFusionPref<ushort> ServerPort;

            public static ServerSettings CreateMelonPrefs()
            {
                return new ServerSettings()
                {
                    ServerPort = new FusionPref<ushort>(TideCategory, "ServerPort", 7777),
                };
            }
        }

        internal static ServerSettings LocalServerSettings;

        internal static void OnInitializePreferences()
        {
            TideCategory = MelonPreferences.CreateCategory("RiptideNetworkLayer");

            // Server settings
            LocalServerSettings = ServerSettings.CreateMelonPrefs();
        }
    }
}
