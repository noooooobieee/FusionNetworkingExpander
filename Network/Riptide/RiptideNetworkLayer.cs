using Il2CppWebSocketSharp;
using Riptide.Utils;

namespace FNPlus.Network
{
    public class RiptideNetworkLayer : NetworkLayer
    {
        private Client _riptideClient = new("RIPTIDE CLIENT");
        private Server _riptideServer = new("RIPTIDE SERVER");

        public override string Title => "Riptide";

        private IVoiceManager _voiceManager = null;
        public override IVoiceManager VoiceManager => _voiceManager;

        public override bool IsServer => _riptideServer.IsRunning;
        public override bool IsClient => _riptideClient.IsConnected;

        public override bool CheckSupported() => true;
        public override bool CheckValidation() => true;

        // Riptide doesn't really have a way to add these features out of the box...
        public override string GetUsername(ulong userId) => "Riptide Enjoyer";
        public override bool IsFriend(ulong userId) => false;

        public override void OnInitializeLayer()
        {
            HookRiptideEvents();

#if DEBUG
            RiptideLogger.Initialize(MelonLogger.Msg, true);
#endif

            _voiceManager = new UnityVoiceManager();
            _voiceManager.Enable();
        }

        private Page _serverInfoCategory;
        private Page _manualJoiningCategory;
        private Page _serverListingCategory;
        private void OnFillMatchmakingPage(Page page)
        {
            // Server making
            _serverInfoCategory = page.CreatePage("Server Info", Color.white);
            CreateServerInfoMenu(_serverInfoCategory);

            // Manual joining
            _manualJoiningCategory = page.CreatePage("Manual Joining", Color.white);
            CreateManualJoiningMenu(_manualJoiningCategory);

            // Server listings
            _serverListingCategory = page.CreatePage("Server Listings", Color.white);
            ServerListing.CreateServerListingMenu(this, _serverListingCategory, $"Server Code\nor\nServer IP", JoinServer);
        }

        private FunctionElement _serverCreationElement;
        private void CreateServerInfoMenu(Page serverInfoCategory)
        {
            _serverCreationElement = serverInfoCategory.CreateFunction("Start Server", Color.white, OnClickServerCreation);

            serverInfoCategory.CreateFunction("Show Server Code", Color.white, () => FusionNotifier.Send(new FusionNotification()
            {
                showTitleOnPopup = false,
                message = $"Server Code: {IPExtensions.EncodeIpAddress(PlayerInfo.PlayerIpAddress)}",
            }));
            serverInfoCategory.CreateFunction("Copy Server Code", Color.white, () => GUIUtility.systemCopyBuffer = IPExtensions.EncodeIpAddress(PlayerInfo.PlayerIpAddress));

            BoneMenuCreator.PopulateServerInfo(serverInfoCategory);
        }

        private void OnClickServerCreation()
        {
            if (_riptideClient.IsConnecting)
                return;

            if (IsClient)
                Disconnect();
            else
                StartServer();
        }

        private StringElement _serverCodeElement;
        private string _serverCode = "";
        private void CreateManualJoiningMenu(Page manualJoiningCategory)
        {
            manualJoiningCategory.CreateFunction("Join Server", Color.white, OnClickJoinServer);

            _serverCodeElement = manualJoiningCategory.CreateString($"Server Code\nor\nServer IP", Color.white, _serverCode, (value) => _serverCode = value);
        }

        private void OnClickJoinServer()
        {
            if (_serverCode.IsNullOrEmpty())
            {
                FusionNotifier.Send(new FusionNotification()
                {
                    showTitleOnPopup = false,
                    type = NotificationType.WARNING,
                    message = "Please enter a server code or IP address to join.",
                });
            }

            if (IsClient)
                Disconnect();

            JoinServer(_serverCode);
        }

        private void HookRiptideEvents()
        {
            // Add server hooks
            MultiplayerHooking.OnMainSceneInitialized += OnUpdateLobby;
            GamemodeManager.OnGamemodeChanged += (gameMode) => OnUpdateLobby();
            MultiplayerHooking.OnPlayerJoin += OnPlayerJoin;
            MultiplayerHooking.OnPlayerLeave += OnPlayerLeave;
            MultiplayerHooking.OnServerSettingsChanged += OnUpdateLobby;
            MultiplayerHooking.OnDisconnect += OnDisconnect;

            // Add BoneMenu hooks
            MatchmakingCreator.OnFillMatchmakingPage += OnFillMatchmakingPage;
        }

