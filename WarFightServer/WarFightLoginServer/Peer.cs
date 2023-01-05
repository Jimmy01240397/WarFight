using JimmikerNetwork;
using JimmikerNetwork.Server;
using PacketType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WarFightLoginServer
{
    public class Peer : PeerBase
    {
        Appllication appllication;
        LoginServerClientType clientType = LoginServerClientType.None;

        public Peer(object peer, INetServer server, Appllication _appllication) : base(peer, server)
        {
            appllication = _appllication;
        }
        public override void OnOperationRequest(SendData sendData)
        {
            switch(clientType)
            {
                case LoginServerClientType.None:
                    {
                        clientType = (LoginServerClientType)sendData.Code;
                    }
                    break;
                case LoginServerClientType.Client:
                    {
                        switch ((LoginServerAndClientRequestType)sendData.Code)
                        {
                            case LoginServerAndClientRequestType.Login:
                                {
                                    string username = sendData.Parameters.ToString();
                                    bool success = appllication.ClientList.Add(username, this);
                                    Reply(sendData, (byte)LoginServerAndClientResponseType.Login, null, Convert.ToInt16(success), "");
                                    if (success && appllication.RoomList.ContainsUsername(username))
                                    {
                                        Room room = appllication.RoomList.GetRoomWithUsername(username);
                                        string[] users = new string[room.PlayerCount];
                                        room.CopyTo(users, 0);
                                        room.SendRoomUpdate();
                                        if (appllication.GameServerList.Contains(room))
                                            Tell((byte)LoginServerAndClientEventType.Start, appllication.GameServerList[room].IPEndPoint.ToString());
                                    }
                                }
                                break;
                            case LoginServerAndClientRequestType.Logout:
                                {
                                    Logout();
                                }
                                break;
                            case LoginServerAndClientRequestType.AddRoom:
                                {
                                    if (!appllication.ClientList.Contains(this))
                                    {
                                        Reply(sendData, (byte)LoginServerAndClientResponseType.Login, null, 0, "");
                                    }
                                    else
                                    {
                                        Reply(sendData, (byte)LoginServerAndClientResponseType.AddRoom, null, Convert.ToInt16(appllication.RoomList.Add((string)sendData.Parameters, appllication.ClientList[this])), "");
                                    }
                                }
                                break;
                            case LoginServerAndClientRequestType.AskRoomData:
                                {
                                    if (!appllication.ClientList.Contains(this))
                                    {
                                        Reply(sendData, (byte)LoginServerAndClientResponseType.Login, null, 0, "");
                                    }
                                    else if (!appllication.RoomList.ContainsUsername(appllication.ClientList[this]))
                                    {
                                        Reply(sendData, (byte)LoginServerAndClientResponseType.AddRoom, null, 0, "");
                                    }
                                    else
                                    {
                                        appllication.RoomList.GetRoomWithUsername(appllication.ClientList[this]).SendRoomUpdate();
                                    }
                                }
                                break;
                            case LoginServerAndClientRequestType.Prepare:
                                {
                                    if (!appllication.ClientList.Contains(this))
                                    {
                                        Reply(sendData, (byte)LoginServerAndClientResponseType.Login, null, 0, "");
                                    }
                                    else if (appllication.RoomList.ContainsUsername(appllication.ClientList[this]) &&
                                            !appllication.GameServerList.Contains(appllication.RoomList.GetRoomWithUsername(appllication.ClientList[this])))
                                    {
                                        Room room = appllication.RoomList.GetRoomWithUsername(appllication.ClientList[this]);
                                        room.SetPlayerPrepare(appllication.ClientList[this], Convert.ToBoolean(sendData.Parameters));
                                        if (room.IsAllPrepare)
                                        {
                                            appllication.GameServerList.Add(room);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    break;
                case LoginServerClientType.GameServer:
                    {
                        switch ((LoginServerAndGameServerRequestType)sendData.Code)
                        {
                            case LoginServerAndGameServerRequestType.Login:
                                {
                                    bool success = false;
                                    try
                                    {
                                        success = appllication.GameServerList.Add(this, TraceRoute.IPEndPointParse(sendData.Parameters.ToString(), AddressFamily.InterNetworkV6));
                                    }
                                    catch (Exception) { }
                                    Reply(sendData, (byte)LoginServerAndClientResponseType.Login, null, Convert.ToInt16(success), "");
                                }
                                break;
                            case LoginServerAndGameServerRequestType.AskUserRoomNum:
                                {
                                    string username = sendData.Parameters.ToString();
                                    if(appllication.ClientList.Contains(username) && appllication.RoomList.ContainsUsername(username))
                                    {
                                        Room room = appllication.RoomList.GetRoomWithUsername(username);
                                        Reply(sendData, (byte)LoginServerAndGameServerResponseType.AskUserRoomNum, new Dictionary<string, object>() { { "Username", username }, { "RoomNum", room.RoomNum }, { "index", room.IndexOf(username) } }, 1, "");
                                    }
                                    else
                                    {
                                        Reply(sendData, (byte)LoginServerAndGameServerResponseType.AskUserRoomNum, new Dictionary<string, object>() { { "Username", username }, { "RoomNum", "" }, { "index", 0 } }, 0, "");
                                    }
                                }
                                break;
                            case LoginServerAndGameServerRequestType.EndGame:
                                {
                                    Dictionary<string, string> data = (Dictionary<string, string>)sendData.Parameters;
                                    appllication.GameServerList.Remove(appllication.RoomList[data["RoomNUM"]], data["WinPlayer"]);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        public void Logout()
        {
            if (appllication.ClientList.Contains(this))
            {
                if (appllication.RoomList.ContainsUsername(appllication.ClientList[this]))
                {
                    Room room = appllication.RoomList.GetRoomWithUsername(appllication.ClientList[this]);
                    if (!appllication.GameServerList.Contains(room))
                        appllication.RoomList.RemoveByUsername(appllication.ClientList[this]);
                }
                appllication.ClientList.Remove(this);
            }
        }

        public override void OnDisconnect()
        {
            switch (clientType)
            {
                case LoginServerClientType.Client:
                    {
                        Logout();
                    }
                    break;
                case LoginServerClientType.GameServer:
                    {
                        appllication.GameServerList.Remove(this);
                    }
                    break;
            }
        }
    }
}