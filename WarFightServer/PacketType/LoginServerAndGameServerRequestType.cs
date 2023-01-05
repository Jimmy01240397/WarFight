using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketType
{
    public enum LoginServerAndGameServerRequestType
    {
        Login = 0,
        AskUserRoomNum,
        EndGame
    }
}