using PacketType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarFightLoginServer
{
    public class Room : ICollection
    {
        public string RoomNum { get; private set; }
        public int PlayerCount { get; private set; }

        public int Count
        {
            get
            {
                return Players.Count;
            }
        }

        public object SyncRoot => ((ICollection)Players).SyncRoot;

        public bool IsSynchronized => ((ICollection)Players).IsSynchronized;

        List<string> Players;
        Dictionary<string, bool> PlayerPrepare;

        object locker = new object();

        public string this[int index]
        {
            get
            {
                lock(locker)
                {
                    return Players[index];
                }
            }
        }

        public bool IsAllPrepare
        {
            get
            {
                if (Players.Count != PlayerCount) return false;
                return Players.All((peer) => PlayerPrepare[peer]);
            }
        }

        public Room(string RoomNum, int playerCount)
        {
            this.PlayerCount = playerCount;
            this.RoomNum = RoomNum;
            this.Players = new List<string>();
            PlayerPrepare = new Dictionary<string, bool>();
        }

        public void SendRoomUpdate()
        {
            lock (locker)
            {
                object[][] userdata = new object[PlayerCount][];
                for (int i = 0; i < Players.Count; i++)
                {
                    userdata[i] = new object[2];
                    userdata[i][0] = Players[i];
                    userdata[i][1] = PlayerPrepare[Players[i]];
                }
                foreach (string user in Players)
                {
                    if (Program.appllication.ClientList.Contains(user))
                        Program.appllication.ClientList[user].Tell((byte)LoginServerAndClientEventType.RoomUpdate, new Dictionary<string, object> { { "RoomNUM", RoomNum }, { "users", userdata } });
                }
            }
        }

        public bool Add(string player)
        {
            lock(locker)
            {
                if (Players.Contains(player) || Players.Count >= PlayerCount) return false;
                Players.Add(player);
                PlayerPrepare.Add(player, false);
                SendRoomUpdate();
                return true;
            }
        }

        public bool Remove(string player)
        {
            lock(locker)
            {
                if (!Players.Contains(player)) return false;
                Players.Remove(player);
                PlayerPrepare.Remove(player);
                SendRoomUpdate();
                return true;
            }
        }

        public bool Remove(int index)
        {
            lock (locker)
            {
                string player = Players[index];
                return Remove(player);
            }
        }

        public bool GetPlayerPrepare(string player)
        {
            lock(locker)
            {
                return PlayerPrepare[player];
            }
        }

        public void SetPlayerPrepare(string player, bool isprepare)
        {
            lock(locker)
            {
                PlayerPrepare[player] = isprepare;
                SendRoomUpdate();
            }
        }

        public bool Contains(string player)
        {
            lock(locker)
            {
                return Players.Contains(player);
            }
        }

        public int IndexOf(string player)
        {
            lock(locker)
            {
                return Players.IndexOf(player);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (locker)
            {
                ((ICollection)Players).CopyTo(array, index);
            }
        }

        public IEnumerator GetEnumerator()
        {
            lock (locker)
            {
                return ((IEnumerable)Players).GetEnumerator();
            }
        }
    }
}