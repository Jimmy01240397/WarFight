using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JimmikerNetwork;
using JimmikerNetwork.Client;
using System.Net.Sockets;
using PacketType;
using System;
using UnityEngine.SceneManagement;
using System.Net;

public class ClientForLogin : ClientListen
{
    public event Action<LinkCobe> OnConnect;
    public event Action<Dictionary<string, object>> RoomUpdate;
    public ClientLinker ClientLinker { get; private set; }

    public ClientForLogin(ProtocolType protocolType)
    {
        ClientLinker = new ClientLinker(this, protocolType);
    }

    public bool Connect(string host, int port)
    {
        bool on = ClientLinker.Connect(host, port);
        Debug.Log(on);
        return on;
    }

    public void DebugReturn(string message)
    {
        Debug.Log(message);
    }

    public void OnEvent(SendData sendData)
    {
        switch((LoginServerAndClientEventType)sendData.Code)
        {
            case LoginServerAndClientEventType.RoomUpdate:
                {
                    Dictionary<string, object> data = (Dictionary<string, object>)sendData.Parameters;
                    object[][] playerdatas = (object[][])data["users"];
                    Manager.Instance.RoomNum = data["RoomNUM"].ToString();
                    Manager.Instance.RoomPlayer = new string[playerdatas.Length];
                    for(int i = 0; i < playerdatas.Length; i++)
                    {
                        if (playerdatas[i] == null)
                            Manager.Instance.RoomPlayer[i] = null;
                        else
                            Manager.Instance.RoomPlayer[i] = (string)playerdatas[i][0];
                    }
                    RoomUpdate?.Invoke(data);
                }
                break;
            case LoginServerAndClientEventType.Start:
                {
                    IPEndPoint endPoint = TraceRoute.IPEndPointParse(sendData.Parameters.ToString(), AddressFamily.InterNetworkV6);
                    if (endPoint.Address.IsIPv4MappedToIPv6) endPoint = new IPEndPoint(endPoint.Address.MapToIPv4(), endPoint.Port);
                    Manager.Instance.ClientForGame.Connect(endPoint.Address.ToString(), endPoint.Port);
                }
                break;
            case LoginServerAndClientEventType.EndGame:
                {
                    Manager.Instance.ClientForGame.ClientLinker.Disconnect();
                    SceneManager.LoadScene("RoomScene");
                }
                break;
        }
    }

    public void OnOperationResponse(SendData sendData)
    {
        switch ((LoginServerAndClientResponseType)sendData.Code)
        {
        }
    }

    public void OnStatusChanged(LinkCobe connect)
    {
        switch (connect)
        {
            case LinkCobe.Connect:
                {
                    ClientLinker.Ask((byte)LoginServerClientType.Client, null);
                }
                break;
            case LinkCobe.Failed:
                {
                    Debug.Log("Connect Failed");
                    ClientLinker.Disconnect();
                    if (SceneManager.GetActiveScene().name != "Connect")
                        SceneManager.LoadScene("Connect");
                }
                break;
            case LinkCobe.Lost:
                {
                    Debug.Log("Disconnect");
                    ClientLinker.Disconnect();
                    if (SceneManager.GetActiveScene().name != "Connect")
                        SceneManager.LoadScene("Connect");
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
