using Il2CppCysharp.Threading.Tasks;
using Il2CppOculus.Platform;
using Il2CppOculus.Platform.Models;
using Il2CppSLZ.Marrow.PuppetMasta;
using Microsoft.VisualBasic;
using Steamworks;
using System.Collections;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace FNPlus.Utilities
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
            PlayerIpAddress = GetPlayerIPAddress();
            Username = GetPlayerUsername();

#if DEBUG
            MelonLogger.Msg($"Player IP Address: {PlayerIpAddress}");
            MelonLogger.Msg($"Username: {Username}");
#endif
        }

        private static TaskCompletionSource<string> _usernameTaskSource = new();
        private static string GetPlayerUsername()
        {
            // Steam
            if (Path.GetFileName(UnityEngine.Application.dataPath) == "BONELAB_Steam_Windows64_Data")
            {
                if (!SteamClient.IsValid)
                    SteamClient.Init(250820, false);

                string username = SteamClient.Name;
                SteamClient.Shutdown();

                return username;
            }

            // Oculus
            if (!HelperMethods.IsAndroid())
                Core.Initialize("5088709007839657");
            else
                Core.Initialize("4215734068529064");

            var request = Users.GetLoggedInUser();

            request.OnComplete((Message<User>.Callback)OnComplete);
            void OnComplete(Message<User> msg)
            {
                Username = msg.Data.OculusID;
            }

            return "Riptide Enjoyer";
        }

        private static string GetPlayerIPAddress()
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

                return responseString;
            }
            catch (Exception e)
            {
                MelonLogger.Error($"Error when fetching IP address:");
                MelonLogger.Error(e);

                return string.Empty;
            }
        }
    }
}
