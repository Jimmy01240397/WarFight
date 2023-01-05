using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacketType
{
    public enum LoginServerAndClientRequestType
    {
        Login = 0,
        Logout,
        AddRoom,
        AskRoomData,
        Prepare
    }
}
