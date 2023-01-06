﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using PacketType;

namespace WarFightGameServer
{
    public class MapObjectList : ICollection
    {
        Dictionary<string, Dictionary<string, List<MapObject>>> dict;

        object locker = new object();

        public MapObject this[int index]
        {
            get
            {
                lock (locker)
                {
                    foreach (string objclassname in dict.Keys)
                    {
                        int nowlen = GetCount(objclassname);
                        if (index < nowlen)
                            return this[objclassname, index];
                        index -= nowlen;
                    }
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public MapObject this[string objclassname, int index]
        {
            get
            {
                lock (locker)
                {
                    foreach (string type in dict[objclassname].Keys)
                    {
                        int nowlen = GetCount(objclassname, type);
                        if (index < nowlen)
                            return this[objclassname, type, index];
                        index -= nowlen;
                    }
                    throw new IndexOutOfRangeException();
                }
            }
        }

        public MapObject this[string objclassname, string type, int index]
        {
            get
            {
                lock (locker)
                {
                    return dict[objclassname][type][index];
                }
            }
        }

        public int Count => GetCount();

        public object SyncRoot => ((ICollection)dict).SyncRoot;

        public bool IsSynchronized => ((ICollection)dict).IsSynchronized;

        public int GetCount()
        {
            lock (locker)
            {
                int len = 0;
                foreach (string objclassname in dict.Keys)
                {
                    len += GetCount(objclassname);
                }
                return len;
            }
        }

        public int GetCount(string objclassname)
        {
            lock (locker)
            {
                int len = 0;
                if (dict.ContainsKey(objclassname))
                {
                    foreach (string type in dict[objclassname].Keys)
                    {
                        len += GetCount(objclassname, type);
                    }
                }
                return len;
            }
        }

        public int GetCount(string objclassname, string type)
        {
            lock (locker)
            {
                if (!dict.ContainsKey(objclassname) || !dict[objclassname].ContainsKey(type)) return 0;
                return dict[objclassname][type].Count;
            }
        }

        public MapObjectList()
        {
            dict = new Dictionary<string, Dictionary<string, List<MapObject>>>();
        }

        public void Add<T>(params object[] args) where T : MapObject
        {
            lock (locker)
            {
                MapObject mapObject = (MapObject)Activator.CreateInstance(typeof(T), args: args);
                if (!dict.ContainsKey(mapObject.type.Split('/')[0]))
                    dict.Add(mapObject.type.Split('/')[0], new Dictionary<string, List<MapObject>>());
                if (!dict[mapObject.type.Split('/')[0]].ContainsKey(mapObject.type.Split('/')[1]))
                    dict[mapObject.type.Split('/')[0]].Add(mapObject.type.Split('/')[1], new List<MapObject>());
                dict[mapObject.type.Split('/')[0]][mapObject.type.Split('/')[1]].Add(mapObject);
            }
        }

        public bool Remove(MapObject mapObject)
        {
            lock (locker)
            {
                if (!dict.ContainsKey(mapObject.type.Split('/')[0]) || 
                    !dict[mapObject.type.Split('/')[0]].ContainsKey(mapObject.type.Split('/')[1]) || 
                    !dict[mapObject.type.Split('/')[0]][mapObject.type.Split('/')[1]].Contains(mapObject)) return false;
                dict[mapObject.type.Split('/')[0]][mapObject.type.Split('/')[1]].Remove(mapObject);
                return true;
            }
        }

        public T GetAttributeSum<T>(string objclassname, string attributeName, int OwnerIndex) where T : struct
        {
            dynamic output = 0;
            foreach (var data in (Dictionary<string, IDictionary>)Program.appllication.DataBase[objclassname])
            {
                if(data.Value.Keys.OfType<string>().Contains(attributeName))
                {
                    output += GetCount(objclassname, data.Key) * (dynamic)data.Value[attributeName];
                }
            }
            return (T)output;
        }

        public void CopyTo(Array array, int index)
        {
            lock (locker)
            {
                for (int i = 0; i < GetCount() && i + index < array.Length; i++)
                    array.SetValue(this[i], index + i);
            }
        }

        public void CopyTo(Array array, string objclassname, int index)
        {
            lock (locker)
            {
                for (int i = 0; i < GetCount(objclassname) && i + index < array.Length; i++)
                    array.SetValue(this[objclassname, i], index + i);
            }
        }
        public void CopyTo(Array array, string objclassname, string type, int index)
        {
            lock (locker)
            {
                for (int i = 0; i < GetCount(objclassname, type) && i + index < array.Length; i++)
                    array.SetValue(this[objclassname, type, i], index + i);
            }
        }

        public MapObject[] ToArray()
        {
            lock (locker)
            {
                MapObject[] mapObjects = new MapObject[GetCount()];
                CopyTo(mapObjects, 0);
                return mapObjects;
            }
        }

        public MapObject[] ToArray(string objclassname)
        {
            lock (locker)
            {
                MapObject[] mapObjects = new MapObject[GetCount(objclassname)];
                CopyTo(mapObjects, objclassname, 0);
                return mapObjects;
            }
        }
        public MapObject[] ToArray(string objclassname, string type)
        {
            lock (locker)
            {
                MapObject[] mapObjects = new MapObject[GetCount(objclassname, type)];
                CopyTo(mapObjects, objclassname, type, 0);
                return mapObjects;
            }
        }

        public override int GetHashCode()
        {
            lock(locker)
            {
                if (Count <= 0) return 0;
                MapObject[] mapObjects = ToArray();
                Array.Sort(mapObjects, (a, b) => a.Hash().CompareTo(b.Hash()));
                int output = mapObjects[0].Hash();
                for (int i = 1; i < Count; i++)
                    if(!mapObjects[i].IsDestroy)
                        output = Tools.HashCombine(output, mapObjects[i].Hash());
                return output;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return ToArray().GetEnumerator();
        }
    }
}