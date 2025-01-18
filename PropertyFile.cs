using System;
using System.Collections.Generic;
using System.IO;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Класс для работы с файлом параметров
    /// </summary>
    internal class PropertyFile
    {
        [Serializable]
        private class PropertyFileReadExeption : Exception
        {
            public PropertyFileReadExeption() : base() { }
            public PropertyFileReadExeption(string message) : base(message) { }
            public PropertyFileReadExeption(string message, Exception inner) : base(message, inner) { }
        }

        private struct Element
        {
            public string name;
            public string value;
            public Element(string name, string value)
            {
                this.name = name;
                this.value = value;
            }
        }

        private Element[] elements;

        /// <summary>
        /// Считывает из файла все элементы и инициализирует обьект
        /// </summary>
        /// <param name="filename">Имя или Путь к файлу</param>
        /// <exception cref="PropertyFileReadExeption"></exception>
        public PropertyFile(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException($"File not found: {filename}");
            }
            string fileContent = File.ReadAllText(filename);
            if (fileContent.Length == 0)
            {
                throw new PropertyFileReadExeption("File is empty.");
            }
            List<string> lines = new List<string>();
            bool canAdd = false;
            bool comment = false;
            string element = "";
            for (int i = 0; i < fileContent.Length; i++)
            {
                if (fileContent[i] == '[')
                {
                    if (!canAdd)
                    {
                        canAdd = true;
                        if (i != fileContent.Length - 1)
                        {
                            i++;
                        }
                    }
                    else
                    {
                        throw new PropertyFileReadExeption("Wrong syntax.");
                    }
                }
                if (fileContent[i] == ']' && canAdd)
                {
                    if (element.Length == 0)
                    {
                        throw new PropertyFileReadExeption("Wrong syntax.");
                    }
                    canAdd = false;
                    lines.Add(element);
                    element = "";
                }
                if (fileContent[i]=='<' && !comment)
                {
                    comment = true;
                    if (i != fileContent.Length - 1)
                    {
                        i++;
                    }
                }
                if (fileContent[i] == '>' && comment)
                {
                    comment = false;
                    if (i != fileContent.Length - 1)
                    {
                        i++;
                    }
                }
                if (fileContent[i] != ' ' && canAdd && !comment)
                {
                    element += fileContent[i];
                }
            }
            if (canAdd)
            {
                throw new PropertyFileReadExeption("Wrong syntax.");
            }
            elements = new Element[lines.Count];
            for (int i = 0; i < elements.Length; i++)
            {
                string[] oneElement = lines[i].Split(':');
                if (oneElement.Length < 2)
                {
                    throw new PropertyFileReadExeption("Wrong element syntax.");
                }
                elements[i] = new Element(oneElement[0], oneElement[1]);
            }
        }

        /// <summary>
        /// Возвращает значение параметра
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <returns>Значение параметра типа int.</returns>
        /// <exception cref="ArgumentException"></exception>
        public int getIntValue(string paramName)
        {
            foreach (Element element in elements)
            {
                if (element.name == paramName)
                {
                    return int.Parse(element.value);
                }
            }
            throw new ArgumentException($"Parameter named: {paramName} does not exist.");
        }

        /// <summary>
        /// Возвращает значение параметра
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <returns>Значение параметра типа bool.</returns>
        /// <exception cref="ArgumentException"></exception>
        public bool getBoolValue(string paramName)
        {
            foreach (Element element in elements)
            {
                if (element.name == paramName)
                {
                    return bool.Parse(element.value);
                }
            }
            throw new ArgumentException($"Parameter named: {paramName} does not exist.");
        }

        /// <summary>
        /// Возвращает значение параметра
        /// </summary>
        /// <param name="paramName">Имя параметра</param>
        /// <returns>Значение параметра типа string.</returns>
        /// <exception cref="ArgumentException"></exception>
        public string getStringValue(string paramName)
        {
            foreach (Element element in elements)
            {
                if (element.name == paramName)
                {
                    return element.value;
                }
            }
            throw new ArgumentException($"Parameter named: {paramName} does not exist.");
        }
    }
}
