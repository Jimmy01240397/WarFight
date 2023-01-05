using JimmikerNetwork.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Connect : MonoBehaviour
{
    public string LoginHost;
    public int LoginPort = 5800;

    // Start is called before the first frame update
    void Start()
    {
        Manager.Instance.ClientForLogin.OnConnect += ClientForLogin_OnConnect;
        Manager.Instance.ClientForLogin.Connect(LoginHost, LoginPort);
        Message message = Instantiate(Resources.Load<GameObject>("Prefabs/UI/MessageBox")).GetComponent<Message>();
        GameObject loading = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Loading"));
        loading.transform.SetParent(message.Back, false);
        loading.transform.localPosition = new Vector3(0, 0);
        message.SetMessage("Connecting", false, null, null, null);
    }

    private void ClientForLogin_OnConnect(LinkCobe connect)
    {
        switch (connect)
        {
            case LinkCobe.Connect:
                {
                    SceneManager.LoadScene("MenuScene");
                }
                break;
            case LinkCobe.Failed:
                {
                    Manager.Instance.ClientForLogin.Connect(LoginHost, LoginPort);
                }
                break;
            case LinkCobe.Lost:
                {
                    Manager.Instance.ClientForLogin.Connect(LoginHost, LoginPort);
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Manager.Instance.ClientForLogin.OnConnect -= ClientForLogin_OnConnect;
    }
}
