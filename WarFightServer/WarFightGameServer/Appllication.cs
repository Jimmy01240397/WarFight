﻿using Chuon;
using JimmikerNetwork;
using JimmikerNetwork.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WarFightGameServer
{
    public class Appllication : AppllicationBase
    {
        public event Action<MessageType, string> GetMessage;

        public ClientList ClientList { get; private set; }
        public RoomList RoomList { get; private set; }

        public Dictionary<string, object> DataBase { get; private set; }
        public Dictionary<string, Action<bool, string, string, int>> AskUserRoomNumDelegates { get; private set; }

        public IPEndPoint PublicEndPoint { get; private set; }

        public Appllication(int port, ProtocolType protocol) : base(IPAddress.IPv6Any, port, protocol)
        {
            using (WebClient webClient = new WebClient())
            {
                string ip = webClient.DownloadString("http://ifconfig.me");
                PublicEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            }
        }

        protected override void Setup()
        {
            ClientList = new ClientList();
            RoomList = new RoomList();
            AskUserRoomNumDelegates = new Dictionary<string, Action<bool, string, string, int>>();
            ChuonBinary chuonBinary = new ChuonBinary(File.ReadAllBytes("database.dat"));

            Type type = chuonBinary.ToObject().GetType();
            DataBase = (Dictionary<string, object>)chuonBinary.ToObject();
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