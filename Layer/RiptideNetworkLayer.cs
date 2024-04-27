using LabFusion.Riptide.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riptide;
using BoneLib.BoneMenu.Elements;
using LabFusion.BoneMenu;
using UnityEngine;
using BoneLib;
using LabFusion.Utilities;
using MelonLoader;
using RiptideNetworkLayer.BoneMenu;
using LabFusion.Network;
using LabFusion.Representation;
using RiptideNetworkLayer.Preferences;
using Riptide.Utils;
using System.Reflection;
using LabFusion.Senders;
using LabFusion.SDK.Gamemodes;
using LabFusion.Grabbables;
using SLZ.Interaction;
using SLZ.Marrow.Data;
using SLZ.Props;
using BoneLib.BoneMenu;
using System.Collections;
using LabFusion.Voice;
using RiptideNetworkLayer.Layer;
using static RiptideNetworkLayer.Layer.ClientManagement;
using static RiptideNetworkLayer.Layer.ServerManagement;
using LabFusion.Voice.Unity;
using RiptideNetworkLayer.Utilities;
using Steamworks;
using JetBrains.Annotations;
using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine.InputSystem.Utilities;

namespace RiptideNetworkLayer.Layer
{
    public class RiptideNetworkLayer : NetworkLayer
    {
        internal static RiptideNetworkLayer Instance;

        public static readonly string TideFusionPath = $"{MelonUtils.UserDataDirectory}/TideFusion";

        public override bool IsClient => CheckIsClient();
        public override bool IsServer => CheckIsServer();

        private IVoiceManager _voiceManager = null;
        public override IVoiceManager VoiceManager => _voiceManager;

        public override string Title => "Riptide";

        public override bool CheckSupported() => true;

        public override bool CheckValidation() => true;

        public override bool IsFriend(ulong userId) => false;

        public override void OnInitializeLayer()
        {
            Instance = this;

            if (!System.IO.Directory.Exists(TideFusionPath))
                System.IO.Directory.CreateDirectory(TideFusionPath);

#if DEBUG
            RiptideLogger.Initialize(MelonLogger.Msg, true);
#endif

            Message.MaxPayloadSize = 1024 * 1024;

            HookRiptideEvents();

            _voiceManager = new UnityVoiceManager();
            _voiceManager.Enable();

            PlayerInfo.InitializeUsername();
            PlayerInfo.InitializePlayerIPAddress();

#if DEBUG
            MelonLogger.Msg("Initialized Riptide layer");
#endif
        }

        private UdpClient client;
        internal void InitLANDiscovery()
        {
            int startPort = 9743;
            int endPort = 9999; // You can adjust the range as needed

            for (int port = startPort; port <= endPort; port++)
            {
                try
                {
                    client = new(port)
                    {
                        EnableBroadcast = true
                    };
                    break;
                }
                catch (SocketException)
                {
                }
            }

            RiptideNetworkLayer.Instance.ChangeBroadcastingData(new RiptideNetworkLayer.LANData("RIPTIDE_UNKNOWN_SERVER_RANDOMSHITASDADWDASDW", RiptidePreferences.LocalServerSettings.ServerPort.GetValue(), false));

            Thread broadcastThread = new(() =>
            {
                UdpClient udpClient = new()
                {
                    EnableBroadcast = true
                };

                while (true)
                {
                    for (int i = 9743; i <= 9999; i++)
                    {
                            if (broadcastingBytes != null)
                                udpClient.Send(broadcastingBytes, broadcastingBytes.Length, new IPEndPoint(IPAddress.Broadcast, i));
                    }

                    Thread.Sleep(1000);
                }
            });


            broadcastThread.Start();
        }

        private readonly object lockObject = new();
        private byte[] broadcastingBytes = null;

        internal void ChangeBroadcastingData(LANData newValue)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(newValue);

            lock (lockObject)
            {
                broadcastingBytes = Encoding.ASCII.GetBytes(json);
            }
        }

