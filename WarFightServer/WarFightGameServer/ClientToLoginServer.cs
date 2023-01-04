using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using JimmikerNetwork;
using JimmikerNetwork.Client;
using PacketType;

namespace WarFightGameServer
{
    public class ClientToLoginServer : ClientListen
    {
        public ClientLinker ClientLinker { get; private set; }

        public ClientToLoginServer(ProtocolType protocolType)
        {
            ClientLinker = new ClientLinker(this, protocolType);
        }

        public bool Connect(string host, int port)
        {
            bool on = ClientLinker.Connect(host, port);
            ClientLinker.RunUpdateThread();
            return on;
        }

        public void DebugReturn(string message)
        {
            Console.Error.WriteLine(message);
        }

        public void OnEvent(SendData sendData)
        {
            switch((LoginServerAndGameServerEventType)sendData.Code)
            {
                case LoginServerAndGameServerEventType.NewGame:
                    {
                        Dictionary<string, object> data = (Dictionary<string, object>)sendData.Parameters;
                        Program.appllication.RoomList.Add(data["RoomNum"].ToString(), (int)data["PlayerCount"]);
                    }
                    break;
                case LoginServerAndGameServerEventType.EndGame:
                    {
                        Program.appllication.RoomList.Remove(sendData.Parameters.ToString());
                    }
                    break;
            }
        }

        public void OnOperationResponse(SendData sendData)
        {
            switch ((LoginServerAndGameServerResponseType)sendData.Code)
            {
                case LoginServerAndGameServerResponseType.Login:
                    {
                        if(!Convert.ToBoolean(sendData.ReturnCode))
                        {
                            DebugReturn("Login fail");
                            Environment.Exit(1);
                        }
                    }
                    break;
            }
        }

        public void OnStatusChanged(LinkCobe connect)
        {
            switch(connect)
            {
                case LinkCobe.Connect:
                    {
                        ClientLinker.Ask((byte)LoginServerClientType.GameServer, null);
                        ClientLinker.Ask((byte)LoginServerAndGameServerRequestType.Login, Program.appllication.PublicEndPoint.ToString());
                    }
                    break;
                case LinkCobe.Failed:
                    {
                        Console.Error.WriteLine("Connect Failed");
                        Environment.Exit(1);
                    }
                    break;
                case LinkCobe.Lost:
                    {
                        Console.Error.WriteLine("Disconnect");
                        Environment.Exit(1);
                    }
                    break;
            }
        }

        public PeerForP2PBase P2PAddPeer(object _peer, object publicIP, INetClient client, bool NAT)
        {
            throw new NotImplementedException();
        }
    }
}