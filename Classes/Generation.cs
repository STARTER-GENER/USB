using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TerrariaGame;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace TerraiaGame
{
    public class Generation
    {

        Random rand = new Random();

        public void generateMap(string fileName)
        {

            PropertyFile file = new PropertyFile(fileName);

            Perlin2D perlinNoise = new Perlin2D(rand.Next(0, 1000));

            for (int x = 0; x < M.getXsize(); x++)
            {
                float sampleX = x / 65f;
                float heightValue = perlinNoise.Noise(sampleX, 0) * 46 + 29;
                M.SetBlock(x, (int)heightValue, 7, 0);//блоки травы
                M.SetBlock(x, (int)heightValue + 1, 7, 0);//блоки травы
            }
                for (int j = 0; j < M.getXsize(); j++)
                {
                    int st = getY(j) + 1;
                    if (rand.Next(2) == 1)
                    {
                        if (rand.Next(10) > 7)
                        {
                            M.SetBlock(j, st-2, 5, (byte)rand.Next(2));//цветы
                        }
                        else
                        {
                            M.SetBlock(j, st-2, 3, (byte)rand.Next(8));//трава
                        }
                    }
                    int kl = 4 + rand.Next(2) + st;
                    for (int f = st; f < kl; f++)
                    {
                        M.SetBlock(j, f + 1, 1, (byte)rand.Next(4));//блоки грязи
                    }
                    for (int f = kl; f < M.getYsize(); f++)
                    {
                        M.SetBlock(j, f, 2, (byte)rand.Next(4));//блоки камня
                    }
                }

                //генерация вкраплений земли(для разнообразия тёмных не то чтобы глубин)

                int max = M.getYsize();
                for (int i = 0; i < M.getXsize(); i++)
                {
                    if (getY(i) < max)
                    {
                        max = getY(i);
                    }
                }
                int DirtRad = rand.Next(file.getIntValue("MinDirtRad"), file.getIntValue("MaxDirtRad"));

                int minDirtHeight = file.getIntValue("minDirtHeight");
                int DirtDistance = rand.Next(file.getIntValue("minDirtDistance"), file.getIntValue("maxDirtDistance"));
                int DirtX = 0;
                int DirtY = 0;

                for (int i = DirtRad; i < M.getXsize() - DirtRad; i++)
                {
                    if (DirtX < i - DirtDistance)
                    {
                        DirtX = i;
                        DirtY = rand.Next((minDirtHeight + DirtRad), M.getYsize() - DirtRad);
                        for (int j = 0; j < rand.Next(20); j++)
                        {
                            generateDirt(DirtX, DirtY, DirtRad);
                            int DirtShift = file.getIntValue("DirtShift");
                            if (rand.Next(4) == 1)
                            {
                                DirtX += DirtShift;
                                DirtY += DirtShift;
                            }
                            else if (rand.Next(4) == 2)
                            {
                                DirtX -= DirtShift;
                                DirtY -= DirtShift;
                            }
                            else if (rand.Next(4) == 3)
                            {
                                DirtX += DirtShift;
                                DirtY -= DirtShift;
                            }
                            else if (rand.Next(4) == 4)
                            {
                                DirtX -= DirtShift;
                                DirtY += DirtShift;
                            }
                        }
                        DirtDistance = rand.Next(file.getIntValue("minDirtDistance"), file.getIntValue("maxDirtDistance"));
                    }

                }
                //

                //генерация руды
                for (int i = 0; i < M.getXsize(); i++)
                {
                    if (getY(i) < max)
                    {
                        max = getY(i);
                    }
                }
                int OreRad = rand.Next(file.getIntValue("MinOreRad"), file.getIntValue("MaxOreRad"));

                int minHeight = file.getIntValue("minHeight");

                int Distance = rand.Next(file.getIntValue("minDistance"), file.getIntValue("maxDistance"));
                int typeOre = 0;
                int OreX = 0;
                int OreY = 0;

                for (int i = OreRad; i < M.getXsize() - OreRad; i++)
                {
                    if (rand.Next(2) == 1)
                    {
                        if (OreX < i - Distance)
                        {
                            OreX = i;
                            OreY = rand.Next((minHeight + OreRad), M.getYsize() - OreRad);
                            typeOre = rand.Next(8, 11);
                            generateOre(OreX, OreY, OreRad, typeOre);
                            Distance = rand.Next(file.getIntValue("minDistance"), file.getIntValue("maxDistance"));
                            //центр руды OreX, OreY
                        }
                    }
                }

                //генерация пещер

                int caveX = 0;
                int caveY = 0;
                int rad = rand.Next(file.getIntValue("minRad"), file.getIntValue("maxRad"));

                int minHeightCave = file.getIntValue("minHeightCave");
                int CaveDistance = rand.Next(file.getIntValue("minCaveDistance"), file.getIntValue("maxCaveDistance"));

                for (int i = rad; i < M.getXsize() - rad; i++)
                {

                    if (caveX < i - CaveDistance)
                    {
                        caveX = i;
                        caveY = rand.Next((minHeightCave + rad), M.getYsize() - rad);
                        for (int j = 0; j < rand.Next(50, 101); j++)
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
                            if (caveY < 75)
                            {
                                caveY += rand.Next(5);
                            }
                            else
                            {
                                caveY++;
                            }
                            CaveDistance = rand.Next(file.getIntValue("minCaveDistance"), file.getIntValue("maxCaveDistance"));
                        }
                    }

                }
                //

                //генерация деревьев
                for (int j = 0; j < M.getXsize(); j += file.getIntValue("TreeDistance"))
                {
                    int heightTree;
                    int st = getY(j) + 1;
                    if (rand.Next(5) == 1)
                    {
                        int treeX = j;
                        int treeY = st - 2;
                        heightTree = rand.Next(6, 16);
                        if (treeX > 1 && treeX < M.getXsize() - 2 && treeY > 20)
                        {
                            generateTree(treeX, treeY, heightTree);
                        }
                    }
                }
            grass_connection();//соединение блоков травы с целью улучшения визуальной части игры
        }


        public void generateCavePart(int caveX, int caveY, int rad)
        {
            int stx = caveX - rad;
            int sty = caveY - rad;
            int fx = caveX + rad;
            int fy = caveY + rad;

            stx = stx < 0 ? 0 : stx;
            sty = sty < 0 ? 0 : sty;
            fx = fx > M.getXsize() - 1 ? M.getXsize() - 1 : fx;
            fy = fy > M.getYsize() - 1 ? M.getYsize() - 1 : fy;
            for (int k = stx; k <= fx; k++)
            {
                for (int m = sty; m <= fy; m++)
                {
                    if (M.GetBlockID(k, m) != 0)
                    {
                        double dist = getDistanceRad(caveX, caveY, k, m);

                        if (dist < rad)
                        {
                            M.SetBlock(k, m, 0, 0);
                        }


                        if (dist == rad && caveY < 50)
                        {
                            M.SetBlock(k, m, 1, 0);
                        }
                    }
                }
            }
        }

        public void generateTree(int x, int y, int heightTree)
        {
            int HeightWoodY = 0;
            int HeightWoodX = 0;
            M.SetBlock(x, y + 1, 1, 0);
            for (int i = 0; i < heightTree; i++)
            {
                y--;
                M.SetBlock(x, y + 1, 6, 0);
                HeightWoodY = y;
                HeightWoodX = x - 3;
            }
            if (heightTree > 10)//высокие деревья
            {
                M.SetBlock(HeightWoodX, HeightWoodY, 6, 3);
                M.SetBlock(HeightWoodX, HeightWoodY - 1, 6, 4);
                for (int k = 0; k < 5; k++)
                {
                    M.SetBlock(HeightWoodX + 1, HeightWoodY, 6, 5);
                    M.SetBlock(HeightWoodX + 1, HeightWoodY - 1, 6, 5);
                    HeightWoodX++;
                }
                M.SetBlock(HeightWoodX + 1, HeightWoodY, 6, 7);
                M.SetBlock(HeightWoodX + 1, HeightWoodY - 1, 6, 6);
                M.SetBlock(HeightWoodX, HeightWoodY - 2, 6, 6);
                M.SetBlock(HeightWoodX - 1, HeightWoodY - 2, 6, 5);
                M.SetBlock(HeightWoodX - 2, HeightWoodY - 2, 6, 5);
                M.SetBlock(HeightWoodX - 3, HeightWoodY - 2, 6, 5);
                M.SetBlock(HeightWoodX - 4, HeightWoodY - 2, 6, 4);
                M.SetBlock(HeightWoodX - 2, HeightWoodY - 3, 6, 5);
                M.SetBlock(HeightWoodX - 3, HeightWoodY - 3, 6, 4);
                M.SetBlock(HeightWoodX - 1, HeightWoodY - 3, 6, 6);
                for (int j = 0; j < 4; j++)
                {
                    int posBranch = rand.Next(2);
                    if (posBranch == 1)
                    {
                        M.SetBlock(x - 1, y + rand.Next(10) + 1, 6, 2);
                    }
                    else
                    {
                        M.SetBlock(x + 1, y + rand.Next(10) + 1, 6, 1);
                    }
                }
            }
            else if (heightTree <= 10 && heightTree > 7)//средние деревья
            {

                for (int k = 0; k < 3; k++)
                {
                    M.SetBlock(HeightWoodX + 2, HeightWoodY, 6, 5);
                    M.SetBlock(HeightWoodX + 2, HeightWoodY - 1, 6, 5);
                    HeightWoodX++;
                }
                M.SetBlock(HeightWoodX + 2, HeightWoodY, 6, 7);
                M.SetBlock(HeightWoodX + 2, HeightWoodY - 1, 6, 6);
                M.SetBlock(HeightWoodX - 2, HeightWoodY, 6, 3);
                M.SetBlock(HeightWoodX - 2, HeightWoodY - 1, 6, 4);
                M.SetBlock(HeightWoodX - 1, HeightWoodY - 2, 6, 4);
                M.SetBlock(HeightWoodX, HeightWoodY - 2, 6, 5);
                M.SetBlock(HeightWoodX + 1, HeightWoodY - 2, 6, 6);

                for (int j = 0; j < 3; j++)
                {
                    int posBranch = rand.Next(2);
                    if (posBranch == 1)
                    {
                        M.SetBlock(x - 1, y + rand.Next(5) + 1, 6, 2);
                    }
                    else
                    {
                        M.SetBlock(x + 1, y + rand.Next(5) + 1, 6, 1);
                    }
                }
            }
            else if (heightTree <= 7)//маленькие деревья
            {
                M.SetBlock(HeightWoodX + 2, HeightWoodY, 6, 3);
                M.SetBlock(HeightWoodX + 2, HeightWoodY - 1, 6, 4);

                M.SetBlock(HeightWoodX + 3, HeightWoodY - 1, 6, 5);
                M.SetBlock(HeightWoodX + 3, HeightWoodY, 6, 5);

                M.SetBlock(HeightWoodX + 4, HeightWoodY - 1, 6, 6);
                M.SetBlock(HeightWoodX + 4, HeightWoodY, 6, 7);

                for (int j = 0; j < 2; j++)
                {
                    int posBranch = rand.Next(2);
                    if (posBranch == 1)
                    {
                        M.SetBlock(x - 1, y + rand.Next(3) + 1, 6, 2);
                    }
                    else
                    {
                        M.SetBlock(x + 1, y + rand.Next(3) + 1, 6, 1);
                    }
                }
            }
        }

        public void generateOre(int x, int y, int rad, int type)
        {
            int stx = x - rad;
            int sty = y - rad;
            int fx = x + rad;
            int fy = y + rad;

            stx = stx < 0 ? 0 : stx;
            sty = sty < 0 ? 0 : sty;
            fx = fx > M.getXsize() - 1 ? M.getXsize() - 1 : fx;
            fy = fy > M.getYsize() - 1 ? M.getYsize() - 1 : fy;
            for (int k = stx; k <= fx; k++)
            {
                for (int m = sty; m <= fy; m++)
                {
                    if (M.GetBlockID(k, m) != 0)
                    {
                        int dist = (int)getDistanceRad(x, y, k, m);
                        if (dist < rad)
                        {
                            if (rand.Next(8) == 1)
                            {
                                M.SetBlock(k, m, 2, (byte)rand.Next(4));
                            }
                            else
                            {
                                M.SetBlock(k, m, (byte)type, 0);
                            }
                        }
                    }
                }
            }
        }

        public void generateDirt(int x, int y, int rad)
        {
            int stx = x - rad;
            int sty = y - rad;
            int fx = x + rad;
            int fy = y + rad;

            stx = stx < 0 ? 0 : stx;
            sty = sty < 0 ? 0 : sty;
            fx = fx > M.getXsize() - 1 ? M.getXsize() - 1 : fx;
            fy = fy > M.getYsize() - 1 ? M.getYsize() - 1 : fy;
            for (int k = stx; k <= fx; k++)
            {
                for (int m = sty; m <= fy; m++)
                {
                    if (M.GetBlockID(k, m) != 0)
                    {
                        double dist = (int)getDistanceRad(x, y, k, m);
                        if (dist < rad)
                        {
                            M.SetBlock(k, m, 1, (byte)rand.Next(4));
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
            for (int i = 0; i < M.getYsize(); i++)
            {
                if (M.GetBlockID(x, i) == 7)
                {
                    return i;
                }
            }
            return 0;
        }

        public bool getBool(int i, int j)
        {
            return M.GetBlockID(j, i) == 7 || M.GetBlockID(j, i) == 1;
        }

        public void grass_connection()
        {
            for (int i = 0; i < M.getYsize(); i++)
            {
                for (int j = 0; j < M.getXsize(); j++)
                {
                    if (M.GetBlockID(j, i) == 7)
                    {
                        bool r = j + 1 < M.getXsize() && getBool(i, j + 1) ? true : false; // прямые 4 точки                       
                        bool l = j - 1 >= 0 && getBool(i, j - 1) ? true : false;
                        bool t = i - 1 >= 0 && getBool(i - 1, j) ? true : false;
                        bool b = i + 1 < M.getYsize() && getBool(i + 1, j) ? true : false; //
                        r = j + 1 == M.getXsize() ? true : r; // условие для границ     //
                        l = j - 1 < 0 ? true : l;   //
                        t = i - 1 < 0 ? true : t;   //
                        b = i + 1 == M.getYsize() ? true : b; //

                        bool tr = j + 1 < M.getXsize() && i - 1 >= 0 && getBool(i - 1, j + 1) ? true : false; // диагональные точки
                        bool tl = j - 1 >= 0 && i - 1 >= 0 && getBool(i - 1, j - 1) ? true : false;
                        bool br = j + 1 < M.getXsize() && i + 1 < M.getYsize() && getBool(i + 1, j + 1) ? true : false;
                        bool bl = j - 1 >= 0 && i + 1 < M.getYsize() && getBool(i + 1, j - 1) ? true : false; //
                        tr = j + 1 == M.getXsize() || i - 1 < 0 ? true : tr; // условие диагональных границ
                        tl = j - 1 < 0 || i - 1 < 0 ? true : tl;
                        br = j + 1 == M.getXsize() || i + 1 == M.getYsize() ? true : br;
                        bl = j - 1 < 0 || i + 1 == M.getYsize() ? true : bl;

                        if (r && b && !t && !l)
                        {
                            if (r && (b || bl) && !tl)
                            {
                                M.SetBlock(j, i, 7, 3);
                            }
                        }
                        if (l && b && !t && !r)
                        {
                            if (l && (b || br) && !tr)
                            {
                                M.SetBlock(j, i, 7, 4);
                            }
                        }
                        if (r && l)
                        {
                            M.SetBlock(j, i, 7, 0);

                        }
                        if (t && b)
                        {
                            if (t && b && r)
                            {
                                M.SetBlock(j, i, 7, 2);
                            }
                            if (t && b && l)
                            {
                                M.SetBlock(j, i, 7, 5);
                            }
                        }
                        if (r && l && (!tl || !tr))
                        {

                            if (l && t && !tl)
                            {
                                M.SetBlock(j, i, 7, 1);
                            }
                            if (r && t && !tr)
                            {
                                M.SetBlock(j, i, 7, 6);
                            }
                        }
                        if (r && l && t && b && tl && tr)
                        {
                            M.SetBlock(j, i, 1, 0); //блоки грязи
                        }
                    }
                }
            }
        }
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

