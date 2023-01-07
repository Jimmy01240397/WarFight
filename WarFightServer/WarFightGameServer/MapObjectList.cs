using System;
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

        public MapObject this[int OwnerIndex, int index]
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

        public MapObject this[string objclassname, int OwnerIndex, int index]
        {
            get
            {
                lock (locker)
                {
                    foreach (string type in dict[objclassname].Keys)
                    {
                        int nowlen = GetCount(objclassname, type, OwnerIndex);
                        if (index < nowlen)
                            return this[objclassname, type, OwnerIndex, index];
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

        public MapObject this[string objclassname, string type, int OwnerIndex, int index]
        {
            get
            {
                lock (locker)
                {
                    return dict[objclassname][type].Where((mapobj) => mapobj.OwnerIndex == OwnerIndex).ElementAt(index);
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

        public int GetCount(int OwnerIndex)
        {
            lock (locker)
            {
                int len = 0;
                foreach (string objclassname in dict.Keys)
                {
                    len += GetCount(objclassname, OwnerIndex);
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
        public int GetCount(string objclassname, int OwnerIndex)
        {
            lock (locker)
            {
                int len = 0;
                if (dict.ContainsKey(objclassname))
                {
                    foreach (string type in dict[objclassname].Keys)
                    {
                        len += GetCount(objclassname, type, OwnerIndex);
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

        public int GetCount(string objclassname, string type, int OwnerIndex)
        {
            lock (locker)
            {
                if (!dict.ContainsKey(objclassname) || !dict[objclassname].ContainsKey(type)) return 0;
                return dict[objclassname][type].Where((mapobj) => mapobj.OwnerIndex == OwnerIndex).Count();
            }
        }

        public MapObjectList()
        {
            dict = new Dictionary<string, Dictionary<string, List<MapObject>>>();
        }

        public MapObject Add<T>(Room room, int OwnerIndex, int x, int y, params object[] args) where T : MapObject
        {
            lock (locker)
            {
                object[] allargs = new object[4 + args.Length];
                allargs[0] = room;
                allargs[1] = OwnerIndex;
                allargs[2] = x;
                allargs[3] = y;
                args.CopyTo(allargs, 4);
                MapObject mapObject = (MapObject)Activator.CreateInstance(typeof(T), args: allargs);
                if (!dict.ContainsKey(mapObject.type.Split('/')[0]))
                    dict.Add(mapObject.type.Split('/')[0], new Dictionary<string, List<MapObject>>());
                if (!dict[mapObject.type.Split('/')[0]].ContainsKey(mapObject.type.Split('/')[1]))
                    dict[mapObject.type.Split('/')[0]].Add(mapObject.type.Split('/')[1], new List<MapObject>());
                dict[mapObject.type.Split('/')[0]][mapObject.type.Split('/')[1]].Add(mapObject);
                return mapObject;
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
                if (data.Value.Keys.OfType<string>().Contains(attributeName))
                {
                    output += GetCount(objclassname, data.Key, OwnerIndex) * (dynamic)data.Value[attributeName];
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
        public void CopyTo(Array array, int OwnerIndex, int index)
        {
            lock (locker)
            {
                for (int i = 0; i < GetCount() && i + index < array.Length; i++)
                    array.SetValue(this[OwnerIndex, i], index + i);
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
        public void CopyTo(Array array, string objclassname, int OwnerIndex, int index)
        {
            lock (locker)
            {
                for (int i = 0; i < GetCount(objclassname, OwnerIndex) && i + index < array.Length; i++)
                    array.SetValue(this[objclassname, OwnerIndex, i], index + i);
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
        public void CopyTo(Array array, string objclassname, string type, int OwnerIndex, int index)
        {
            lock (locker)
            {
                for (int i = 0; i < GetCount(objclassname, type, OwnerIndex) && i + index < array.Length; i++)
                    array.SetValue(this[objclassname, type, OwnerIndex, i], index + i);
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
        public MapObject[] ToArray(int OwnerIndex)
        {
            lock (locker)
            {
                MapObject[] mapObjects = new MapObject[GetCount(OwnerIndex)];
                CopyTo(mapObjects, OwnerIndex, 0);
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
        public MapObject[] ToArray(string objclassname, int OwnerIndex)
        {
            lock (locker)
            {
                MapObject[] mapObjects = new MapObject[GetCount(objclassname, OwnerIndex)];
                CopyTo(mapObjects, objclassname, OwnerIndex, 0);
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
        public MapObject[] ToArray(string objclassname, string type, int OwnerIndex)
        {
            lock (locker)
            {
                MapObject[] mapObjects = new MapObject[GetCount(objclassname, type, OwnerIndex)];
                CopyTo(mapObjects, objclassname, type, OwnerIndex, 0);
                return mapObjects;
            }
        }

        public override int GetHashCode()
        {
            lock (locker)
            {
                if (Count <= 0) return 0;
                MapObject[] mapObjects = ToArray();
                Array.Sort(mapObjects, (a, b) => a.Hash().CompareTo(b.Hash()));
                int output = mapObjects[0].Hash();
                for (int i = 1; i < Count; i++)
                    if (!mapObjects[i].IsDestroy)
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