        private void HookRiptideEvents()
        {
            // Riptide Hooks
            CurrentServer.ClientDisconnected += OnClientDisconnect;
            CurrentServer.ClientConnected += OnClientConnect;

            CurrentClient.Disconnected += OnDisconnectFromServer;
            CurrentClient.ConnectionFailed += OnConnectionFail;

            CurrentServer.MessageReceived += ServerManagement.OnMessageReceived;
            CurrentClient.MessageReceived += ClientManagement.OnMessageReceived;

            // Add Server Hooks
            MultiplayerHooking.OnPlayerJoin += OnPlayerJoin;
            MultiplayerHooking.OnPlayerLeave += OnPlayerLeave;
            MultiplayerHooking.OnDisconnect += OnDisconnect;
        }

        private void UnHookRiptideEvents()
        {
            // Riptide Hooks
            CurrentServer.ClientDisconnected -= OnClientDisconnect;
            CurrentServer.ClientConnected -= OnClientConnect;

            CurrentClient.Disconnected -= OnDisconnectFromServer;
            CurrentClient.ConnectionFailed -= OnConnectionFail;

            CurrentServer.MessageReceived -= ServerManagement.OnMessageReceived;
            CurrentClient.MessageReceived -= ClientManagement.OnMessageReceived;

            // Add Server Hooks
            MultiplayerHooking.OnPlayerJoin -= OnPlayerJoin;
            MultiplayerHooking.OnPlayerLeave -= OnPlayerLeave;
            MultiplayerHooking.OnDisconnect -= OnDisconnect;
        }

        private void OnPlayerJoin(PlayerId id)
        {
            if (!id.IsSelf)
                _voiceManager.GetSpeaker(id);

            OnUpdateLobby();
        }

        private void OnPlayerLeave(PlayerId id)
        {
            _voiceManager.RemoveSpeaker(id);

            OnUpdateLobby();
        }

        private void OnDisconnect()
        {
            _voiceManager.ClearManager();

            OnUpdateLobby();
        }

        public override void OnSetupBoneMenu(MenuCategory category)
        {
            // Create the basic options
            CreateMatchmakingMenu(category);
            BoneMenuCreator.CreateGamemodesMenu(category);
            BoneMenuCreator.CreateSettingsMenu(category);
            BoneMenuCreator.CreateNotificationsMenu(category);
        }

        private FunctionElement _createServerElement;
        private Keyboard _serverPortKeyboard;
        private void CreateMatchmakingMenu(MenuCategory category)
        {
            // Root category
            var matchmaking = category.CreateCategory("Matchmaking", Color.red);

            // Server info
            var serverInfo = matchmaking.CreateCategory("Server Info", Color.white);

            // Server Starting/Settings
            _createServerElement = serverInfo.CreateFunctionElement("Start Server", Color.green, () => OnClickStartServer());

            // Server settings
            var p2pServerSettingsMenu = serverInfo.CreateCategory("P2P Server Settings", Color.cyan);
            _serverPortKeyboard = Keyboard.CreateKeyboard(p2pServerSettingsMenu, $"Server Port:\n{RiptidePreferences.LocalServerSettings.ServerPort.GetValue()}", (port) => OnChangeServerPort(port));

            // Server code
            serverInfo.CreateFunctionElement("Display Server Code", Color.white, () => OnDislayServerCode());

            // Server Info
            BoneMenuCreator.PopulateServerInfo(serverInfo);

            // P2P Server
            CreateManualJoiningMenu(matchmaking);

            // Server Listings
            ServerListingCategory.CreateServerListingCategory(matchmaking);

            // LAN Discovery
            CreateLanDiscoveryMenu(matchmaking);
        }

        private void CreateLanDiscoveryMenu(MenuCategory category)
        {
            var lanDiscovery = category.CreateCategory("LAN Discovery", Color.white);

            // Start LAN Discovery
            lanDiscovery.CreateFunctionElement("Search for LAN Servers", Color.green, () => StartLanDiscovery(lanDiscovery));
        }

