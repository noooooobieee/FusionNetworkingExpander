using UnityEngine;
using System.Collections;

namespace FusionNetworkingPlus.Network.Riptide
{
    public class AutoReconnect : MonoBehaviour
    {
        public string serverIP = "127.0.0.1";
        public ushort serverPort = 7777;
        public float reconnectDelay = 5.0f;

        private bool isConnected = false;

        void Start()
        {
            // Assume an event-based system that sets isConnected when connected/disconnected
            StartCoroutine(CheckConnection());
        }

        IEnumerator CheckConnection()
        {
            while (true)
            {
                if (!isConnected)
                {
                    Debug.Log("[AutoReconnect] Disconnected. Attempting to reconnect in " + reconnectDelay + " seconds...");
                    yield return new WaitForSeconds(reconnectDelay);
                    AttemptReconnect();
                }
                yield return null;
            }
        }

        private void AttemptReconnect()
        {
            Debug.Log("[AutoReconnect] Attempting to reconnect to " + serverIP + ":" + serverPort);
            // Replace with actual connection code, e.g.:
            // RiptideClient.Connect(serverIP, serverPort);
        }

        // Call this method when connection is established
        public void OnConnected()
        {
            isConnected = true;
            Debug.Log("[AutoReconnect] Connected successfully.");
        }

        // Call this method when connection is lost
        public void OnDisconnected()
        {
            isConnected = false;
            Debug.Log("[AutoReconnect] Connection lost.");
        }
    }
}
