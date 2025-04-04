using UnityEngine;

namespace FusionNetworkingPlus.Network.Riptide
{
    public class EnhancedConnectionManager : MonoBehaviour
    {
        public enum RiptideMode
        {
            None,
            Client,
            Server
        }

        public RiptideMode CurrentMode { get; private set; } = RiptideMode.None;

        void Awake()
        {
            Debug.Log("EnhancedConnectionManager: Awake.");
        }

        public void StartClient(string ip, ushort port)
        {
            // Placeholder: Riptide client init code here
            CurrentMode = RiptideMode.Client;
            Debug.Log($"[EnhancedConnectionManager] Starting client. IP: {ip}, Port: {port}");
            // e.g. RiptideClient.Connect(ip, port);
        }

        public void StartServer(ushort port)
        {
            // Placeholder: Riptide server init code here
            CurrentMode = RiptideMode.Server;
            Debug.Log($"[EnhancedConnectionManager] Starting server on port {port}");
            // e.g. RiptideServer.Start(port);
        }

        public void StopAll()
        {
            // Placeholder: Riptide shutdown code here
            if (CurrentMode == RiptideMode.Client)
            {
                Debug.Log("[EnhancedConnectionManager] Stopping client...");
                // e.g. RiptideClient.Disconnect();
            }
            else if (CurrentMode == RiptideMode.Server)
            {
                Debug.Log("[EnhancedConnectionManager] Stopping server...");
                // e.g. RiptideServer.Stop();
            }

            CurrentMode = RiptideMode.None;
        }
    }
}
