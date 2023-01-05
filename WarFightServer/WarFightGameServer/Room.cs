using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarFightGameServer
{
    public class Room : ICollection
    {
        public AStar Map { get; private set; }
        public string RoomNum { get; private set; }

        public IDictionary[] MapObjects { get; set; }

        public int Count
        {
            get
            {
                return Players.Length;
            }
        }

        public object SyncRoot => ((ICollection)Players).SyncRoot;

        public bool IsSynchronized => ((ICollection)Players).IsSynchronized;

        string[] Players;

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
            set
            {
                lock(locker)
                {
                    Players[index] = value;
                }
            }
        }

        public Room(string RoomNum, int playerCount, Dictionary<string, object> mapdata)
        {
            this.RoomNum = RoomNum;
            this.Players = new string[playerCount];
            Map = new AStar((byte[][])mapdata["TileMap"]);
            MapObjects = (IDictionary[])mapdata["MapObjects"];
        }

        public bool Contains(string player)
        {
            lock(locker)
            {
                return IndexOf(player) != -1;
            }
        }

        public int IndexOf(string player)
        {
            lock(locker)
            {
                return Array.IndexOf(Players, player);
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((ICollection)Players).CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)Players).GetEnumerator();
        }
    }
}