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
                            MapObject mymainbuild = room.MapObjects.ToArray("Builds", "mainbuild").Where((mapObject) => mapObject.OwnerIndex == room.IndexOf(username)).First();
                            Reply(sendData, (byte)GameServerAndClientResponseType.GetMap, new Dictionary<string, object>() 
                            { 
                                { "Builds", appllication.DataBase["Builds"] }, 
                                { "People", appllication.DataBase["People"] }, 
                                { "CameraPosition", new int[] { mymainbuild.x, mymainbuild.y } }, 
                                { "TileMap", room.Map.ToArray() } 
                            }, 0, "");
                        }
                    }
                    break;
                case GameServerAndClientRequestType.GetMapObjects:
                    {
                        Room room = appllication.RoomList.GetRoomWithUsername(appllication.ClientList[this]);
                        List<Dictionary<string, object>> sendMapObjectList = new List<Dictionary<string, object>>();
                        foreach (MapObject mapObject in room.MapObjects.ToArray())
                            sendMapObjectList.Add(mapObject.ObjectToData());

                        Reply(sendData, (byte)GameServerAndClientEventType.UpdateMapObjects, new Dictionary<string, object>
                        {
                            { "population", new int[] { room.MapObjects.GetAttributeSum<int>("People", "population", room.IndexOf(appllication.ClientList[this])), room.MapObjects.GetAttributeSum<int>("Builds", "population", room.IndexOf(appllication.ClientList[this])) } },
                            { "food", new int[] { room.MapObjects.GetAttributeSum<int>("People", "food", room.IndexOf(appllication.ClientList[this])), room.MapObjects.GetAttributeSum<int>("Builds", "food", room.IndexOf(appllication.ClientList[this])) } },
                            { "ore", new int[] { room.MapObjects.GetAttributeSum<int>("Builds", "ore", room.IndexOf(appllication.ClientList[this])), room.Ore[room.IndexOf(appllication.ClientList[this])] } },
                            { "MapObjects", sendMapObjectList.ToArray() }
                        }, 0, "");
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