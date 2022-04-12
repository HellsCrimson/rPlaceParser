using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace RedditPlaceParser
{
    class Program
    {
        static void Main(string[] args)
        {
            HeatMapGif heatMapGif = new HeatMapGif();
            heatMapGif.FillTimeline();
            heatMapGif.CanvaToPicture();
        }
    }

    class Preprocess
    {
        private static string path = "../../../../Resources/";
        
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
                    
                    //HeatMap(Int32.Parse(coordString[1]), Int32.Parse(coordString[0]));
                }
            }
        }
        
        public static void ReSaveOpti()
        {
            using (StreamReader sr = new StreamReader(path + "2022_place_canvas_history.csv"))
            {
                using (StreamWriter sw = new StreamWriter(path + "re_saved.csv"))
                {
                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        string[] splitLine = Regex.Split(line, "([\\w-.:\\s]+),([\\w+/=]+),([\\w#]+),\"([\\w,]+)\"");
                        splitLine = new[] {splitLine[1], splitLine[2], splitLine[3], splitLine[4]};
                        string[] coordString = splitLine[splitLine.Length - 1].Split(',');
                        string[] time = Regex.Split(splitLine[0], "([.]*) UTC");
                        long timeSeconds = DateTimeOffset.Parse(time[0]).ToUnixTimeSeconds();
                        sw.WriteLine(timeSeconds + "|" + splitLine[2] + "|" + coordString[0] + "|" + coordString[1]);
                    }
                }
            }
        }
        
        public static IEnumerable<string> SortedList()
        {
            List<string> list = new List<string>();
            using (StreamReader sr = new StreamReader(path + "re_saved.csv"))
            {
                while (!sr.EndOfStream)
                {
                    list.Add(sr.ReadLine());
                }
            }
            return list.OrderBy(item => Int32.Parse(item.Split("|")[0]));
        }

        public static void SaveOrdered(IEnumerable<string> list)
        {
            using (StreamWriter sw = new StreamWriter(path + "ordered.csv"))
            {
                foreach (string s in list)
                {
                    sw.WriteLine(s);
                }
            }
        }
    }

    class HeatMapGif
    {
        private string path = "../../../../Resources/";
        private int index = 0;
        private int[,] canva = new int[2000, 2000];

        public void FillTimeline()
        {
            int minute = 0;
            int prevMinute = -1;
            using (StreamReader sr = new StreamReader(path + "ordered.csv"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] splitLine = line.Split("|");
                    minute = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse(splitLine[0])).Minute;
                    if (CompareTime(minute, prevMinute))
                    {
                        prevMinute = minute;
                        CanvaToPicture();
                        index++;
                        canva = new int[2000, 2000];
                    }
                    TemporalHeatMap(Int32.Parse(splitLine[2]), Int32.Parse(splitLine[3]));
                }
            }
        }
        
        public void TemporalHeatMap(int x, int y)
        {
            canva[y, x] += 1;
        }

        public void CanvaToPicture()
        {
            Bitmap bitmap = new Bitmap(2000, 2000);

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (canva[y, x] < 4)
                    {
                        bitmap.SetPixel(x, y, Color.Black);
                    }
                    else if (canva[y, x] < 8)
                    {
                        bitmap.SetPixel(x, y, Color.Yellow);
                    }
                    else if (canva[y, x] < 12)
                    {
                        bitmap.SetPixel(x, y, Color.Orange);
                    }
                    else if (canva[y, x] < 15)
                    {
                        bitmap.SetPixel(x, y, Color.Red);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, Color.DarkRed);
                    }
                }

            }

            bitmap.Save($"{path}/toGif/HeatMapGIF{index:000}.bmp");
            bitmap.Dispose();
        }

        public bool CompareTime(int minute, int prevMinute)
        {
            if (prevMinute + 5 < 60)
            {
                if (minute >= prevMinute + 5)
                    return true;
            }
            else
            {
                if (minute < 54 && minute >= 60 - prevMinute + 5)
                    return true;
            }

            return false;
        }
    }

    class HeatMapSimple
    {
        private string path = "../../../../Resources/";
        private int[,] canva = new int[2000, 2000];
        private int[,,] canvaDim = new int[2000, 2000, 86];

        public void LoadLazy()
        {
            int index = 0;
            int hour = 0;
            int prevHour = -1;
            using (StreamReader sr = new StreamReader(path + "ordered.csv"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string[] splitLine = line.Split("|");
                    hour = DateTimeOffset.FromUnixTimeSeconds(Int64.Parse(splitLine[0])).Hour;
                    if (hour != prevHour)
                    {
                        index++;
                        prevHour = hour;
                    }
                    //HeatMap(Int32.Parse(splitLine[2]), Int32.Parse(splitLine[3]));
                    TemporalHeatMap(index, Int32.Parse(splitLine[2]), Int32.Parse(splitLine[3]));
                }
            }

            PictureHeatMapMultiDim();
        }

        public void SaveCanva()
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
        
        public void LazyLoad(string filename)
        {
            using (StreamReader sr = new StreamReader(path + filename))
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

        public void HeatMap(int x, int y)
        {
            canva[y, x] += 1;
        }

        public void PictureHeatMap()
        {
            Bitmap bitmap = new Bitmap(2000, 2000);

            for (var y = 0; y < bitmap.Height; y++)
            {
                for (var x = 0; x < bitmap.Width; x++)
                {
                    if (canva[y, x] < 100)
                    {
                        bitmap.SetPixel(x, y, Color.Black);
                    }
                    else if (canva[y, x] < 200)
                    {
                        bitmap.SetPixel(x, y, Color.Yellow);
                    }
                    else if (canva[y, x] < 300)
                    {
                        bitmap.SetPixel(x, y, Color.Orange);
                    }
                    else if (canva[y, x] < 500)
                    {
                        bitmap.SetPixel(x, y, Color.Red);
                    }
                    else
                    {
                        bitmap.SetPixel(x, y, Color.DarkRed);
                    }
                }
            }

            bitmap.Save("../../../../Resources/HeatMap.bmp");
            bitmap.Dispose();
        }

        public void TemporalHeatMap(int dim, int x, int y)
        {
            canvaDim[y, x, dim] += 1;
        }
        
        public void PictureHeatMapMultiDim()
        {
            for (int dim = 0; dim < canvaDim.GetLength(2); dim++)
            {

                Bitmap bitmap = new Bitmap(2000, 2000);

                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        if (canvaDim[y, x, dim] < 5)
                        {
                            bitmap.SetPixel(x, y, Color.Black);
                        }
                        else if (canvaDim[y, x, dim] < 10)
                        {
                            bitmap.SetPixel(x, y, Color.Yellow);
                        }
                        else if (canvaDim[y, x, dim] < 15)
                        {
                            bitmap.SetPixel(x, y, Color.Orange);
                        }
                        else if (canvaDim[y, x, dim] < 20)
                        {
                            bitmap.SetPixel(x, y, Color.Red);
                        }
                        else
                        {
                            bitmap.SetPixel(x, y, Color.DarkRed);
                        }
                    }

                }

                bitmap.Save($"{path}/toGif/HeatMapDim{dim}.bmp");
                bitmap.Dispose();
            }
        }
    }
}