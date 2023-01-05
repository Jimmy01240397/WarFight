using JimmikerNetwork.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    private static Manager instance = null;
    public static Manager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject Manager = new GameObject("Manager");
                instance = Manager.AddComponent<Manager>();
                DontDestroyOnLoad(Manager);
            }
            return instance;
        }
    }

    public string Username { get; set; } = null;

    public string RoomNum { get; set; } = null;
    public string[] RoomPlayer { get; set; } = null;

    public ClientForLogin ClientForLogin { get; private set; }
    public ClientForGame ClientForGame { get; private set; }

    void Awake()
    {
        ClientForLogin = new ClientForLogin(System.Net.Sockets.ProtocolType.Tcp);
        ClientForGame = new ClientForGame(System.Net.Sockets.ProtocolType.Udp);
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ClientForLogin.ClientLinker.Update();
        ClientForGame.ClientLinker.Update();
    }

    private void OnDestroy()
    {
        OnApplicationQuit();
    }

    void OnApplicationQuit()
    {
        ClientForLogin?.ClientLinker.Disconnect();
        ClientForGame?.ClientLinker.Disconnect();
        Action action = () =>
        {
            while(ClientForLogin.ClientLinker.linkstate == LinkCobe.Connect || ClientForGame.ClientLinker.linkstate == LinkCobe.Connect)
            {
                ClientForLogin.ClientLinker.Update();
                ClientForGame.ClientLinker.Update();
            }
        };
        action.BeginInvoke((ar) => action.EndInvoke(ar), null);
    }
}
