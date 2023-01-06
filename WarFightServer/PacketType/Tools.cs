using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketType
{
    public static class Tools
    {
        public static int HashCombine(int h1, int h2)
        {
            uint num = (uint)((h1 << 5) | (int)((uint)h1 >> 27));
            return ((int)num + h1) ^ h2;
        }
    }
}