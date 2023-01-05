using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JimmikerNetwork;
using JimmikerNetwork.Client;
using System.Net.Sockets;
using PacketType;
using UnityEngine.SceneManagement;

public class ClientForGame : ClientListen
{
    public event Action<LinkCobe> OnConnect;
    public ClientLinker ClientLinker { get; private set; }

    public ClientForGame(ProtocolType protocolType)
    {
        ClientLinker = new ClientLinker(this, protocolType);
    }

    public bool Connect(string host, int port)
    {
        bool on = ClientLinker.Connect(host, port);
        return on;
    }

    public void DebugReturn(string message)
    {
        Debug.Log(message);
    }

    public void OnEvent(SendData sendData)
    {
        switch ((GameServerAndClientEventType)sendData.Code)
        {

        }
    }

    public void OnOperationResponse(SendData sendData)
    {
        switch ((GameServerAndClientResponseType)sendData.Code)
        {
            case GameServerAndClientResponseType.Login:
                {
                    if (Convert.ToBoolean(sendData.ReturnCode))
                    {
                        SceneManager.LoadScene("GameScene");
                    }
                    else
                    {
                        Manager.Instance.ClientForGame.ClientLinker.Disconnect();
                        Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.Logout, null);
                        Manager.Instance.Username = null;
                        Manager.Instance.RoomNum = null;
                        Manager.Instance.RoomPlayer = null;
                        SceneManager.LoadScene("MenuScene");
                    }
                }
                break;
            case GameServerAndClientResponseType.GetMap:
                {
                    Dictionary<string, object> mapdata = (Dictionary<string, object>)sendData.Parameters;
                    GameManager.Instance.MapObjects = (IDictionary[])mapdata["MapObjects"];
                    GameManager.Instance.Builds = (Dictionary<string, IDictionary>)mapdata["builds"];
                    GameManager.Instance.People = (Dictionary<string, IDictionary>)mapdata["people"];
                }
                break;
        }
    }

    public void OnStatusChanged(LinkCobe connect)
    {
        switch (connect)
        {
            case LinkCobe.Connect:
                {
                    ClientLinker.Ask((byte)GameServerAndClientRequestType.Login, Manager.Instance.Username);
                }
                break;
            case LinkCobe.Failed:
                {
                    Debug.Log("Connect Failed");
                }
                break;
            case LinkCobe.Lost:
                {
                    Debug.Log("Disconnect");
                }
                break;
        }
        OnConnect?.Invoke(connect);
    }

    public PeerForP2PBase P2PAddPeer(object _peer, object publicIP, INetClient client, bool NAT)
    {
        throw new System.NotImplementedException();
    }
}