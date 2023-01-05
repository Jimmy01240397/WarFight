using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine.Events;

[ExecuteInEditMode]
public class Message : MonoBehaviour
{
    public Text MessageText { get; private set; }
    public Button[] backcloses { get; private set; }
    public Button close { get; private set; }
    public RectTransform Back { get; private set; }
    public RectTransform Head { get; private set; }

    public UnityAction MessageUpdate { get; private set; }
    public UnityAction MessageStart { get; private set; }
    public UnityAction MessageClose { get; private set; }

    private void Awake()
    {
        Back = transform.Find("back").GetComponent<RectTransform>();
        backcloses = new Button[4];
        for(int i = 0; i < backcloses.Length; i++)
        {
            backcloses[i] = transform.Find("backclose" + i).GetComponent<Button>();
        }
        Head = Back.Find("head").GetComponent<RectTransform>();
        close = Head.Find("close").GetComponent<Button>();
        MessageText = Back.Find("MessageText").GetComponent<Text>();
    }

    // Use this for initialization
    void Start () 
    {
        MessageStart?.Invoke();
    }

    #region 設定Messagebox
    public void SetMessage(string Message, bool CloseButtom, UnityAction messageStart, UnityAction messageUpdate, UnityAction MessageClose)
    {
        Awake();
        this.MessageUpdate = messageUpdate;
        this.MessageStart = messageStart;
        this.MessageClose = MessageClose;
        MessageText.text = Message;
        close.gameObject.SetActive(CloseButtom);
        if (CloseButtom)
        {
            close.onClick.AddListener(Close);
            foreach(Button backclose in backcloses)
            {
                backclose.onClick.AddListener(Close);
            }
        }
    }

    #endregion

    void FixedUpdate()
    {
    }

    void Update()
    {
        backcloses[0].GetComponent<RectTransform>().sizeDelta = new Vector2(backcloses[0].GetComponent<RectTransform>().sizeDelta.x,
            GetComponent<RectTransform>().sizeDelta.y / 2 -
            Back.sizeDelta.y / 2 -
            Back.anchoredPosition.y);
        backcloses[0].GetComponent<RectTransform>().offsetMin = new Vector2(
            GetComponent<RectTransform>().sizeDelta.x / 2 -
            Back.sizeDelta.x / 2 + 
            Back.anchoredPosition.x, 
            backcloses[0].GetComponent<RectTransform>().offsetMin.y);

        backcloses[1].GetComponent<RectTransform>().sizeDelta = new Vector2(
            GetComponent<RectTransform>().sizeDelta.x / 2 - 
            Back.sizeDelta.x / 2 -
            Back.anchoredPosition.x,
            backcloses[1].GetComponent<RectTransform>().sizeDelta.y);
        backcloses[1].GetComponent<RectTransform>().offsetMax = new Vector2(backcloses[1].GetComponent<RectTransform>().offsetMax.x,
            -(GetComponent<RectTransform>().sizeDelta.y / 2 -
            Back.sizeDelta.y / 2 -
            Back.anchoredPosition.y));

        backcloses[2].GetComponent<RectTransform>().sizeDelta = new Vector2(backcloses[2].GetComponent<RectTransform>().sizeDelta.x,
            GetComponent<RectTransform>().sizeDelta.y / 2 -
            Back.sizeDelta.y / 2 +
            Back.anchoredPosition.y);
        backcloses[2].GetComponent<RectTransform>().offsetMax = new Vector2(
            -(GetComponent<RectTransform>().sizeDelta.x / 2 -
            Back.sizeDelta.x / 2 - 
            Back.anchoredPosition.x), 
            backcloses[2].GetComponent<RectTransform>().offsetMax.y);

        backcloses[3].GetComponent<RectTransform>().sizeDelta = new Vector2(
            GetComponent<RectTransform>().sizeDelta.x / 2 - 
            Back.sizeDelta.x / 2 +
            Back.anchoredPosition.x,
            backcloses[3].GetComponent<RectTransform>().sizeDelta.y);
        backcloses[3].GetComponent<RectTransform>().offsetMin = new Vector2(backcloses[3].GetComponent<RectTransform>().offsetMin.x,
            GetComponent<RectTransform>().sizeDelta.y / 2 -
            Back.sizeDelta.y / 2 +
            Back.anchoredPosition.y);

        MessageText.GetComponent<RectTransform>().offsetMax = new Vector2(MessageText.GetComponent<RectTransform>().offsetMax.x, -Head.sizeDelta.y);


        //transform.Find("backclose0").GetComponent<RectTransform>().sizeDelta = new Vector2();
        MessageUpdate?.Invoke();
    }

    public void Close()
    {
        MessageClose?.Invoke();
        Destroy(this.gameObject);
    }
}
