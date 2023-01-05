using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PacketType;
using System;
using UnityEngine.SceneManagement;

public class MenuUIInit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    public void Login()
    {
        if (string.IsNullOrEmpty(Manager.Instance.Username))
        {
            Message message = Instantiate(Resources.Load<GameObject>("Prefabs/UI/MessageBox")).GetComponent<Message>();
            GameObject AskName = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Menu/AskString"));
            AskName.transform.SetParent(message.Back, false);
            message.SetMessage("Enter Your Nickname", false, null, null, null);
            AskName.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(() =>
            {
                AskName.transform.Find("Confirm").GetComponent<Button>().interactable = false;
                Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.Login, AskName.transform.Find("name").GetComponent<TMP_InputField>().text, (responsedata) =>
                {
                    switch ((LoginServerAndClientResponseType)responsedata.Code)
                    {
                        case LoginServerAndClientResponseType.Login:
                            {
                                if (Convert.ToBoolean(responsedata.ReturnCode))
                                {
                                    Manager.Instance.Username = AskName.transform.Find("name").GetComponent<TMP_InputField>().text;
                                    transform.Find("MainWindow").Find("ShowName").Find("TxtName").GetComponent<TextMeshProUGUI>().text = "Nickname: " + Manager.Instance.Username;
                                    message.Close();
                                }
                                else
                                {
                                    message.MessageText.text = "Login Fail! Please change your Nicename";
                                    AskName.transform.Find("Confirm").GetComponent<Button>().interactable = true;
                                }
                            }
                            break;
                        default:
                            {
                                message.MessageText.text = "Login Fail! Please change your Nicename";
                                AskName.transform.Find("Confirm").GetComponent<Button>().interactable = true;
                            }
                            break;
                    }
                }, 1000);
            });
        }
    }

    public void Logout()
    {
        Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.Logout, null);
        Manager.Instance.Username = null;
        transform.Find("MainWindow").Find("ShowName").Find("TxtName").GetComponent<TextMeshProUGUI>().text = "Nickname: ";
        Login();
    }

    void SendAddRoom(string RoomNum)
    {
        Manager.Instance.ClientForLogin.ClientLinker.Ask((byte)LoginServerAndClientRequestType.AddRoom, RoomNum, (responsedata) =>
        {
            switch ((LoginServerAndClientResponseType)responsedata.Code)
            {
                case LoginServerAndClientResponseType.Login:
                    {
                        if (!Convert.ToBoolean(responsedata.ReturnCode))
                        {
                            Logout();
                        }
                    }
                    break;
                case LoginServerAndClientResponseType.AddRoom:
                    {
                        if (Convert.ToBoolean(responsedata.ReturnCode))
                        {
                            SceneManager.LoadScene("RoomScene");
                        }
                        else
                        {
                            Message message = Instantiate(Resources.Load<GameObject>("Prefabs/UI/MessageBox")).GetComponent<Message>();
                            message.SetMessage("Bad Room Number", true, null, null, null);
                        }
                    }
                    break;
            }
        }, 1000);
    }

    public void AddRoom()
    {
        Message message = Instantiate(Resources.Load<GameObject>("Prefabs/UI/MessageBox")).GetComponent<Message>();
        GameObject AskRoomNUM = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Menu/AskString"));
        AskRoomNUM.transform.SetParent(message.Back, false);
        message.SetMessage("Enter Room Number", true, null, null, null);
        AskRoomNUM.transform.Find("Confirm").GetComponent<Button>().onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(AskRoomNUM.transform.Find("name").GetComponent<TMP_InputField>().text))
            {
                message.MessageText.text = "Room Number can't be empty.";
                return;
            }
            SendAddRoom(AskRoomNUM.transform.Find("name").GetComponent<TMP_InputField>().text);
            message.Close();
        });
    }

    public void NewRoom()
    {
        SendAddRoom(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
