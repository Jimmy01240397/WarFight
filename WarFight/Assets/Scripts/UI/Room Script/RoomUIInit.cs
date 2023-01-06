using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PacketType;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class RoomUIInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Manager.Instance.ClientForLogin.RoomUpdate += ClientForLogin_RoomUpdate;

        Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.AskRoomData, null, (responsedata) =>
        {
            switch((LoginServerAndClientResponseType)responsedata.Code)
            {
                case LoginServerAndClientResponseType.Login:
                    {
                        if(!Convert.ToBoolean(responsedata.ReturnCode))
                        {
                            Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.Logout, null);
                            Manager.Instance.Username = null;
                            Manager.Instance.RoomNum = null;
                            Manager.Instance.RoomPlayer = null;
                            SceneManager.LoadScene("MenuScene");
                        }
                    }
                    break;
                case LoginServerAndClientResponseType.AddRoom:
                    {
                        if (!Convert.ToBoolean(responsedata.ReturnCode))
                        {
                            Manager.Instance.RoomNum = null;
                            Manager.Instance.RoomPlayer = null;
                            SceneManager.LoadScene("MenuScene");
                        }
                    }
                    break;
            }
        }, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ClientForLogin_RoomUpdate(Dictionary<string, object> Roomdata)
    {
        object[][] playerdatas = (object[][])Roomdata["users"];
        transform.Find("OtherWindow").Find("Room Number").GetComponent<TextMeshProUGUI>().text = "Room Number\n" + Roomdata["RoomNUM"].ToString();
        PlayersWindow playersWindow = transform.Find("PlayersWindow").GetComponent<PlayersWindow>();
        playersWindow.Length = playerdatas.Length;

        bool readystate = false;

        TextMeshProUGUI readybuttontext = transform.Find("OtherWindow").Find("BtnReady").GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < playerdatas.Length; i++)
        {
            if (playerdatas[i] == null)
                playersWindow[i] = (null, false);
            else
                playersWindow[i] = (playerdatas[i][0].ToString(), (bool)playerdatas[i][1]);
            if(playersWindow[i].Item1 == Manager.Instance.Username)
            {
                readystate = playersWindow[i].Item2;
                readybuttontext.text = readystate ? "Cancel Ready!" : "Ready!";
            }
        }
        if(!readystate)
        {
            bool allreadystate = true;
            foreach((string, bool) data in playersWindow)
                if (data.Item1 != Manager.Instance.Username && !data.Item2)
                    allreadystate = false;
            readybuttontext.text = allreadystate ? "Start Game!" : "Ready!";
        }
    }

    bool ReadyState()
    {
        PlayersWindow playersWindow = transform.Find("PlayersWindow").GetComponent<PlayersWindow>();

        foreach ((string, bool) data in playersWindow)
            if (data.Item1 == Manager.Instance.Username)
                return data.Item2;
        return false;
    }

    public void Prepare()
    {
        Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.Prepare, !ReadyState(), (responsedata) =>
        {
            switch ((LoginServerAndClientResponseType)responsedata.Code)
            {
                case LoginServerAndClientResponseType.Login:
                    {
                        if (!Convert.ToBoolean(responsedata.ReturnCode))
                        {
                            Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.Logout, null);
                            Manager.Instance.Username = null;
                            Manager.Instance.RoomNum = null;
                            Manager.Instance.RoomPlayer = null;
                            SceneManager.LoadScene("MenuScene");
                        }
                    }
                    break;
            }
        }, 1000);
    }

    void OnDestroy()
    {
        Manager.Instance.ClientForLogin.RoomUpdate -= ClientForLogin_RoomUpdate;
    }
}
