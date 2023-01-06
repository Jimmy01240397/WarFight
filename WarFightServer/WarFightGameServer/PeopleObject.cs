using PacketType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WarFightGameServer
{
    public class PeopleObject : MapObject
    {
        public bool walking { get; private set; }
        public bool special_moving { get; private set; }

        float MoveRateMax = 0;
        float MoveRate = 0;
        public PeopleObject(Room room, int OwnerIndex, int x, int y) : base(room, OwnerIndex, x, y)
        {
            type = "People";
            MoveRateMax = 1 / (float)((Dictionary<string, IDictionary>)Program.appllication.DataBase[type.Split('/')[0]])[type.Split('/')[1]]["movespeed"];
            MoveRate = MoveRateMax;
        }

        public override Dictionary<string, object> ObjectToData()
        {
            var data = base.ObjectToData();
            data.Add("walking", walking);
            data.Add("special_moving", special_moving);
            return data;
        }

        public override int Hash()
        {
            return Tools.HashCombine(base.Hash(), Tools.HashCombine(walking.GetHashCode(), special_moving.GetHashCode()));
        }

        public override void Update()
        {
            base.Update();


        }
    }
}