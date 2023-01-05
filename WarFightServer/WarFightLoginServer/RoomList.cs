using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarFightLoginServer
{
    public class RoomList : ICollection
    {
        public int PlayerCount { get; private set; }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public object SyncRoot => ((ICollection)list).SyncRoot;

        public bool IsSynchronized => ((ICollection)list).IsSynchronized;

        Dictionary<string, Room> roomnumtoroom;
        Dictionary<string, Room> usernametoroom;
        List<Room> list;


        object locker = new object();

        public Room this[int index]
        {
            get
            {
                lock (locker)
                {
                    return list[index];
                }
            }
        }

        /// <summary>
        /// Get Room by RoomNUM
        /// </summary>
        /// <param name="index">RoomNUM</param>
        /// <returns>room</returns>
        public Room this[string index]
        {
            get
            {
                lock(locker)
                {
                    return roomnumtoroom[index];
                }
            }
        }

        public RoomList(int playerCount)
        {
            this.PlayerCount = playerCount;

            roomnumtoroom = new Dictionary<string, Room>();
            usernametoroom = new Dictionary<string, Room>();
            list = new List<Room>();
        }

        public Room GetRoomWithUsername(string username)
        {
            lock (locker)
            {
                return usernametoroom[username];
            }
        }

        public bool Add(string RoomNum, string username)
        {
            lock(locker)
            {
                Room nowroom;
                if (usernametoroom.ContainsKey(username)) return false;
                if (RoomNum == null)
                {
                    for (RoomNum = new Random(Guid.NewGuid().GetHashCode()).Next(1000, 10000).ToString().PadLeft(4, '0'); 
                        roomnumtoroom.ContainsKey(RoomNum); 
                        RoomNum = new Random(Guid.NewGuid().GetHashCode()).Next(1000, 10000).ToString().PadLeft(4, '0'));
                    nowroom = new Room(RoomNum, PlayerCount);
                    roomnumtoroom.Add(RoomNum, nowroom);
                    list.Add(nowroom);
                    nowroom = null;
                }
                if (!roomnumtoroom.ContainsKey(RoomNum)) return false;
                nowroom = roomnumtoroom[RoomNum];
                if (nowroom.Count >= nowroom.PlayerCount) return false;
                nowroom.Add(username);
                usernametoroom.Add(username, nowroom);
                return true;
            }
        }

        public bool RemoveByUsername(string username)
        {
            lock(locker)
            {
                if (!usernametoroom.ContainsKey(username)) return false;
                Room nowroom = usernametoroom[username];
                nowroom.Remove(username);
                usernametoroom.Remove(username);
                if (nowroom.Count <= 0)
                {
                    roomnumtoroom.Remove(nowroom.RoomNum);
                    list.Remove(nowroom);
                }
                return true;
            }
        }

        public bool RemoveByRoomNum(string RoomNum)
        {
            lock(locker)
            {
                if (!roomnumtoroom.ContainsKey(RoomNum)) return false;
                Room nowroom = roomnumtoroom[RoomNum];
                while(nowroom.Count > 0)
                {
                    RemoveByUsername(nowroom[0]);
                }
                if(list.Contains(nowroom))
                {
                    roomnumtoroom.Remove(RoomNum);
                    list.Remove(nowroom);
                }
                return true;
            }
        }

        public bool RemoveAt(int index)
        {
            lock(locker)
            {
                Room nowroom = list[index];
                return RemoveByRoomNum(nowroom.RoomNum);
            }
        }

        public bool ContainsRoomNum(string RoomNum)
        {
            lock (locker)
            {
                return roomnumtoroom.ContainsKey(RoomNum);
            }
        }
        public bool ContainsUsername(string username)
        {
            lock (locker)
            {
                return usernametoroom.ContainsKey(username);
            }
        }
        public bool Contains(Room room)
        {
            lock (locker)
            {
                return list.Contains(room);
            }
        }

        public int IndexOf(Room room)
        {
            lock(locker)
            {
                return list.IndexOf(room);
            }
        }

        public int IndexOfRoomNum(string RoomNum)
        {
            lock(locker)
            {
                if (!ContainsRoomNum(RoomNum)) return -1;
                return IndexOf(roomnumtoroom[RoomNum]);
            }
        }

        public int IndexOfUsername(string username)
        {
            lock (locker)
            {
                if (!ContainsUsername(username)) return -1;
                return IndexOf(usernametoroom[username]);
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)list).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }
    }
}