        private void StartLanDiscovery(MenuCategory category)
        {
            category.Elements.Clear();
            category.CreateFunctionElement("Search for LAN Servers", Color.green, () => StartLanDiscovery(category));

            List<LANData> obtainedUsers = [];

            bool hasMessages = false;
            do
            {
                if (client.Available > 0)
                {
                    IPEndPoint endPoint = new(IPAddress.Broadcast, 0);
                    byte[] bytes = client.Receive(ref endPoint);

                    string message = Encoding.ASCII.GetString(bytes);

                    LANData data = Newtonsoft.Json.JsonConvert.DeserializeObject<LANData>(message);
                    data.IpAddress = endPoint.Address.ToString();

                    // Handle message
                    if (!obtainedUsers.Exists(x => x.Username == data.Username) && data.Username != "RIPTIDE_UNKNOWN_SERVER_RANDOMSHITASDADWDASDW")
                    {
                        obtainedUsers.Add(data);
                    } else if (obtainedUsers.Exists(x => x.Username == data.Username) && data.Username != "RIPTIDE_UNKNOWN_SERVER_RANDOMSHITASDADWDASDW")
                    {
                        obtainedUsers.Remove(obtainedUsers.Where(x => x.Username == data.Username).First());
                        obtainedUsers.Add(data);
                    }
                }
                else
                {
                    hasMessages = true;
                }
            }
            while (!hasMessages);

            foreach (var user in obtainedUsers)
            {
#if DEBUG
                MelonLogger.Msg($"Obtained server with username {user.Username}, IP {user.IpAddress} and port {user.Port}");
#endif
                if (user.Username != PlayerIdManager.LocalUsername && user.IsOpen)
                    category.CreateFunctionElement($"Join {user.Username}", Color.white, () => P2PJoinServer(user.IpAddress, user.Port));
            }
        }


        public class LANData(string username, ushort port, bool isOpen)
        {
            public string Username = username;
            public ushort Port = port;
            public bool IsOpen = isOpen;
            public string IpAddress { get; set; }
        }

        private void OnDislayServerCode()
        {
            FusionNotifier.Send(new FusionNotification()
            {
                isMenuItem = false,
                isPopup = true,
                showTitleOnPopup = true,
                popupLength = 20f,
                title = "Server Code",
                message = $"{IPExtensions.EncodeIpAddress(PlayerInfo.PlayerIpAddress)}",
                type = NotificationType.INFORMATION,
            });
        }

        private void OnClickStartServer()
        {
            // Is a server already running? Disconnect
            if (IsClient)
            {
                Disconnect();
            }
            // Otherwise, start a server
            else
            {
                ServerManagement.StartServer();
            }
        }

        private void OnChangeServerPort(string port)
        {
            if (!ushort.TryParse(port, out ushort result) || result <= 1024 || result >= 65535)
            {
                FusionNotifier.Send(new FusionNotification()
                {
                    isMenuItem = false,
                    isPopup = true,
                    message = "Entered a Port which is incorrect!" +
                              "\nMake SURE to only input numbers and that the port range is between 1024 and 65535",
                    type = NotificationType.ERROR,
                });

                return;
            }

            RiptidePreferences.LocalServerSettings.ServerPort.SetValue(result);
            _serverPortKeyboard.Category.SetName($"Server Port:\n{RiptidePreferences.LocalServerSettings.ServerPort.GetValue()}");
        }

        private MenuCategory _targetP2PCodeCategory;
        private MenuCategory _targetP2PPortCategory;
        private string _serverCodeToJoin;
        private ushort _serverPortToJoin = 7777;
        private void CreateManualJoiningMenu(MenuCategory category)
        {
            var manualJoining = category.CreateCategory("Manual Joining", Color.white);
            manualJoining.CreateFunctionElement("Join Server", Color.green, () => P2PJoinServer(_serverCodeToJoin, _serverPortToJoin));
            _targetP2PCodeCategory = Keyboard.CreateKeyboard(manualJoining, "Server Code:", (code) => OnChangeJoinCode(code)).Category;
            _targetP2PPortCategory = Keyboard.CreateKeyboard(manualJoining, $"Server Port:\n{_serverPortToJoin}", (port) => OnChangeJoinPort(port)).Category;
        }

