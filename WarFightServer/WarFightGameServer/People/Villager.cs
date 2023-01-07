using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarFightGameServer.People
{
    public class Villager:PeopleObject
    {
        public override string type => base.type + "/villager";
        public Villager(Room room, int OwnerIndex, int x, int y) : base(room, OwnerIndex, x, y)
        {
        }
    }
}