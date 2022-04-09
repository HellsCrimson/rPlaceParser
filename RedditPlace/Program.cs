using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;

namespace RedditPlace
{
    class Program
    {
        static void Main(string[] args)
        {
            HeatMap heatMap = new HeatMap();
        }
    }

    class HeatMap
    {
        private static string path = "../../../../Resources/2022_place_canvas_history.csv";
        private static int[,] canva = new int[2000, 2000];

        public HeatMap()
        {
            //Load();
            SavePicture();
        }

        public static void Load()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] splitLine = Regex.Split(line, "([\\w-.:\\s]+),([\\w+/=]+),([\\w#]+),\"([\\w,]+)\"");
                    splitLine = new[] {splitLine[1], splitLine[2], splitLine[3], splitLine[4]};
                    string[] coordString = splitLine[splitLine.Length - 1].Split(',');
                    canva[Int32.Parse(coordString[0]), Int32.Parse(coordString[1])] += 1;
                }
            }
        }

        public static void SavePicture()
        {
            Bitmap bitmap = new Bitmap(2000, 2000);

            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    if (x % 2 == 0)
                    {
                        bitmap.SetPixel(x, y, Color.Red);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, Color.Blue);
                    }
                }
            }

            bitmap.Save("test.bmp");
        }
    }
}