using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class PlayersWindow : MonoBehaviour, ICollection, IEnumerable<(string, bool)>
{
    GameObject[] players;


    public (string, bool) this[int index]
    {
        get
        {
            if (players[index] == null) return (null, false);
            Transform background = players[index].transform.Find("Background");
            return (background.Find("name").GetComponent<TextMeshProUGUI>().text, background.Find("state").GetComponent<TextMeshProUGUI>().text == "ready");
        }
        set
        {
            if (value.Item1 == null)
            {
                if (players[index] != null)
                {
                    Destroy(players[index]);
                    players[index] = null;
                }
            }
            else
            {
                if (players[index] == null)
                {
                    players[index] = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Room/PlayerInfo"));
                    players[index].transform.SetParent(transform, false);
                }
                players[index].GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().GetSize().x / players.Length, players[index].GetComponent<RectTransform>().sizeDelta.y);
                players[index].GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().GetSize().x / players.Length * index, players[index].GetComponent<RectTransform>().anchoredPosition.y);
                Transform background = players[index].transform.Find("Background");
                background.Find("name").GetComponent<TextMeshProUGUI>().text = value.Item1;
                background.Find("state").GetComponent<TextMeshProUGUI>().text = value.Item2 ? "ready" : "not ready";
            }
        }
    }

    public int Length
    {
        get
        {
            return players.Length;
        }
        set
        {
            if (players == null) players = new GameObject[value];
            else if (players.Length == value) return;
            else
            {
                for (int i = value; i < players.Length; i++)
                    this[i] = (null, false);
                Array.Resize(ref players, value);
            }
            for (int i = 0; i < value; i++)
            {
                if (players[i] != null)
                {
                    players[i].GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x / players.Length, players[i].GetComponent<RectTransform>().sizeDelta.y);
                    players[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().sizeDelta.x / players.Length * i, players[i].GetComponent<RectTransform>().anchoredPosition.y);
                }
            }
        }
    }

    public int Count => ((ICollection)players).Count;

    public bool IsSynchronized => players.IsSynchronized;

    public object SyncRoot => players.SyncRoot;

    public void CopyTo(Array array, int index)
    {
        for(int i = 0; i < players.Length && index + i < array.Length; i++)
        {
            array.SetValue(this[i], index + i);
        }
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable<(string, bool)>)this).GetEnumerator();
    }

    IEnumerator<(string, bool)> IEnumerable<(string, bool)>.GetEnumerator()
    {
        (string, bool)[] data = new (string, bool)[Length];
        CopyTo(data, 0);
        return new List<(string, bool)>(data).GetEnumerator();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
