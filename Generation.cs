using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerraiaMoment
{
    internal class Generation
    {
        public BlockInfo[,] map;


        public BlockInfo[,] getMap()
        {
            return map;
        }

        public void generateMap(int mapSizeX, int mapSizeY, string fileName)
        {
            map = new BlockInfo[mapSizeY, mapSizeX];

            PropertyFile file = new PropertyFile(fileName);

            Random rand = new Random();

            //Console.WriteLine(file.getIntValue("id"));

            Perlin2D perlinNoise = new Perlin2D(rand.Next(0, 1000));

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j] = new BlockInfo(0, 0);
                }
            }


            for (int x = 0; x < map.GetLength(1); x++)
            {
                float sampleX = x / 65f;
                float heightValue = perlinNoise.Noise(sampleX, 0) * 46 + 29;

                map[(int)heightValue, x] = new BlockInfo(1, 5); //блоки травы  
            }
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int st = getY(j) + 1;
                if (rand.Next(2) == 1)
                {
                    if (rand.Next(10) == 1)
                    {
                        map[st - 2, j] = new BlockInfo(5, (byte)rand.Next(2)); //цветы

                    }
                    else
                    {
                        map[st - 2, j] = new BlockInfo(3, (byte)rand.Next(4)); //трава
                    }
                    if (rand.Next(10) == 1)
                    {
                        int treeX = j;
                        int treeY = st - 2;
                        if (treeX > 0 && treeX < map.GetLength(0) && treeY - 18 > 0)
                        {
                            generateTree(treeX, treeY); //деревья
                        }
                    }
                }
                int kl = 4 + rand.Next(2) + st;
                for (int f = st; f < kl; f++)
                {
                    map[f, j] = new BlockInfo(1, (byte)rand.Next(4)); //блоки грязи
                }
                for (int f = kl; f < map.GetLength(0); f++)
                {
                    map[f, j] = new BlockInfo(2, (byte)rand.Next(4)); //блоки камня
                }
            }
            int max = map.GetLength(0);
            for (int i = 0; i < map.GetLength(1); i++)
            {
                if (getY(i) < max)
                {
                    max = getY(i);
                }
            }

            //пещеры

            int caveX = 0;
            int caveY = 0;
            int rad = rand.Next(file.getIntValue("minRad"), file.getIntValue("maxRad"));

            for (int i = 0; i < map.GetLength(1); i++)
            {
                if (getY(i) == max)
                {
                    caveX = i;
                    caveY = max;
                }
            }
            for (int i = 0; i < rand.Next(50, 101); i++)
            {
                generateCavePart(caveX, caveY, rad);

                int caveMove = rand.Next(2);
                int caveShiftX = rand.Next(file.getIntValue("CaveShiftX"));
                if (caveMove == 1)
                {
                    caveX += caveShiftX;
                }
                else
                {
                    caveX -= caveShiftX;
                }
                if (i < 75)
                {
                    caveY += rand.Next(5);

                }
                else
                {
                    caveY++;

                }

            }

            //руда
            int OreRad = rand.Next(file.getIntValue("MinOreRad"), file.getIntValue("MaxOreRad"));

            int minHeight = file.getIntValue("minHeight");

            int Distance = rand.Next(file.getIntValue("minDistance"), file.getIntValue("maxDistance"));

            int OreX = 0;
            int OreY = 0;

            for (int i = OreRad; i < map.GetLength(1) - OreRad; i++)
            {
                if (rand.Next(2) == 1)
                {
                    if (OreX < i - Distance)
                    {
                        OreX = i;
                        OreY = rand.Next((minHeight + OreRad), map.GetLength(0) - OreRad);
                        generateOre(OreX, OreY, OreRad);
                        Distance = rand.Next(10, 25);

                        //спавн руды OreX, OreY
                    }
                }
            }
        }



        public void generateCavePart(int caveX, int caveY, int rad)
        {
            int stx = caveX - rad;
            int sty = caveY - rad;
            int fx = caveX + rad;
            int fy = caveY + rad;

            stx = stx < 0 ? 0 : stx;
            sty = sty < 0 ? 0 : sty;
            fx = fx > map.GetLength(1) - 1 ? map.GetLength(1) - 1 : fx;
            fy = fy > map.GetLength(0) - 1 ? map.GetLength(0) - 1 : fy;
            for (int k = stx; k <= fx; k++)
            {
                for (int m = sty; m <= fy; m++)
                {
                    if (map[m, k].blockID != 0)
                    {
                        double dist = getDistanceRad(caveX, caveY, k, m);
                        if (dist < rad)
                        {
                            map[m, k] = new BlockInfo(0, 0);
                        }
                    }
                }
            }
        }

        public void generateTree(int x, int y)
        {
            Random rand = new Random();
            for (int i = 0; i < 15; i++)
            {
                y--;
                map[y + 1, x] = new BlockInfo(6, 0);

            }
            for (int j = 0; j < 3; j++)
            {
                int posBranch = rand.Next(2);
                if (posBranch == 1)
                {
                    map[y + rand.Next(6) + 2, x - 1] = new BlockInfo(6, 2);
                }
                else
                {
                    map[y + rand.Next(6) + 2, x + 1] = new BlockInfo(6, 1);
                }
            }
        }

        public void generateOre(int x, int y, int rad)
        {
            int stx = x - rad;
            int sty = y - rad;
            int fx = x + rad;
            int fy = y + rad;

            stx = stx < 0 ? 0 : stx;
            sty = sty < 0 ? 0 : sty;
            fx = fx > map.GetLength(1) - 1 ? map.GetLength(1) - 1 : fx;
            fy = fy > map.GetLength(0) - 1 ? map.GetLength(0) - 1 : fy;
            for (int k = stx; k <= fx; k++)
            {
                for (int m = sty; m <= fy; m++)
                {
                    if (map[m, k].blockID != 0)
                    {
                        int dist = (int)getDistanceRad(x, y, k, m);
                        if (dist < rad)
                        {
                            map[m, k] = new BlockInfo(7, 0);
                        }
                    }
                }
            }
        }

        double getDistanceRad(int x, int y, int tx, int ty)
        {
            return Math.Abs(Math.Sqrt(Math.Pow(tx - x, 2) + Math.Pow(ty - y, 2)));
        }

        public int getY(int x)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                if (map[i, x].blockID == 1)
                {
                    return i;
                }
            }
            return 0;
        }
        class Perlin2D
        {
            byte[] permutationTable;

            public Perlin2D(int seed = 0)
            {
                var rand = new System.Random(seed);
                permutationTable = new byte[1024];
                rand.NextBytes(permutationTable);
            }

            private float[] GetPseudoRandomGradientVector(int x, int y)
            {
                int v = (int)(((x * 1836311903) ^ (y * 2971215073) + 4807526976) & 1023);
                v = permutationTable[v] & 3;

                switch (v)
                {
                    case 0: return new float[] { 1, 0 };
                    case 1: return new float[] { -1, 0 };
                    case 2: return new float[] { 0, 1 };
                    default: return new float[] { 0, -1 };
                }
            }

            static float QunticCurve(float t)
            {
                return t * t * t * (t * (t * 6 - 15) + 10);
            }

            static float Lerp(float a, float b, float t)
            {
                return a + (b - a) * t;
            }

            static float Dot(float[] a, float[] b)
            {
                return a[0] * b[0] + a[1] * b[1];
            }

            public float Noise(float fx, float fy)
            {
                int left = (int)System.Math.Floor(fx);
                int top = (int)System.Math.Floor(fy);
                float pointInQuadX = fx - left;
                float pointInQuadY = fy - top;

                float[] topLeftGradient = GetPseudoRandomGradientVector(left, top);
                float[] topRightGradient = GetPseudoRandomGradientVector(left + 1, top);
                float[] bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1);
                float[] bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1);

                float[] distanceToTopLeft = new float[] { pointInQuadX, pointInQuadY };
                float[] distanceToTopRight = new float[] { pointInQuadX - 1, pointInQuadY };
                float[] distanceToBottomLeft = new float[] { pointInQuadX, pointInQuadY - 1 };
                float[] distanceToBottomRight = new float[] { pointInQuadX - 1, pointInQuadY - 1 };

                float tx1 = Dot(distanceToTopLeft, topLeftGradient);
                float tx2 = Dot(distanceToTopRight, topRightGradient);
                float bx1 = Dot(distanceToBottomLeft, bottomLeftGradient);
                float bx2 = Dot(distanceToBottomRight, bottomRightGradient);

                pointInQuadX = QunticCurve(pointInQuadX);
                pointInQuadY = QunticCurve(pointInQuadY);

                float tx = Lerp(tx1, tx2, pointInQuadX);
                float bx = Lerp(bx1, bx2, pointInQuadX);
                float tb = Lerp(tx, bx, pointInQuadY);

                return tb;
            }

            public float Noise(float fx, float fy, int octaves, float persistence = 0.5f)
            {
                float amplitude = 1;
                float max = 0;
                float result = 0;

                while (octaves-- > 0)
                {
                    max += amplitude;
                    result += Noise(fx, fy) * amplitude;
                    amplitude *= persistence;
                    fx *= 2;
                    fy *= 2;
                }

                return result / max;
            }
        }
    }
}
