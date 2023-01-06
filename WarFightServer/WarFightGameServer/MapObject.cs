
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketType;

namespace WarFightGameServer
{
    public abstract class MapObject
    {
        public string type { get; protected set; }
        public Room Room { get; private set; }
        public int x { get; set; }
        public int y { get; set; }
        public int OwnerIndex { get; set; }
        public float Life { get; set; }

        public bool IsDestroy { get; private set; }

        public int LastHash { get; private set; }

        public virtual int width { get; }
        public virtual int height { get; }

        public MapObject(Room room, int OwnerIndex, int x, int y)
        {
            this.Room = room;
            this.OwnerIndex = OwnerIndex;
            this.x = x;
            this.y = y;
            type = "";
            Life = (float)((Dictionary<string, IDictionary>)Program.appllication.DataBase[type.Split('/')[0]])[type.Split('/')[1]]["life"];
            IsDestroy = false;
            width = 1;
            height = 1;

            for (LastHash = Guid.NewGuid().GetHashCode(); LastHash == Hash(); LastHash = Guid.NewGuid().GetHashCode()) ;
        }

        public virtual void Update()
        {

        }

        public virtual Dictionary<string, object> ObjectToData()
        {
            return new Dictionary<string, object>() { { "ID", GetHashCode() }, { "type", type }, { "OwnerIndex", OwnerIndex }, { "Life", Life }, { "x", x }, { "y", y }, { "Destroy", IsDestroy } };
        }

        public bool IsInAttackRange(MapObject target)
        {
            if (!((Dictionary<string, IDictionary>)Program.appllication.DataBase[type.Split('/')[0]])[type.Split('/')[1]].Contains("attackrange")) return false;
            return Math.Sqrt(
                        Math.Pow(x < target.x-target.width/2 ? target.x - target.width/2 - x : (x > target.x + target.width / 2 ? x - target.x - target.width / 2:0), 2) +
                        Math.Pow(y < target.y - target.height / 2 ? target.y - target.height / 2 - y : (y > target.y + target.height / 2 ? y - target.y - target.height / 2 : 0), 2)
                        ) <= (int)((Dictionary<string, IDictionary>)Program.appllication.DataBase[type.Split('/')[0]])[type.Split('/')[1]]["attackrange"];
        }

        public virtual int Hash()
        {
            float maxlife = (float)((Dictionary<string, IDictionary>)Program.appllication.DataBase[type.Split('/')[0]])[type.Split('/')[1]]["life"];
            return Tools.HashCombine(GetHashCode(), 
                   Tools.HashCombine(x.GetHashCode(), 
                   Tools.HashCombine(y.GetHashCode(), 
                   Tools.HashCombine(OwnerIndex.GetHashCode(), 
                   Tools.HashCombine((Life / maxlife).GetHashCode(), 
                                     IsDestroy.GetHashCode()
                   )))));
        }

        public void UpdateLastHash()
        {
            LastHash = Hash();
        }

        public void Destroy()
        {
            IsDestroy = true;
        }
    }
}