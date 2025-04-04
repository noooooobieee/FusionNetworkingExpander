using UnityEngine;

namespace FusionNetworkingPlus.Network.Riptide
{
    public class CustomServerMessageHandler : MonoBehaviour
    {
        // Example enum for message types
        public enum MessageType
        {
            ChatMessage = 1,
            PlayerUpdate = 2,
            ServerNotification = 3
        }

        /// <summary>
        /// Processes an incoming message from a client.
        /// </summary>
        public void ProcessMessage(byte[] packetData)
        {
            // Assume the first byte is the message type.
            MessageType messageType = (MessageType)packetData[0];

            switch (messageType)
            {
                case MessageType.ChatMessage:
                    HandleChatMessage(packetData);
                    break;
                case MessageType.PlayerUpdate:
                    HandlePlayerUpdate(packetData);
                    break;
                case MessageType.ServerNotification:
                    HandleServerNotification(packetData);
                    break;
                default:
                    Debug.LogWarning("Unknown message type received.")
                    break;
            }
        }

        private void HandleChatMessage(byte[] data)
        {
            // Decode and handle chat message
            string message = System.Text.Encoding.UTF8.GetString(data, 1, data.Length - 1);
            Debug.Log("[CustomServerMessageHandler] Chat message: " + message);
        }

        private void HandlePlayerUpdate(byte[] data)
        {
            // Parse player update info (this is a placeholder)
            Debug.Log("[CustomServerMessageHandler] Player update received.");
        }

        private void HandleServerNotification(byte[] data)
        {
            // Parse and log server notification
            Debug.Log("[CustomServerMessageHandler] Server notification received.");
        }
    }
}
