using JimmikerNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace WarFightLoginServer
{
    public class GameServer : ICollection
    {
        public Peer Peer { get; private set; }
        public IPEndPoint IPEndPoint { get; private set; }

        public int Count
        {
            get
            {
                lock (locker)
                {
                    return rooms.Count;
                }
            }
        }

        public object SyncRoot => ((ICollection)rooms).SyncRoot;

        public bool IsSynchronized => ((ICollection)rooms).IsSynchronized;

        List<Room> rooms;

        object locker = new object();

        public Room this[int index]
        {
            get
            {
                lock(locker)
                {
                    return rooms[index];
                }
            }
        }

        public GameServer(Peer peer, IPEndPoint iPEndPoint)
        {
            Peer = peer;
            IPEndPoint = iPEndPoint;
            rooms = new List<Room>();
        }

        public bool Add(Room room)
        {
            lock(locker)
            {
                if (rooms.Contains(room)) return false;
                rooms.Add(room);
                return true;
            }
        }

        public bool Remove(Room room)
        {
            lock(locker)
            {
                if (!rooms.Contains(room)) return false;
                rooms.Remove(room);
                return true;
            }
        }

        public bool Remove(int index)
        {
            lock (locker)
            {
                rooms.RemoveAt(index);
                return true;
            }
        }

        public bool Contains(Room room)
        {
            lock(locker)
            {
                return rooms.Contains(room);
            }
        }

        public int IndexOf(Room room)
        {
            lock(locker)
            {
                return rooms.IndexOf(room);
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)rooms).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)rooms).GetEnumerator();
        }
    }
}