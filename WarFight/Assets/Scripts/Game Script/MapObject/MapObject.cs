using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public abstract class MapObject : MonoBehaviour
{
    public string type;
    public int ID { get; protected set; }
    public int OwnerIndex { get; protected set; }
    public float MaxLife { get; protected set; }
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

    public virtual void SetData(IDictionary data)
    {
        ID = (int)data["ID"];
        OwnerIndex = (int)data["OwnerIndex"];
        Life = (int)data["Life"];
        Vector3 position = GetComponentInParent<TilemapSetting>().GetComponent<Tilemap>().CellToLocal(new Vector3Int((int)data["x"], (int)data["y"]));
        transform.localPosition = new Vector3(position.x, position.y, transform.localPosition.z);
        lifeline.GetComponent<Image>().color = OwnerIndex == -1 ? Color.white : (OwnerIndex == Array.IndexOf(Manager.Instance.RoomPlayer, Manager.Instance.Username) ? Color.blue : Color.red);
    }

    public override int GetHashCode()
    {
        Tilemap tilemap = GetComponentInParent<TilemapSetting>().GetComponent<Tilemap>();
        return PacketType.Tools.HashCombine(GetHashCode(), 
               PacketType.Tools.HashCombine(tilemap.LocalToCell(transform.localPosition).x.GetHashCode(), 
               PacketType.Tools.HashCombine(tilemap.LocalToCell(transform.localPosition).y.GetHashCode(), 
               PacketType.Tools.HashCombine(OwnerIndex.GetHashCode(), 
               PacketType.Tools.HashCombine(lifeline.value.GetHashCode(), 
                                            false.GetHashCode()
               )))));
    }

    protected virtual void Awake()
    {
        if(transform.Find("lifeline") == null)
        {
            GameObject lifelineobj = Instantiate(Resources.Load<GameObject>("Prefabs/Game/lifeline"));
            lifelineobj.transform.SetParent(transform, false);
        }
        lifeline = GetComponentInChildren<Slider>();
        lifeline.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<SpriteRenderer>().sprite.rect.size.x, 3);
        lifeline.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, GetComponent<SpriteRenderer>().sprite.rect.size.y / 2 + 4);
        lifeline.value = 1;
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

}
