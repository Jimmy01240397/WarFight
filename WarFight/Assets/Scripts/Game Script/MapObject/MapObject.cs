using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapObject : MonoBehaviour
{
    public int ID { get; private set; }
    public int OwnerIndex { get; private set; }
    public float MaxLife { get; private set; }
    public float Life
    {
        get
        {
            return lifeline.value * MaxLife;
        }
        set
        {
            lifeline.value = value / MaxLife;
        }
    }

    Slider lifeline;

    private void Awake()
    {
        if(transform.Find("lifeline") == null)
        {
            GameObject lifelineobj = Instantiate(Resources.Load<GameObject>("Prefabs/Game/lifeline"));
            lifelineobj.transform.SetParent(transform, false);
        }
        lifeline = GetComponentInChildren<Slider>();
        Debug.Log(GetComponent<SpriteRenderer>().sprite.rect.size);
        lifeline.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<SpriteRenderer>().sprite.rect.size.x, 3);
        lifeline.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, GetComponent<SpriteRenderer>().sprite.rect.size.y / 2 + 4);
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
