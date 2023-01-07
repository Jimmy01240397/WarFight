using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketType;
using WarFightGameServer.Builds;
using WarFightGameServer.People;

namespace WarFightGameServer
{
    public class Room : ICollection
    {
        public AStar Map { get; private set; }
        public string RoomNum { get; private set; }

        public MapObjectList MapObjects { get; private set; }

        public IDictionary[] InitMapObjectDatas { get; private set; }

        public int[] Ore { get; private set; }

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
            this.Ore = new int[playerCount];
            Map = new AStar((byte[][])mapdata["TileMap"]);
            MapObjects = new MapObjectList();
            InitMapObjectDatas = (IDictionary[])mapdata["InitMapObjectDatas"];
            for(int i = 0; i < InitMapObjectDatas.Length; i++)
            {
                MainBuild mainBuild = null;
                foreach(IDictionary data in (IDictionary[])InitMapObjectDatas[i]["Builds"])
                {
                    switch((string)data["type"])
                    {
                        case "mainbuild":
                            {
                                mainBuild = (MainBuild)MapObjects.Add<MainBuild>(this, i, (int)data["x"], (int)data["y"]);
                            }
                            break;
                    }
                }
                foreach (IDictionary data in (IDictionary[])InitMapObjectDatas[i]["People"])
                {
                    for (int k = 0; k < (int)data["count"]; k++)
                    {
                        switch ((string)data["type"])
                        {
                            case "villager":
                                {
                                    MapObjects.Add<Villager>(this, i,
                                        new Random(Guid.NewGuid().GetHashCode()).Next(mainBuild.x - mainBuild.width / 2, mainBuild.x + mainBuild.width / 2 + 1),
                                        new Random(Guid.NewGuid().GetHashCode()).Next(mainBuild.y - mainBuild.height / 2, mainBuild.y + mainBuild.height / 2 + 1)
                                        );
                                }
                                break;
                        }
                    }
                }
            }
        }

        public void Update()
        {
            lock (locker)
            {
                MapObject[] mapObjects = MapObjects.ToArray();
                foreach (MapObject mapObject in mapObjects)
                {
                    mapObject.Update();
                }

                List<Dictionary<string, object>> sendMapObjectList = new List<Dictionary<string, object>>();
                foreach (MapObject mapObject in mapObjects)
                {
                    if (mapObject.LastHash != mapObject.Hash())
                    {
                        sendMapObjectList.Add(mapObject.ObjectToData());
                        mapObject.UpdateLastHash();
                    }
                    if (mapObject.IsDestroy)
                        MapObjects.Remove(mapObject);
                }
                for (int i = 0; i < Players.Length; i++)
                {
                    if (Players[i] != null && Program.appllication.ClientList.Contains(Players[i]))
                        Program.appllication.ClientList[Players[i]].Tell((byte)GameServerAndClientEventType.UpdateMapObjects, new Dictionary<string, object> 
                        {
                            { "hash", MapObjects.GetHashCode() },
                            { "population", new int[] { MapObjects.GetAttributeSum<int>("People", "population", i), MapObjects.GetAttributeSum<int>("Builds", "population", i) } },
                            { "food", new int[] { MapObjects.GetAttributeSum<int>("People", "food", i), MapObjects.GetAttributeSum<int>("Builds", "food", i) } },
                            { "ore", new int[] { MapObjects.GetAttributeSum<int>("Builds", "ore", i), Ore[i] } },
                            { "MapObjects", sendMapObjectList.ToArray() } 
                        });
                }
            }
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