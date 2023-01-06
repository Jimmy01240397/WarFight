using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketType
{
    public enum GameServerAndClientResponseType
    {
        Login = 0,
        GetMap,
        GetMapObjects
    }
}