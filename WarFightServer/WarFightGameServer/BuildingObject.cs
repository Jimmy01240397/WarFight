using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace WarFightGameServer
{
    public class BuildingObject : MapObject
    {
        public override int width
        {
            get
            {
                return (int)((Dictionary<string, IDictionary>)Program.appllication.DataBase[type.Split('/')[0]])[type.Split('/')[1]]["width"];
            }
        }

        public override int height
        {
            get
            {
                return (int)((Dictionary<string, IDictionary>)Program.appllication.DataBase[type.Split('/')[0]])[type.Split('/')[1]]["height"];
            }
        }

        public BuildingObject(Room room, int OwnerIndex, int x, int y) : base(room, OwnerIndex, x, y)
        {
            type = "Builds";
        }

        public override void Update()
        {
            base.Update();


        }
    }
}