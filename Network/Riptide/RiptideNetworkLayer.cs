using FNExtender.Patches;
using Il2CppTMPro;
using LabFusion.Menu;
using Riptide;

namespace FNExtender.Network
{
    public class RiptideNetworkLayer : NetworkLayer
    {
        public override string Title => "Riptide";

        private IVoiceManager _voiceManager = null;
        public override IVoiceManager VoiceManager => _voiceManager;

        public override bool IsServer => RiptideThreader.IsServerRunning;
        public override bool IsClient => RiptideThreader.IsClientConnected;

        public override bool CheckSupported() => true;
        public override bool CheckValidation() => true;

        // Riptide doesn't really have a way to add these features out of the box...
        public override string GetUsername(ulong userId) => $"Riptide Enjoyer {userId}";
        public override bool IsFriend(ulong userId) => false;

        public override void LogIn() => InvokeLoggedInEvent();
        public override void LogOut() => InvokeLoggedOutEvent();

        public override void OnInitializeLayer()
        {
            RiptideThreader.StartThread();

            HookRiptideEvents();

            _voiceManager = new UnityVoiceManager();
            _voiceManager.Enable();

            CreateRiptideUIElements();
        }

        public override void OnDeinitializeLayer()
        {
            _voiceManager.Disable();
            _voiceManager = null;

            Disconnect();

            RiptideThreader.KillThread();

            UnhookRiptideEvents();

            RemoveRiptideUIElements();
        }

        private void HookRiptideEvents()
        {
            // Add server hooks
            MultiplayerHooking.OnPlayerJoin += OnPlayerJoin;
            MultiplayerHooking.OnPlayerLeave += OnPlayerLeave;
            MultiplayerHooking.OnDisconnect += OnDisconnect;
        }

        private void UnhookRiptideEvents()
        {
            // Remove server hooks
            MultiplayerHooking.OnPlayerJoin -= OnPlayerJoin;
            MultiplayerHooking.OnPlayerLeave -= OnPlayerLeave;
            MultiplayerHooking.OnDisconnect -= OnDisconnect;
        }

        static Transform pingDisplayTransform = null;
        internal static TMP_Text PingDisplayTMP = null;
        internal static void CreateRiptideUIElements()
        {
            MenuMatchmakingPatches.HideOptions();

            if (!pingDisplayTransform)
            {
                pingDisplayTransform = GameObject.Instantiate(MenuCreator.MenuGameObject.transform.Find("page_Profile/text_Title"), MenuCreator.MenuGameObject.transform);
                PingDisplayTMP = pingDisplayTransform.GetComponent<TMP_Text>();

                pingDisplayTransform.localPosition = new Vector3(-330, 330, 0);
                PingDisplayTMP.text = "PING:";
                pingDisplayTransform.gameObject.SetActive(true);
            }
            else
                pingDisplayTransform.gameObject.SetActive(true);
        }

        internal static void RemoveRiptideUIElements()
        {
            MenuMatchmakingPatches.ShowOptions();

            if (pingDisplayTransform)
                pingDisplayTransform.gameObject.SetActive(false);
        }

        private void OnPlayerJoin(PlayerId id)
        {
            if (VoiceManager == null)
            {
                return;
            }

            if (!id.IsMe)
            {
                VoiceManager.GetSpeaker(id);
            }
        }

        private void OnPlayerLeave(PlayerId id)
        {
            if (VoiceManager == null)
            {
                return;
            }

            VoiceManager.RemoveSpeaker(id);
        }

        private void OnDisconnect()
        {
            VoiceManager.ClearManager();
        }

        internal static readonly ConcurrentQueue<Tuple<byte[], bool>> MessageQueue = new();
        internal static readonly ConcurrentQueue<Action> ActionQueue = new();
        public override void OnUpdateLayer()
        {
            for (int i = 0; i < MessageQueue.Count; i++)
            { 
                if (MessageQueue.TryDequeue(out Tuple<byte[], bool> messageTuple))
                {
                    byte[] bytes = messageTuple.Item1;
                    bool isServerHandled = messageTuple.Item2;

                    FusionMessageHandler.ReadMessage(bytes, isServerHandled);
                }
            }

            for (int i = 0; i < ActionQueue.Count; i++)
                if (ActionQueue.TryDequeue(out Action action))
                    action();
        }

        public override void StartServer()
        {
            RiptideThreader.StartServer();
        }

        public override void Disconnect(string reason = "")
        {
            RiptideThreader.Disconnect();
        }

        public string ServerCode { get; private set; } = null;
        public override string GetServerCode() => ServerCode;

        public override void RefreshServerCode()
        {
            ServerCode = IPUtils.EncodeIPAddress(Utilities.PlayerInfo.PlayerIpAddress);
            GUIUtility.systemCopyBuffer = ServerCode;

            FusionNotifier.Send(new FusionNotification()
            {
                SaveToMenu = false,
                Message = "Saved Code to Clipboard!",
                Type = NotificationType.INFORMATION,
                PopupLength = 3,
            });
        }

        public override void JoinServerByCode(string code)
        {
            RiptideThreader.ConnectToServer(code);
        }

        private static MessageSendMode GetSendMode(NetworkChannel channel)
        {
            return channel switch
            {
                NetworkChannel.Reliable => MessageSendMode.Reliable,
                NetworkChannel.Unreliable => MessageSendMode.Unreliable,
                _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null),
            };
        }

        public override void BroadcastMessage(NetworkChannel channel, FusionMessage message)
        {
            byte[] data = message.ToByteArray();
            MessageSendMode sendMode = GetSendMode(channel);

            var messageTuple = new Tuple<byte[], MessageSendMode, ushort, bool>(data, sendMode, 0, true);

            RiptideThreader.ServerSendQueue.Enqueue(messageTuple);
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
            byte[] data = message.ToByteArray();
            MessageSendMode sendMode = GetSendMode(channel);

            var messageTuple = new Tuple<byte[], MessageSendMode, ushort, bool>(data, sendMode, (ushort)userId, false);

            RiptideThreader.ServerSendQueue.Enqueue(messageTuple);
        }

        public override void SendToServer(NetworkChannel channel, FusionMessage message)
        {
            byte[] data = message.ToByteArray();
            MessageSendMode sendMode = GetSendMode(channel);

            var messageTuple = new Tuple<byte[], MessageSendMode>(data, sendMode);

            if (IsServer)
                FusionMessageHandler.ReadMessage(data);
            else
                RiptideThreader.ClientSendQueue.Enqueue(messageTuple);
        }
    }
}