        private void UnhookRiptideEvents()
        {
            // Add server hooks
            MultiplayerHooking.OnMainSceneInitialized -= OnUpdateLobby;
            GamemodeManager.OnGamemodeChanged -= (gameMode) => OnUpdateLobby();
            MultiplayerHooking.OnPlayerJoin -= OnPlayerJoin;
            MultiplayerHooking.OnPlayerLeave -= OnPlayerLeave;
            MultiplayerHooking.OnServerSettingsChanged -= OnUpdateLobby;
            MultiplayerHooking.OnDisconnect -= OnDisconnect;

            // Add BoneMenu hooks
            MatchmakingCreator.OnFillMatchmakingPage -= OnFillMatchmakingPage;
        }

        private void OnPlayerJoin(PlayerId id)
        {
            if (!id.IsOwner)
                VoiceManager.GetSpeaker(id);

            OnUpdateLobby();
        }

        private void OnPlayerLeave(PlayerId id)
        {
            VoiceManager.RemoveSpeaker(id);

            OnUpdateLobby();
        }

        private void OnDisconnect()
        {
            VoiceManager.ClearManager();
        }

        public override void OnCleanupLayer()
        {
            UnhookRiptideEvents();
        }

        public override void OnUpdateLayer()
        {
            _riptideServer.Update();
            _riptideClient.Update();
        }

        public override void OnUpdateLobby()
        {
            if (!IsClient)
                PlayerIdManager.SetUsername(PlayerInfo.Username);

            UpdateServerCreationText();
        }

        private void UpdateServerCreationText()
        {
            if (IsServer)
            {
                _serverCreationElement.ElementColor = Color.red;
                _serverCreationElement.ElementName = "Stop Server";
            }
            else if (IsClient)
            {
                _serverCreationElement.ElementColor = Color.red;
                _serverCreationElement.ElementName = "Disconnect";
            } else
            {
                _serverCreationElement.ElementColor = Color.white;
                _serverCreationElement.ElementName = "Start Server";
            }
        }

        public override void StartServer()
        {
            _riptideServer.Start(7777, 256);

            _riptideClient.Connected += OnConnect;
            _riptideClient.Connect("127.0.0.1:7777");

            void OnConnect(object sender, EventArgs args)
            {
                _riptideClient.Connected -= OnConnect;

                PlayerIdManager.SetLongId(_riptideClient.Id);
                InternalServerHelpers.OnStartServer();

                OnUpdateLobby();
            }
        }

        public void JoinServer(string serverCode)
        {
            string ipAddress;

            if (!serverCode.Contains('.'))
                ipAddress = IPExtensions.DecodeIpAddress(serverCode);
            else
                ipAddress = serverCode;

            _riptideClient.Connected += OnConnect;
            _riptideClient.Connect($"{ipAddress}:7777");

            void OnConnect(object sender, EventArgs args)
            {
                _riptideClient.Connected -= OnConnect;

                PlayerIdManager.SetLongId(_riptideClient.Id);
                InternalServerHelpers.OnJoinServer();
                OnUpdateLobby();
            }
        }

        public override void Disconnect(string reason = "")
        {
            _riptideClient.Disconnect();
            _riptideServer.Stop();

            InternalServerHelpers.OnDisconnect(reason);

            OnUpdateLobby();
        }

        public override void BroadcastMessage(NetworkChannel channel, FusionMessage message)
        {
            _riptideServer.SendFusionMessageToAll(message, channel);
        }

        public override void SendFromServer(byte userId, NetworkChannel channel, FusionMessage message)
        {
            var id = PlayerIdManager.GetPlayerId(userId);

            if (id != null)
            {
                SendFromServer(id.LongId, channel, message);
            }
        }

        public override void SendFromServer(ulong userId, NetworkChannel channel, FusionMessage message)
        {
            _riptideServer.SendFusionMessage((ushort)userId, message, channel);
        }

        public override void SendToServer(NetworkChannel channel, FusionMessage message)
        {
            if (IsServer)
                FusionMessageHandler.ReadMessage(message.ToByteArray());

            _riptideClient.SendFusionMessage(message, channel);
        }

        [MessageHandler(0)]
        public static void HandleServerMessage(ushort clientId, Message riptideMessage)
        {
            FusionMessageHandler.ReadMessage(riptideMessage.GetBytes());
        }

        [MessageHandler(0)]
        public static void HandleClientMessage(Message riptideMessage)
        {
            FusionMessageHandler.ReadMessage(riptideMessage.GetBytes());
        }
    }
}