        private void OnChangeJoinCode(string code)
        {
            _serverCodeToJoin = code;
            _targetP2PCodeCategory.SetName($"Server Code:\n{_serverCodeToJoin}");
        }

        private void OnChangeJoinPort(string port)
        {
            if (!ushort.TryParse(port, out ushort result) || result <= 1024 || result >= 65535)
            {
                FusionNotifier.Send(new FusionNotification()
                {
                    isMenuItem = false,
                    isPopup = true,
                    message = "Entered a Port which is incorrect!" +
                              "\nMake SURE to only input numbers and that the port range is between 1024 and 65535",
                    type = NotificationType.ERROR,
                });

                return;
            }

            _serverPortToJoin = result;
            _targetP2PPortCategory.SetName($"Edit Port:\n" +
                                           $"{result}");
        }

        private bool CheckIsClient()
        {
            if (CurrentClient == null)
                return false;

            return CurrentClient.IsConnected;
        }

        private bool CheckIsServer()
        {
            if (CurrentServer == null)
                return false;

            return CurrentServer.IsRunning;
        }

        public override void OnUpdateLayer()
        {
            CurrentServer.Update();
            CurrentClient.Update();
        }

        public override void OnUpdateLobby()
        {
            // Update bonemenu items
            OnUpdateCreateServerText();
        }

        private void OnUpdateCreateServerText()
        {
            try
            {
                if (_createServerElement == null)
                    return;

                if (IsClient && !IsServer)
                {
                    _createServerElement.SetName("Disconnect");
                    _createServerElement.SetColor(Color.red);
                }
                else if (IsServer)
                {
                    _createServerElement.SetName("Stop Server");
                    _createServerElement.SetColor(Color.red);
                }
                else if (!IsClient)
                {
                    _createServerElement.SetName("Start Server");
                    _createServerElement.SetColor(Color.green);
                }
            } catch (Exception ex)
            {
                MelonLogger.Error($"Failed to update Create Server text with reason: {ex.Message}");
            }
        }

        public override void StartServer() => ServerManagement.StartServer();

        // Am I insane for trying to implement three different networking systems into one layer? Maybe! Do I care? Nop.
        #region Fusion Messaging
        #region BROADCAST
        public override void BroadcastMessage(NetworkChannel channel, FusionMessage message)
        {
            if (IsServer)
            {
                CurrentServer.SendToAll(RiptideFusionMessage.CreateFusionMessage(message, channel, 0));
            }
            else
            {
                CurrentClient.Send(RiptideFusionMessage.CreateFusionMessage(message, channel, 0));
            }
        }
        #endregion
        #region SENDTOSERVER
        public override void SendToServer(NetworkChannel channel, FusionMessage message)
        {
            CurrentClient.Send(RiptideFusionMessage.CreateFusionMessage(message, channel,  0));
        }
        #endregion
        #region SENDFROMSERVER
        public override void SendFromServer(byte userId, NetworkChannel channel, FusionMessage message)
        {
            PlayerId playerId = PlayerIdManager.GetPlayerId(userId);
            if (playerId != null)
            {
                SendFromServer(playerId.LongId, channel, message);
            }
        }

        public override void SendFromServer(ulong userId, NetworkChannel channel, FusionMessage message)
        {
            if (IsServer)
            {
                if (userId == PlayerIdManager.LocalLongId)
                {
                    FusionMessageHandler.ReadMessage(message.ToByteArray());
                }
                else if (CurrentServer.TryGetClient((ushort)userId, out var client))
                {
                    CurrentServer.Send(RiptideFusionMessage.CreateFusionMessage(message, channel, 0), client);
                }
            }
        }
        #endregion
        #endregion

        public override void Disconnect(string reason = "")
        {
            DisconnectedReason = reason;

            if (!IsServer && !IsClient)
                return;

            if (IsClient)
                CurrentClient.Disconnect();

            CurrentServer.Stop();

            OnUpdateLobby();
        }

        public override void OnCleanupLayer()
        {
            Disconnect();

            UnHookRiptideEvents();

            _voiceManager.ClearManager();
            _voiceManager.Disable();
        }
    }
}
