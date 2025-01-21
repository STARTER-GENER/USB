using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerrariaGame
{
    public class GameValues
    {
        public static readonly short voidBlockID = -1;

        public static Block[] blocks;

        public static BlockData[,] map;

        public static int tileSize;

        public static byte DefaulLightLevel = 0;

        public static bool[] keys = new bool[4];
    }
}
