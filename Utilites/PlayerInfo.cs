using UnityEngine;
using UnityEngine.Networking;

using Il2CppOculus.Platform;
using Il2CppOculus.Platform.Models;

using Steamworks;

using LabFusion.Player;
using System.Net.Http;

namespace FusionNetworkAddons.Utilities
{
    /// <summary>
    /// Class for managing player info.
    /// </summary>
    public class PlayerInfo
    {
        public static string PlayerIpAddress { get; private set; } = "Unknown";
        public static string Username { get; private set; } = "Unknown";

        internal static void Initialize()
        {
            InitializePlayerIPAddress();
            InitializeUsername();

#if DEBUG
            MelonLogger.Msg($"Player IP Address: {PlayerIpAddress}");
            MelonLogger.Msg($"Username: {Username}");
#endif
        }

        private static void InitializeUsername()
        {
            if (Path.GetFileName(UnityEngine.Application.dataPath) == "BONELAB_Steam_Windows64_Data")
            {
                if (!SteamClient.IsValid)
                    SteamClient.Init(250820, false);

                Username = SteamClient.Name;
                SteamClient.Shutdown();
                return;
            }

            if (!HelperMethods.IsAndroid())
            {

                Core.Initialize("5088709007839657");
                Users.GetLoggedInUser().OnComplete((Message<User>.Callback)GetLoggedInUserCallback);
            }
            else
            {
                Core.Initialize("4215734068529064");
                Users.GetLoggedInUser().OnComplete((Message<User>.Callback)GetLoggedInUserCallback);
            }
        }

        private static void GetLoggedInUserCallback(Message<User> msg)
        {
            if (!msg.IsError)
            {
                Username = msg.Data.OculusID;            
            }
            else
            {
                MelonLogger.Error($"Failed to initalize Il2CppOculus username with error: {msg.error}\n{msg.error.Message}");
            }
        }

        private static void InitializePlayerIPAddress()
        {
            try
            {
                string link = "https://api.ipify.org";

                HttpClientHandler handler = new()
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                };

                HttpClient client = new(handler);

                HttpRequestMessage request = new()
                {
                    RequestUri = new Uri(link),
                    Method = HttpMethod.Get,
                };

                var response = client.Send(request);
                string responseString = response.Content.ReadAsStringAsync().Result;

                PlayerIpAddress = responseString;
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error when fetching IP address:");
                MelonLogger.Error(e);
            }
        }
    }
}
