using PacketType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WarFightLoginServer
{
    public class GameServerList : ICollection
    {
        Dictionary<object, GameServer> dict;
        List<GameServer> list;

        object locker = new object();

        public int Count
        {
            get
            {
                lock (locker)
                {
                    return list.Count;
                }
            }
        }

        public object SyncRoot => ((ICollection)list).SyncRoot;

        public bool IsSynchronized => ((ICollection)list).IsSynchronized;

        public GameServer this[int index]
        {
            get
            {
                lock(locker)
                {
                    return list[index];
                }
            }
        }

        public GameServer this[IPEndPoint index]
        {
            get
            {
                lock(locker)
                {
                    return dict[index];
                }
            }
        }

        public GameServer this[Peer index]
        {
            get
            {
                lock (locker)
                {
                    return dict[index];
                }
            }
        }

        public GameServer this[Room index]
        {
            get
            {
                lock(locker)
                {
                    return dict[index];
                }
            }
        }

        public GameServerList()
        {
            dict = new Dictionary<object, GameServer>();
            list = new List<GameServer>();
        }

        public bool Add(Peer peer, IPEndPoint iPEndPoint)
        {
            lock(locker)
            {
                if (dict.ContainsKey(peer) || dict.ContainsKey(iPEndPoint)) return false;
                GameServer gameServer = new GameServer(peer, iPEndPoint);
                dict.Add(peer, gameServer);
                dict.Add(iPEndPoint, gameServer);
                list.Add(gameServer);
                return true;
            }
        }

        public bool Add(Room room)
        {
            lock(locker)
            {
                if (dict.ContainsKey(room) || list.Count <= 0) return false;
                var gameServer = list[new Random(Guid.NewGuid().GetHashCode()).Next(list.Count)];
                dict.Add(room, gameServer);
                gameServer.Add(room);
                gameServer.Peer.Tell((byte)LoginServerAndGameServerEventType.NewGame, new Dictionary<string, object> { { "RoomNum", room.RoomNum },{ "PlayerCount", room.PlayerCount } });
                foreach (string player in room)
                {
                    if (Program.appllication.ClientList.Contains(player))
                        Program.appllication.ClientList[player].Tell((byte)LoginServerAndClientEventType.Start, gameServer.IPEndPoint.ToString());
                }
                return true;
            }
        }

        public bool Remove(Room room, string win = null)
        {
            lock(locker)
            {
                if (!dict.ContainsKey(room)) return false;
                GameServer gameServer = dict[room];
                dict.Remove(room);
                gameServer.Remove(room);
                gameServer.Peer.Tell((byte)LoginServerAndGameServerEventType.EndGame, room.RoomNum);
                foreach (string player in room)
                {
                    if (Program.appllication.ClientList.Contains(player))
                        Program.appllication.ClientList[player].Tell((byte)LoginServerAndClientEventType.EndGame, new Dictionary<string, string>() { { "endpoint", gameServer.IPEndPoint.ToString() }, { "winplayer", win } });
                    else
                        Program.appllication.RoomList.RemoveByUsername(player);
                }
                return true;
            }
        }

        public bool Remove(GameServer gameServer)
        {
            lock(locker)
            {
                if (!list.Contains(gameServer)) return false;
                Room[] rooms = new Room[gameServer.Count];
                foreach(Room room in rooms)
                {
                    Remove(room);
                }
                dict.Remove(gameServer.Peer);
                dict.Remove(gameServer.IPEndPoint);
                list.Remove(gameServer);
                return true;
            }
        }

        public bool Remove(IPEndPoint iPEndPoint)
        {
            lock (locker)
            {
                if (!dict.ContainsKey(iPEndPoint)) return false;
                return Remove(dict[iPEndPoint]);
            }
        }

        public bool Remove(Peer peer)
        {
            lock (locker)
            {
                if (!dict.ContainsKey(peer)) return false;
                return Remove(dict[peer]);
            }
        }

        public bool Remove(int index)
        {
            lock (locker)
            {
                return Remove(list[index]);
            }
        }

        public bool Contains(Peer peer)
        {
            lock(locker)
            {
                return dict.ContainsKey(peer);
            }
        }

        public bool Contains(IPEndPoint iPEndPoint)
        {
            lock (locker)
            {
                return dict.ContainsKey(iPEndPoint);
            }
        }

        public bool Contains(Room room)
        {
            lock (locker)
            {
                return dict.ContainsKey(room);
            }
        }

        public bool Contains(GameServer gameServer)
        {
            lock (locker)
            {
                return list.Contains(gameServer);
            }
        }

        public int IndexOf(GameServer gameServer)
        {
            lock(locker)
            {
                return list.IndexOf(gameServer);
            }
        }

        public int IndexOf(Peer peer)
        {
            lock(locker)
            {
                if (!Contains(peer)) return -1;
                return IndexOf(dict[peer]);
            }
        }
        public int IndexOf(IPEndPoint iPEndPoint)
        {
            lock (locker)
            {
                if (!Contains(iPEndPoint)) return -1;
                return IndexOf(dict[iPEndPoint]);
            }
        }
        public int IndexOf(Room room)
        {
            lock (locker)
            {
                if (!Contains(room)) return -1;
                return IndexOf(dict[room]);
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