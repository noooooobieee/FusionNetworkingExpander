using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LabFusion.Senders;

using Riptide;
using Riptide.Transports;

namespace FNPlus.Network.Riptide
{
    internal class RiptideThreader
    {
        private static Thread _riptideThread;

        /// <summary>
        /// Starts the Riptide Thread and begins polling messages/actions.
        /// </summary>
        internal static void StartThread()
        {
            if (_riptideThread == null)
                _riptideThread = new Thread(RiptideThread);

            if (_riptideThread.IsAlive)
                return;

            _isThreadKilled = false;

            _riptideThread.IsBackground = true;
            _riptideThread.Start();
        }

        /// <summary>
        /// Stops the Riptide Thread from running and clears all events in the action queue.
        /// </summary>
        internal static void KillThread()
        {
            _isThreadKilled = true;
        }

        // ANYTIME these are accessed outside of the Riptide Thread, please, please, please lock it, future people. (You shouldn't do that anyway but still)
        private static Client _riptideClient;
        private static Server _riptideServer;

        internal static readonly ConcurrentQueue<Tuple<byte[], MessageSendMode>> ClientSendQueue = new();
        internal static readonly ConcurrentQueue<Tuple<byte[], MessageSendMode, ushort, bool>> ServerSendQueue = new();
        internal static readonly ConcurrentQueue<Action> ActionQueue = new();

        private static bool _isThreadKilled = false;
        private static void RiptideThread()
        {
            InitalizeRiptide();

            while (_isThreadKilled)
            {
                if (ClientSendQueue.Count > 0 && ClientSendQueue.TryDequeue(out Tuple<byte[], MessageSendMode> clientMessageTuple))
                {
                    byte[] messageData = clientMessageTuple.Item1;
                    MessageSendMode sendMode = clientMessageTuple.Item2;

                    Message message = Message.Create(sendMode, 0);
                    message.AddBytes(messageData);

                    _riptideClient.Send(message);
                }

                if (ServerSendQueue.Count > 0 && ServerSendQueue.TryDequeue(out Tuple<byte[], MessageSendMode, ushort, bool> serverMessageTuple))
                {
                    byte[] messageData = serverMessageTuple.Item1;
                    MessageSendMode sendMode = serverMessageTuple.Item2;
                    ushort id = serverMessageTuple.Item3;
                    bool isBroadcast = serverMessageTuple.Item4;

                    Message message = Message.Create(sendMode, 0);
                    message.AddBytes(messageData);

                    if (isBroadcast)
                        _riptideServer.SendToAll(message);
                    else
                        _riptideServer.Send(message, id);
                }
            }

            DeinitializeRiptide();
        }

        private static void InitalizeRiptide()
        {
            _riptideClient = new("RIPTIDE CLIENT");
            _riptideServer = new("RIPTIDE SERVER");

            _riptideClient.MessageReceived += OnClientReceives;
            _riptideServer.MessageReceived += OnServerReceives;
        }

        private static void DeinitializeRiptide()
        {
            _riptideClient = null;
            _riptideServer = null;
        }

        internal static bool IsServerRunning { get; private set; }
        internal static bool IsClientConnected { get; private set; }

        public static void StartServer()
        {
            if (_riptideServer == null || _riptideClient == null)
            {
                MelonLogger.Error("Riptide server failed to start as the Server or Client was null! This is very bad! Try restarting your game.");
                return;
            }

            ActionQueue.Enqueue(new Action(() =>
            {
                _riptideServer.Start(7777, 256);

                _riptideClient.Connected += OnConnect;
                _riptideClient.Connect("127.0.0.1:7777");

                void OnConnect(object sender, EventArgs args)
                {
                    _riptideClient.Connected -= OnConnect;

                    IsServerRunning = true;
                    IsClientConnected = true;

                    PlayerIdManager.SetLongId(_riptideClient.Id);
                    LocalPlayer.Username = Utilities.PlayerInfo.Username;

                    InternalServerHelpers.OnStartServer();
                }
            }));
        }

        public static void StopServer()
        {
            ActionQueue.Enqueue(new Action(() => 
            {
                _riptideServer.Stop();

                IsServerRunning = false;
                IsClientConnected = false;
            }));
        }

        public static void ConnectToServer(string serverCode)
        {
            ActionQueue.Enqueue(new Action(() => 
            {
                string ipString;

                if (IPAddress.TryParse(serverCode, out IPAddress ipAddress))
                    ipString = serverCode;
                else
                    ipString = IPUtils.DecodeIPAddress(serverCode);

                _riptideClient.Connected += OnConnect;
                _riptideClient.Connect($"{ipAddress}:7777");

                void OnConnect(object sender, EventArgs args)
                {
                    _riptideClient.Connected -= OnConnect;

                    IsClientConnected = true;

                    PlayerIdManager.SetLongId(_riptideClient.Id);
                    LocalPlayer.Username = Utilities.PlayerInfo.Username;

                    ConnectionSender.SendConnectionRequest();
                }
            }));
        }

        public static void Disconnect()
        {
            ActionQueue.Enqueue(new Action(() => 
            {
                _riptideClient.Disconnect();

                IsClientConnected = false;
            }));
        }

        private static void OnServerReceives(object sender, MessageReceivedEventArgs e)
        {
            RiptideNetworkLayer.MessageQueue.Enqueue(new Tuple<byte[], bool>(e.Message.GetBytes(), true));
        }

        private static void OnClientReceives(object sender, MessageReceivedEventArgs e)
        {
            RiptideNetworkLayer.MessageQueue.Enqueue(new Tuple<byte[], bool>(e.Message.GetBytes(), false));
        }
    }
}
