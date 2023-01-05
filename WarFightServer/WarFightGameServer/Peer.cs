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
                        Program.client.ClientLinker.Ask((byte)LoginServerAndGameServerRequestType.AskUserRoomNum, username, (responsesendData) =>
                        {
                            switch ((LoginServerAndGameServerResponseType)responsesendData.Code)
                            {
                                case LoginServerAndGameServerResponseType.AskUserRoomNum:
                                    {
                                        Dictionary<string, object> data = (Dictionary<string, object>)responsesendData.Parameters;
                                        bool nowsuccess = Convert.ToBoolean(responsesendData.ReturnCode) && appllication.ClientList.Add(username, this);
                                        if (nowsuccess)
                                        {
                                            appllication.RoomList.Add(data["RoomNum"].ToString(), username, (int)data["index"]);
                                        }
                                        Reply(sendData, (byte)GameServerAndClientResponseType.Login, (int)data["index"], Convert.ToInt16(nowsuccess), "");
                                    }
                                    break;
                            }
                        }, 1000);
                    }
                    break;
                case GameServerAndClientRequestType.GetMap:
                    {
                        if (!appllication.ClientList.Contains(this))
                        {
                            Reply(sendData, (byte)GameServerAndClientResponseType.Login, null, 0, "");
                        }
                        else
                        {
                            string username = appllication.ClientList[this];
                            Room room = appllication.RoomList.GetRoomWithUsername(username);
                            Reply(sendData, (byte)GameServerAndClientResponseType.GetMap, new Dictionary<string, object>() { { "builds", appllication.DataBase["builds"] }, { "people", appllication.DataBase["people"] }, { "MapObjects", room.MapObjects }, { "TileMap", room.Map.ToArray() } }, 0, "");
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