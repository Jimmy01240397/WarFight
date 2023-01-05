using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarFightGameServer
{
    public class RoomList : ICollection
    {
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

        public RoomList()
        {
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

        public bool Add(string RoomNum, int PlayerCount)
        {
            lock(locker)
            {
                if (roomnumtoroom.ContainsKey(RoomNum)) return false;
                Room nowroom;
                nowroom = new Room(RoomNum, PlayerCount, (Dictionary<string, object>)((IDictionary[])Program.appllication.DataBase["Map"])[0]);
                roomnumtoroom.Add(RoomNum, nowroom);
                list.Add(nowroom);
                return true;
            }
        }

        public bool Add(string RoomNum, string username, int index)
        {
            lock (locker)
            {
                Room nowroom;
                if (usernametoroom.ContainsKey(username)) return false;
                if (!roomnumtoroom.ContainsKey(RoomNum)) return false;
                nowroom = roomnumtoroom[RoomNum];
                if(nowroom[index] != null && usernametoroom.ContainsKey(nowroom[index]))
                {
                    usernametoroom.Remove(nowroom[index]);
                    if (Program.appllication.ClientList.Contains(nowroom[index]))
                        Program.appllication.ClientList[nowroom[index]].Close();
                }

                nowroom[index] = username;
                usernametoroom.Add(username, nowroom);
                return true;
            }
        }

        public bool Remove(string RoomNum)
        {
            lock(locker)
            {
                if (!roomnumtoroom.ContainsKey(RoomNum)) return false;
                Room nowroom = roomnumtoroom[RoomNum];
                for(int i = 0; i < nowroom.Count; i++)
                {
                    usernametoroom.Remove(nowroom[i]);
                    if(Program.appllication.ClientList.Contains(nowroom[i]))
                        Program.appllication.ClientList[nowroom[i]].Close();
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
                return Remove(nowroom.RoomNum);
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