using JimmikerNetwork;
using JimmikerNetwork.Server;
using PacketType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WarFightGameServer
{
    public class Peer : PeerBase
    {
        Appllication appllication;

        public Peer(object peer, INetServer server, Appllication _appllication) : base(peer, server)
        {
            appllication = _appllication;
        }
        public override void OnOperationRequest(SendData sendData)
        {
            switch ((GameServerAndClientRequestType)sendData.Code)
            {
                case GameServerAndClientRequestType.Login:
                    {
                        string username = sendData.Parameters.ToString();
                        appllication.AskUserRoomNumDelegates.Add(username, (success, name, RoomNUM, index) =>
                        {
                            bool nowsuccess = success && appllication.ClientList.Add(username, this);
                            if (nowsuccess)
                            {
                                appllication.RoomList.Add(RoomNUM, username, index);
                            }
                            Reply((byte)GameServerAndClientResponseType.Login, index, Convert.ToInt16(nowsuccess), "");
                        });
                        Program.client.ClientLinker.Ask((byte)LoginServerAndGameServerRequestType.AskUserRoomNum, username);
                    }
                    break;
                case GameServerAndClientRequestType.GetMap:
                    {
                        if (!appllication.ClientList.Contains(this))
                        {
                            Reply((byte)GameServerAndClientResponseType.Login, null, 0, "");
                        }
                        else
                        {
                            string username = appllication.ClientList[this];
                            Room room = appllication.RoomList.GetRoomWithUsername(username);
                            Reply((byte)GameServerAndClientResponseType.GetMap, new Dictionary<string, object>() { { "MapObjects", room.MapObjects }, { "TileMap", room.Map.ToArray() } }, 0, "");
                        }
                    }
                    break;
            }
        }

        public override void OnDisconnect()
        {
            appllication.ClientList.Remove(this);
        }
    }
}