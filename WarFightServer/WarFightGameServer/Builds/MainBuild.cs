using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarFightGameServer.Builds
{
    public class MainBuild : BuildingObject
    {
        public override string type => base.type + "/mainbuild";
        public MainBuild(Room room, int OwnerIndex, int x, int y) : base(room, OwnerIndex, x, y)
        {
        }
    }
}