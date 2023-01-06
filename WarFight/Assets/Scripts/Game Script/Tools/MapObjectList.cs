using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Object = UnityEngine.Object;

public class MapObjectList : ICollection
{
    public enum SelectMode
    {
        Index = 0,
        ID
    }

    Dictionary<int, MapObject> dict;
    List<int> list;

    public int Count => list.Count;

    public bool IsSynchronized => ((ICollection)list).IsSynchronized;

    public object SyncRoot => ((ICollection)list).SyncRoot;

    public MapObject this[SelectMode selectMode, int index]
    {
        get
        {
            switch(selectMode)
            {
                case SelectMode.Index:
                    {
                        return dict[index];
                    }
                case SelectMode.ID:
                    {
                        return dict[list[index]];
                    }
                default:
                    {
                        return null;
                    }
            }
        }
    }

    public override int GetHashCode()
    {
        if (Count <= 0) return 0;
        MapObject[] mapObjects = ToArray();
        Array.Sort(mapObjects, (a, b) => a.GetHashCode().CompareTo(b.GetHashCode()));
        int output = mapObjects[0].GetHashCode();
        for (int i = 1; i < Count; i++)
            output = PacketType.Tools.HashCombine(output, mapObjects[i].GetHashCode());
        return output;
    }

    public void SetMapObject(IDictionary data)
    {
        if (!dict.ContainsKey((int)data["ID"]) && !(bool)data["Destroy"])
        {
            GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Game/" + data["type"]));
            obj.transform.parent = GameObject.Find("Grid").transform.Find("Tilemap");
            dict.Add((int)data["ID"], obj.GetComponent<MapObject>());
            list.Add((int)data["ID"]);
        }
        if (dict.ContainsKey((int)data["ID"]))
        {
            if((bool)data["Destroy"])
                Remove((int)data["ID"]);
            else
                dict[(int)data["ID"]].SetData(data);
        }
    }

    public void ResetMapObjects(IDictionary[] data)
    {
        for(int i = 0; i < list.Count; i++)
        {
            IDictionary[] nowdata = data.Where((now) => list[i] == (int)now["ID"]).ToArray();
            if (nowdata.Length <= 0)
            {
                Remove(list[i]);
                i--;
            }
            else
            {
                SetMapObject(nowdata[0]);
            }
        }
    }

    public bool Remove(int ID)
    {
        if (!list.Contains(ID)) return false;
        Object.Destroy(dict[ID].gameObject);
        dict.Remove(ID);
        list.Remove(ID);
        return true;
    }

    public void Clear()
    {
        foreach(int ID in list)
        {
            Object.Destroy(dict[ID].gameObject);
        }
        dict.Clear();
        list.Clear();
    }

    public void CopyTo(System.Array array, int index)
    {
        for (int i = 0; i < Count && i + index < array.Length; i++)
            array.SetValue(this[SelectMode.Index, i], index + i);
    }

    public MapObject[] ToArray()
    {
            MapObject[] mapObjects = new MapObject[Count];
            CopyTo(mapObjects, 0);
            return mapObjects;
    }

    public IEnumerator GetEnumerator()
    {
        return ToArray().GetEnumerator();
    }
}
