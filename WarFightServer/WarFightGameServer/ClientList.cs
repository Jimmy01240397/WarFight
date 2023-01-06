using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarFightGameServer
{
    public class ClientList : ICollection
    {
        Dictionary<object, (string, Peer)> dict;
        List<(string, Peer)> list;
        object locker = new object();

        Func<string, bool> keycheck;

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public object SyncRoot => ((ICollection)list).SyncRoot;

        public bool IsSynchronized => ((ICollection)list).IsSynchronized;

        public (string, Peer) this[int index]
        {
            get
            {
                lock (locker)
                {
                    return list[index];
                }
            }
        }

        public string this[Peer index]
        {
            get
            {
                lock (locker)
                {
                    return dict[index].Item1;
                }
            }
        }

        public Peer this[string index]
        {
            get
            {
                lock (locker)
                {
                    return dict[index].Item2;
                }
            }
        }

        public ClientList(Func<string, bool> keycheck = null)
        {
            dict = new Dictionary<object, (string, Peer)>();
            list = new List<(string, Peer)>();
            this.keycheck = keycheck;
        }

        public bool Add(string id, Peer peer)
        {
            lock (locker)
            {
                if (keycheck != null && !keycheck(id)) return false;
                if (dict.ContainsKey(id) || dict.ContainsKey(peer)) return false;
                dict.Add(id, (id, peer));
                dict.Add(peer, (id, peer));
                list.Add((id, peer));
                return true;
            }
        }

        private bool Remove((string, Peer) data)
        {
            lock(locker)
            {
                if (!list.Contains(data)) return false;
                dict.Remove(data.Item1);
                dict.Remove(data.Item2);
                list.Remove(data);
                return true;
            }
        }

        public bool Remove(string id)
        {
            lock(locker)
            {
                if (!dict.ContainsKey(id)) return false;
                var data = dict[id];
                return Remove(data);
            }
        }

        public bool Remove(Peer peer)
        {
            lock (locker)
            {
                if (!dict.ContainsKey(peer)) return false;
                var data = dict[peer];
                return Remove(data);
            }
        }

        public bool Remove(int index)
        {
            lock (locker)
            {
                var data = list[index];
                return Remove(data);
            }
        }

        public bool Contains(string id)
        {
            lock (locker)
            {
                return dict.ContainsKey(id);
            }
        }
        public bool Contains(Peer peer)
        {
            lock (locker)
            {
                return dict.ContainsKey(peer);
            }
        }
        public bool Contains((string, Peer) data)
        {
            lock (locker)
            {
                return list.Contains(data);
            }
        }

        public int IndexOf((string, Peer) data)
        {
            lock(locker)
            {
                return list.IndexOf(data);
            }
        }

        public int IndexOf(string id)
        {
            lock (locker)
            {
                if (!Contains(id)) return -1;
                return IndexOf(dict[id]);
            }
        }

        public int IndexOf(Peer peer)
        {
            lock (locker)
            {
                if (!Contains(peer)) return -1;
                return IndexOf(dict[peer]);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (locker)
            {
                ((ICollection)list).CopyTo(array, index);
            }
        }

        public IEnumerator GetEnumerator()
        {
            lock (locker)
            {
                return ((IEnumerable)list).GetEnumerator();
            }
        }
    }
}