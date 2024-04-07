using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LabFusion.Representation;
using LabFusion.Riptide.Utilities;
using LabFusion.Senders;
using LabFusion.Utilities;
using RiptideNetworkLayer.Utilities;
using Riptide;
using LabFusion.Network;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using RiptideNetworkLayer.Preferences;
using System.Net;
using MelonLoader;
using Steamworks.Data;
using LabFusion.Voice;

namespace RiptideNetworkLayer.Layer
{
    public class ClientManagement
    {
        public static Client CurrentClient = new();

        public static bool IsConnected => CurrentClient.IsConnected;
        public static bool IsConnecting { get; private set; }

        /// <summary>
        /// Connects to a server, if the player is not already connecting to a server.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="port"></param>
        public static void P2PJoinServer(string code, ushort port)
        {
            if (IsConnecting)
            {
                FusionNotifier.Send(new FusionNotification()
                {
                    showTitleOnPopup = false,
                    message = $"Already connecting to a server!",
                    isMenuItem = false,
                    isPopup = true,
                    popupLength = 5f,
                    type = NotificationType.WARNING
                });

                return;
            }

            if (string.IsNullOrEmpty(code))
            {
                FusionNotifier.Send(new FusionNotification()
                {
                    title = "No Server Code",
                    showTitleOnPopup = false,
                    message = $"You have not entered a server code to join! Please click on the \"Server Code\" button to enter a server code!",
                    isMenuItem = false,
                    isPopup = true,
                    popupLength = 5f,
                    type = NotificationType.WARNING
                });

                return;
            }

            if (ServerManagement.CurrentServer.IsRunning)
                ServerManagement.CurrentServer.Stop();

            if (CurrentClient.IsConnected)
                CurrentClient.Disconnect();

            if (!code.Contains("."))
                code = IPExtensions.DecodeIpAddress(code);

            CurrentClient.Connected += OnConnect;
            CurrentClient.Connect($"{code}:{port}", 5, 0, null, false);

            IsConnecting = true;
        }
        private static void OnConnect(object sender, EventArgs e)
        {
            CurrentClient.Connected -= OnConnect;

            IsConnecting = false;

            CurrentClient.Connection.TimeoutTime = 60000;
            CurrentClient.Connection.CanQualityDisconnect = false;

            PlayerIdManager.SetLongId(CurrentClient.Id);

            ConnectionSender.SendConnectionRequest();

            InternalLayerHelpers.OnUpdateLobby();
        }

        /// <summary>
        /// Warns the client that a connection has failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnConnectionFail(object sender, ConnectionFailedEventArgs e)
        {
            IsConnecting = false;
            CurrentClient.Connected -= OnConnect;

            FusionNotifier.Send(new FusionNotification()
            {
                title = "Connection Failed",
                showTitleOnPopup = true,
                message = $"Failed to connect to server! Is the server running and the host has their port forwarded?",
                isMenuItem = false,
                isPopup = true,
                popupLength = 5f,
                type = NotificationType.ERROR
            });
        }

        internal static string DisconnectedReason = "";
        public static void OnDisconnectFromServer(object sender, DisconnectedEventArgs args)
        {
            if (DisconnectedReason != "")
                InternalServerHelpers.OnDisconnect(DisconnectedReason);
            else
                InternalServerHelpers.OnDisconnect(GetDisconnectReason(args.Reason));

            DisconnectedReason = "";
        }

        private static string GetDisconnectReason(DisconnectReason reason)
        {
            return reason switch
            {
                (DisconnectReason.ConnectionRejected) => "Connection to the server was rejected!",
                (DisconnectReason.TransportError) => "A transport error occurred!",
                (DisconnectReason.TimedOut) => "Connection timed out!",
                (DisconnectReason.Kicked) => "You were kicked from the server!",
                (DisconnectReason.ServerStopped) => "The server has stopped!",
                (DisconnectReason.Disconnected) => "You have disconnected from the server!",
                (DisconnectReason.PoorConnection) => "Connection quality was poor!",
                _ => "Idk lol",
            };
        }

        /// <summary>
        /// Calls a handler when a message is received based on its MessageId.
        /// </summary>
        public static void OnMessageReceived(object sender, MessageReceivedEventArgs message)
        {
            FusionMessageHandler.ReadMessage(VoiceCompressor.DecompressVoiceData(message.Message.GetBytes()), false);
        }
    }
}
