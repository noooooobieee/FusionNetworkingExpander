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
                if (ClientManagement.IsConnecting)
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
                }
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

            void OnConnect(object sender, EventArgs e)
            {
                CurrentClient.Connected -= OnConnect;

                IsConnecting = false;

                CurrentClient.TimeoutTime = 15000;
                CurrentClient.Connection.CanQualityDisconnect = false;

                PlayerIdManager.SetLongId(CurrentClient.Id);

                ConnectionSender.SendConnectionRequest();

                InternalLayerHelpers.OnUpdateLobby();
            }

            CurrentClient.Connected += OnConnect;
            CurrentClient.Connect($"{code}:{port}", 5, 0, null, false);

            IsConnecting = true;
        }

        /// <summary>
        /// Warns the client that a connection has failed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnConnectionFail(object sender, ConnectionFailedEventArgs e)
        {
            IsConnecting = false;

            FusionNotifier.Send(new FusionNotification()
            {
                title = "Connection Failed",
                showTitleOnPopup = true,
                message = $"Failed to connect to server! Is the server running?",
                isMenuItem = false,
                isPopup = true,
                popupLength = 5f,
                type = NotificationType.ERROR
            });
        }

        public static void OnDisconnectFromServer(object sender, DisconnectedEventArgs args)
        {
            InternalServerHelpers.OnDisconnect();
        }

        /// <summary>
        /// Calls a handler when a message is received based on its MessageId.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        public static void OnMessageReceived(object obj, MessageReceivedEventArgs args)
        {
            switch (args.MessageId)
            {
                case MessageTypes.FusionMessage:
                    {
                        RiptideFusionMessage.HandleClientFusionMessage(args.Message);
                        break;
                    }
            }
        }
    }
}
