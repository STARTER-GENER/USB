using System;
using System.Drawing;
using System.IO;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Класс для хранения информации о блоке, предоставляющий функции обработки его текстур.
    /// </summary>
    internal class Block
    {
        /// <summary>
        /// Уникальный номер блока
        /// </summary>
        public readonly short blockID;
        /// <summary>
        /// Является ли блок пустотой
        /// </summary>
        public readonly bool isVoid;
        /// <summary>
        /// Есть ли коллизия у блока
        /// </summary>
        public readonly bool collision;
        /// <summary>
        /// Является ли блок источником света
        /// </summary>
        public readonly bool isLightSourse;
        /// <summary>
        /// Можно ли сломать блок
        /// </summary>
        public readonly bool breakable;
        /// <summary>
        /// Есть ли у блока анимация
        /// </summary>
        public readonly bool animated;
        /// <summary>
        /// Можно ли взаимодействовать с блоком
        /// </summary>
        public readonly bool interactive;
        /// <summary>
        /// Уровень излучаемого света(только если блок исчтоник света)
        /// </summary>
        public readonly byte lightLevel;
        /// <summary>
        /// Прочность блока
        /// </summary>
        public readonly short hardness;

        private string texture_directory;
        private Bitmap[] textures;     
        private byte minLightLevel;

        /// <summary>
        /// Инициализирует обьект с параметрами из файла и загружает текстуры
        /// </summary>
        /// <param name="directory">Директория папки с текстурами</param>
        public Block(string directory)
        {
            PropertyFile file = new PropertyFile(directory + "\\block_properties.pf");
            blockID = (short)file.getIntValue("blockid");
            isVoid = file.getBoolValue("is-void");
            collision = file.getBoolValue("collision");
            isLightSourse = file.getBoolValue("is-light-source");
            breakable = file.getBoolValue("breakable");
            animated = file.getBoolValue("animated");
            interactive = file.getBoolValue("interactive");
            lightLevel = (byte)file.getIntValue("light-level");
            hardness = (short)file.getIntValue("hardness");
            minLightLevel = (byte)file.getIntValue("min-light-level");
            if (!isVoid)
            {
                LoadFromDirectory(directory);
            }
            this.texture_directory = directory;
        }

        private void LoadFromDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException("No directory found : " + directory);
            }
            string[] files = Directory.GetFiles(directory, "*png");
            Console.WriteLine(files.Length);
            if (files.Length == 0)
            {
                throw new DirectoryNotFoundException("Directory is empty : " + directory);
            }
            textures = new Bitmap[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                textures[i] = (Bitmap)Image.FromFile(files[i]);
            }
        }

        /// <summary>
        /// Метод выполняющий масштабирование текстуры блока до опред. размера
        /// </summary>
        /// <param name="textureSize">Размер текстуры</param>
        /// <exception cref="ArgumentException"></exception>
        public void ResizeTextures(short textureSize)
        {
            if (!isVoid)
            {
                LoadFromDirectory(texture_directory);
                if (textureSize == 0)
                {
                    throw new ArgumentException("TextureSize was null");
                }
                for (int i = 0; i < textures.Length; i++)
                {
                    textures[i] = new Bitmap(textures[i], textureSize, textureSize);
                }
            }
        }

        /// <summary>
        /// Метод возвращающий текстуру блока
        /// </summary>
        /// <param name="variant">Вариант текстуры</param>
        /// <param name="brightness">Яркость текстуры</param>
        /// <returns>Текстура блока</returns>
        /// <exception cref="ArgumentException"></exception>
        public Bitmap GetTexture(byte variant, byte brightness)
        {
            if (!isVoid)
            {
                if (variant < 0)
                {
                    throw new ArgumentException("Variant cannot be negative. :" + variant);
                }
                if (variant > textures.Length - 1)
                {
                    throw new ArgumentException("Found " + textures.Length + " variants." + " Not " + (variant + 1) + ".");
                }
                if (brightness < minLightLevel)
                {
                    brightness = minLightLevel;
                }
                if (brightness > 255)
                {
                    brightness = 255;
                }
                if (brightness == 0)
                {
                    return textures[variant];
                }
                Bitmap temp = new Bitmap(textures[variant]);
                Graphics graphics = Graphics.FromImage(temp);
                graphics.FillRectangle(new SolidBrush(Color.FromArgb(brightness, 0, 0, 0)), 0, 0, temp.Width, temp.Height);
                graphics.Dispose();
                return temp;
            }
            else return null;
        }
    }
}
