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
        private static string path = "../../../../Resources/";
        private static int[,] canva = new int[2000, 2000];

        public HeatMap()
        {
            //LoadCanva();
            LazyLoad();
            //SaveCanva();
            //SavePicture();
        }

        public static void LoadCanva()
        {
            using (StreamReader sr = new StreamReader(path + "2022_place_canvas_history.csv"))
            {
                sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] splitLine = Regex.Split(line, "([\\w-.:\\s]+),([\\w+/=]+),([\\w#]+),\"([\\w,]+)\"");
                    splitLine = new[] {splitLine[1], splitLine[2], splitLine[3], splitLine[4]};
                    string[] coordString = splitLine[splitLine.Length - 1].Split(',');
                    canva[Int32.Parse(coordString[1]), Int32.Parse(coordString[0])] += 1;
                }
            }
        }
        
        public static void SaveCanva()
        {
            using (StreamWriter sr = new StreamWriter(path + "save.csv"))
            {
                for (int y = 0; y < canva.GetLength(0); y++)
                {
                    for (int x = 0; x < canva.GetLength(1); x++)
                    {
                        sr.Write(canva[y, x] + ",");
                    }
                    sr.WriteLine();
                }
            }
        }
        
        public static void LazyLoad()
        {
            using (StreamReader sr = new StreamReader(path + "save.csv"))
            {
                string[] load = sr.ReadToEnd().Split("\n");
                for (int y = 0; y < load.Length - 1; y++)
                {
                    string[] line = load[y].Split(",");
                    for (int x = 0; x < line.Length - 1; x++)
                    {
                        canva[y, x] = Int32.Parse(line[x]);
                    }
                }
            }
        }

        public static void SavePicture()
        {
            Bitmap bitmap = new Bitmap(2000, 2000);

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (canva[y, x] < 10)
                    {
                        bitmap.SetPixel(x, y, Color.Black);
                    }
                    if (canva[y, x] < 50)
                    {
                        bitmap.SetPixel(x, y, Color.Yellow);
                    }
                    else if (canva[y, x] < 100)
                    {
                        bitmap.SetPixel(x, y, Color.Orange);
                    }
                    else if (canva[y, x] < 200)
                    {
                        bitmap.SetPixel(x, y, Color.Red);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, Color.DarkRed);
                    }
                }
            }

            bitmap.Save("../../../../Resources/test.bmp");
        }
    }
}