using LabFusion.Network;
using LabFusion.Riptide.Utilities;
using LabFusion.Utilities;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace RiptideNetworkLayer.Layer
{
    public class RiptideFusionMessage
    {
        /// <summary>
        /// Creates a Riptide message from a Fusion message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public static Message CreateFusionMessage(FusionMessage message, NetworkChannel channel, ushort messageId = 0)
        {
            return Message.Create(ConvertSendMode(channel), messageId).AddBytes(message.ToByteArray());
        }

        internal static void HandleClientFusionMessage(Message message)
        {
            FusionMessageHandler.ReadMessage(message.GetBytes());
        }

        internal static void HandleServerFusionMessage(Message message)
        {
            FusionMessageHandler.ReadMessage(message.GetBytes(), true);
        }

        private static MessageSendMode ConvertSendMode(NetworkChannel fusionChannel)
        {
            switch (fusionChannel)
            {
                case NetworkChannel.Unreliable:
                    return MessageSendMode.Unreliable;
                case NetworkChannel.Reliable:
                    return MessageSendMode.Reliable;
                case NetworkChannel.VoiceChat:
                    return MessageSendMode.Unreliable;
                default: 
                    return MessageSendMode.Unreliable;
            }
        }
    }
}
