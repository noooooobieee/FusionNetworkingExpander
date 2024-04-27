using LabFusion.Network;
using LabFusion.Riptide.Utilities;
using LabFusion.Utilities;
using LabFusion.Voice;
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
        /// <returns></returns>
        public static Message CreateFusionMessage(FusionMessage message, NetworkChannel channel, ushort messageId)
        {
            Message riptideMessage = Message.Create(ConvertSendMode(channel), messageId);

            var bytes = message.ToByteArray();

            riptideMessage.AddBytes(bytes);

            return riptideMessage;
        }

        private static MessageSendMode ConvertSendMode(NetworkChannel fusionChannel)
        {
            return fusionChannel switch
            {
                NetworkChannel.Unreliable => MessageSendMode.Unreliable,
                NetworkChannel.Reliable => MessageSendMode.Reliable,
                NetworkChannel.VoiceChat => MessageSendMode.Unreliable,
                _ => MessageSendMode.Unreliable,
            };
        }
    }
}
