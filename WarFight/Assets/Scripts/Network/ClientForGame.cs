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
            case GameServerAndClientEventType.UpdateMapObjects:
                {
                    int hash = (int)((IDictionary)sendData.Parameters)["hash"];
                    GameManager.Instance.Population = (((int[])((IDictionary)sendData.Parameters)["population"])[0], ((int[])((IDictionary)sendData.Parameters)["population"])[1]);
                    GameManager.Instance.Food = (((int[])((IDictionary)sendData.Parameters)["food"])[0], ((int[])((IDictionary)sendData.Parameters)["food"])[1]);
                    GameManager.Instance.Ore = (((int[])((IDictionary)sendData.Parameters)["ore"])[0], ((int[])((IDictionary)sendData.Parameters)["ore"])[1]);
                    IDictionary[] MapObjectData = (IDictionary[])((IDictionary)sendData.Parameters)["MapObjects"];
                    foreach(IDictionary data in MapObjectData)
                    {
                        GameManager.Instance.MapObjects.SetMapObject(data);
                    }
                    if(hash != GameManager.Instance.MapObjects.GetHashCode())
                    {
                        ClientLinker.Ask((byte)GameServerAndClientRequestType.GetMapObjects, null);
                    }
                }
                break;
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
                    //GameManager.Instance.MapObjects = (IDictionary[])mapdata["MapObjects"];
                    GameManager.Instance.Builds = (Dictionary<string, IDictionary>)mapdata["Builds"];
                    GameManager.Instance.People = (Dictionary<string, IDictionary>)mapdata["People"];
                    Camera.main.transform.position = new Vector3(((int[])mapdata["CameraPosition"])[0], ((int[])mapdata["CameraPosition"])[1], Camera.main.transform.position.z);
                }
                break;
            case GameServerAndClientResponseType.GetMapObjects:
                {
                    GameManager.Instance.Population = (((int[])((IDictionary)sendData.Parameters)["population"])[0], ((int[])((IDictionary)sendData.Parameters)["population"])[1]);
                    GameManager.Instance.Food = (((int[])((IDictionary)sendData.Parameters)["food"])[0], ((int[])((IDictionary)sendData.Parameters)["food"])[1]);
                    GameManager.Instance.Ore = (((int[])((IDictionary)sendData.Parameters)["ore"])[0], ((int[])((IDictionary)sendData.Parameters)["ore"])[1]);
                    IDictionary[] MapObjectData = (IDictionary[])((IDictionary)sendData.Parameters)["MapObjects"];
                    GameManager.Instance.MapObjects.ResetMapObjects(MapObjectData);
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