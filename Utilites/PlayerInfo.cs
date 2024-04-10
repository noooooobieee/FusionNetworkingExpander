using System;
using System.IO;
using System.Reflection;
using BoneLib;
using LabFusion.Representation;
using LabFusion.Utilities;
using MelonLoader;
using Oculus.Platform;
using Oculus.Platform.Models;
using RiptideNetworkLayer.Preferences;
using Steamworks;
using UnityEngine;
using UnityEngine.Networking;

namespace LabFusion.Riptide.Utilities
{
    /// <summary>
    /// Class for managing player info.
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// Last IP obtained from Ipify. Null if failed to obtain.
        /// </summary>
        internal static string PlayerIpAddress;

        #region USERNAME
        /// <summary>
        /// Inits Oculus or Steam based on platform, then sets the player's username.
        /// </summary>
        public static void InitializeUsername()
        {
            if (!HelperMethods.IsAndroid())
            {
                if (Path.GetFileName(UnityEngine.Application.dataPath) == "BONELAB_Steam_Windows64_Data")
                {
                    if (!SteamClient.IsValid)
                    {
                        SteamClient.Init(250820u, asyncCallbacks: false);
                    }

                    PlayerIdManager.SetUsername(SteamClient.Name);

                    RiptideNetworkLayer.Layer.RiptideNetworkLayer.Instance.ChangeBroadcastingData(new RiptideNetworkLayer.Layer.RiptideNetworkLayer.BeaconData(PlayerIdManager.LocalUsername, RiptidePreferences.LocalServerSettings.ServerPort.GetValue()));

                    SteamClient.Shutdown();
                }
                else
                {
                    Oculus.Platform.Core.Initialize("5088709007839657");
                    Users.GetLoggedInUser().OnComplete((Message<User>.Callback)GetLoggedInUserCallback);
                }
            }
            else
            {
                Oculus.Platform.Core.Initialize("4215734068529064");
                Users.GetLoggedInUser().OnComplete((Message<User>.Callback)GetLoggedInUserCallback);
            }
        }

        private static void GetLoggedInUserCallback(Message<User> msg)
        {
            if (!msg.IsError)
            {
                PlayerIdManager.SetUsername(msg.Data.OculusID);
                RiptideNetworkLayer.Layer.RiptideNetworkLayer.Instance.ChangeBroadcastingData(new RiptideNetworkLayer.Layer.RiptideNetworkLayer.BeaconData(PlayerIdManager.LocalUsername, RiptidePreferences.LocalServerSettings.ServerPort.GetValue()));
            }
            else
            {
                PlayerIdManager.SetUsername("Unknown");
                RiptideNetworkLayer.Layer.RiptideNetworkLayer.Instance.ChangeBroadcastingData(new RiptideNetworkLayer.Layer.RiptideNetworkLayer.BeaconData(PlayerIdManager.LocalUsername, RiptidePreferences.LocalServerSettings.ServerPort.GetValue()));

                MelonLogger.Error($"Failed to initalize Oculus username with error: {msg.error}\n{msg.error.Message}");
            }
        }
        #endregion

        #region PLAYERIP
        /// <summary>
        /// Requests the player's IP address from Ipify, then sets the PlayerIpAddress variable.
        /// </summary>
        internal static void InitializePlayerIPAddress()
        {
            string ip = "";
            try
            {
                string link = "https://api.ipify.org";
                UnityWebRequest httpWebRequest = UnityWebRequest.Get(link);
                var requestSent = httpWebRequest.SendWebRequest();

                requestSent.m_completeCallback += new System.Action<AsyncOperation>((op) =>
                {
                    ip = httpWebRequest.downloadHandler.text;
                    if (httpWebRequest.result == UnityWebRequest.Result.ConnectionError || httpWebRequest.result == UnityWebRequest.Result.ProtocolError)
                    {
                        MelonLogger.Error(httpWebRequest.error);
                        PlayerIpAddress = ip;
                    }
                    PlayerIpAddress = ip;
                });

            }
            catch (System.Exception e)
            {
                MelonLogger.Error($"Error when fetching IP address:");
                MelonLogger.Error(e);
            }
        }
        #endregion
    }
}
