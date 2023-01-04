using JimmikerNetwork;
using JimmikerNetwork.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WarFightLoginServer
{
    public class Appllication : AppllicationBase
    {
        public ClientList ClientList { get; private set; }
        public GameServerList GameServerList { get; private set; }
        public RoomList RoomList { get; private set; }

        public event Action<MessageType, string> GetMessage;
        public Appllication(int port, ProtocolType protocol) : base(IPAddress.IPv6Any, port, protocol)
        {
        }

        protected override void Setup()
        {
            ClientList = new ClientList();
            GameServerList = new GameServerList();
            RoomList = new RoomList(2);

            RunUpdateThread();
        }

        protected override PeerBase AddPeerBase(object _peer, INetServer server)
        {
            return new Peer(_peer, server, this);
        }

        protected override void TearDown()
        {

        }

        protected override void DebugReturn(MessageType messageType, string msg)
        {
            GetMessage?.Invoke(messageType, msg);
        }
    }
}