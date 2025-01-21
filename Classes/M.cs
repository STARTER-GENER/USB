using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerrariaGame
{
    public class M
    {
        public static void SetBlock(int x, int y, short blockID, byte variant)
        {
            if (x < 0 || x > GameValues.map.GetLength(1) - 1)
            {
                throw new ArgumentException("Position X is out of range");
            }
            if (y < 0 || y > GameValues.map.GetLength(0) - 1)
            {
                throw new ArgumentException("Position Y is out of range");
            }
            if (blockID > GameValues.blocks.Length - 1)
            {
                throw new ArgumentException($"Block with id = {blockID} does not exist");
            }
            if (variant > GameValues.blocks[blockID].maxVariants - 1)
            {
                throw new ArgumentException($"Variant {variant} does not exist");
            }
            GameValues.map[y, x] = new BlockData(blockID, variant);
        }

        public static void RemoveBlock(int x, int y)
        {
            if (x < 0 || x > GameValues.map.GetLength(1) - 1)
            {
                throw new ArgumentException("Position X is out of range");
            }
            if (y < 0 || y > GameValues.map.GetLength(0) - 1)
            {
                throw new ArgumentException("Position Y is out of range");
            }
            GameValues.map[y, x] = new BlockData(GameValues.voidBlockID, 0);
        }
    }
}
