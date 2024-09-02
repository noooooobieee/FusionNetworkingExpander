using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FusionNetworkAddons.Network
{
    public static class RiptideExtensions
    {
        private static MessageSendMode GetSendMode(NetworkChannel channel)
        {
            return channel switch
            {
                NetworkChannel.Reliable => MessageSendMode.Reliable,
                NetworkChannel.Unreliable => MessageSendMode.Unreliable,
                _ => throw new ArgumentOutOfRangeException(nameof(channel), channel, null),
            };
        }

        public static void SendFusionMessage(this Client client, FusionMessage fusionMessage, NetworkChannel channel)
        {
            byte[] bytes = fusionMessage.ToByteArray();

            Message riptideMessage = Message.Create(GetSendMode(channel), 0);
            riptideMessage.AddBytes(bytes);

            client.Send(riptideMessage);
        }

        public static void SendFusionMessage(this Server server, ushort clientId, FusionMessage fusionMessage, NetworkChannel channel)
        {
            byte[] bytes = fusionMessage.ToByteArray();

            Message riptideMessage = Message.Create(GetSendMode(channel), 0);
            riptideMessage.AddBytes(bytes);

            server.Send(riptideMessage, clientId);
        }

        public static void SendFusionMessageToAll(this Server server, FusionMessage fusionMessage, NetworkChannel channel)
        {
            byte[] bytes = fusionMessage.ToByteArray();

            Message riptideMessage = Message.Create(GetSendMode(channel), 0);
            riptideMessage.AddBytes(bytes);

            server.SendToAll(riptideMessage);
        }
    }
}
