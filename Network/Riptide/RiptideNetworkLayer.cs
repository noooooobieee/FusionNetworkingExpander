using FNPlus.Network.Riptide;
using LabFusion.Network;
using Riptide;
using Steamworks;
using System.Collections.Concurrent;
using System.Net;
using UnityEngine.Rendering;

namespace FNPlus.Network
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
        public override string GetUsername(ulong userId) => "Riptide Enjoyer";
        public override bool IsFriend(ulong userId) => false;

        public override void LogIn()
        {
            InvokeLoggedInEvent();
        }

        public override void LogOut() 
        {
            InvokeLoggedOutEvent(); 
        }

        public override void OnInitializeLayer()
        {
            RiptideThreader.StartThread();

            HookRiptideEvents();

            _voiceManager = new UnityVoiceManager();
            _voiceManager.Enable();
        }

        public override void OnDeinitializeLayer()
        {
            _voiceManager.Disable();
            _voiceManager = null;

            Disconnect();

            RiptideThreader.KillThread();

            UnhookRiptideEvents();
        }

        private void HookRiptideEvents()
        {
            // Add server hooks
            MultiplayerHooking.OnPlayerJoin += OnPlayerJoin;
            MultiplayerHooking.OnPlayerLeave += OnPlayerLeave;
            MultiplayerHooking.OnDisconnect += OnDisconnect;

            LobbyInfoManager.OnLobbyInfoChanged += OnUpdateLobby;
        }

        private void UnhookRiptideEvents()
        {
            // Remove server hooks
            MultiplayerHooking.OnPlayerJoin -= OnPlayerJoin;
            MultiplayerHooking.OnPlayerLeave -= OnPlayerLeave;
            MultiplayerHooking.OnDisconnect -= OnDisconnect;

            LobbyInfoManager.OnLobbyInfoChanged -= OnUpdateLobby;
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
        public override void OnUpdateLayer()
        {
            if (MessageQueue.Count > 0 && MessageQueue.TryDequeue(out Tuple<byte[], bool> messageTuple))
            {
                byte[] bytes = messageTuple.Item1;
                bool isServerHandled = messageTuple.Item2;

                FusionMessageHandler.ReadMessage(bytes, isServerHandled);
            }
        }

        public void OnUpdateLobby()
        {
            if (!IsClient)
                LocalPlayer.Username = Utilities.PlayerInfo.Username;
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

        public override string GetServerCode()
        {
            return ServerCode;
        }

        public override void RefreshServerCode()
        {
            ServerCode = RandomCodeGenerator.GetString(8);

            LobbyInfoManager.PushLobbyUpdate();
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
            RiptideThreader.ServerSendQueue.Enqueue(new Tuple<byte[], MessageSendMode, ushort, bool>(message.ToByteArray(), GetSendMode(channel), 0, false));
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
            RiptideThreader.ServerSendQueue.Enqueue(new Tuple<byte[], MessageSendMode, ushort, bool>(message.ToByteArray(), GetSendMode(channel), (ushort)userId, false));
        }

        public override void SendToServer(NetworkChannel channel, FusionMessage message)
        {
            if (IsServer)
                FusionMessageHandler.ReadMessage(message.ToByteArray());

            RiptideThreader.ClientSendQueue.Enqueue(new Tuple<byte[], MessageSendMode>(message.ToByteArray(), GetSendMode(channel)));
        }
    }
}
