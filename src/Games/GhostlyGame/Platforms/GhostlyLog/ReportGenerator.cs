using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Vub.Etro.IO;
using System.Reflection;

namespace GhostlyLog
{
    class ReportGenerator
    {
        public static double PAGE_WIDTH = 258;
        public const double EMG_HALF_HEIGHT = 15;
        public const int LEVEL_LINES = 18;
        public const double TILE_PIXEL_WIDTH = 40;
        private string _c3dFile = "";

        public string C3dFile {
            get { return _c3dFile; }
            set { _c3dFile = value; }
        }

        delegate void TemplateCallback(TextWriter writer);

        private Dictionary<string, TemplateCallback> callbacks = new Dictionary<string, TemplateCallback>();

        public ReportGenerator() {
            InitializeCallbacks();
        }

        public bool runTemplating(string templateFile, string dest)
        {
            using (StreamReader template = new StreamReader(templateFile))
            {
                using (StreamWriter file =
                 new StreamWriter(dest))
                {
                    string line;
                    do
                    {
                        line = template.ReadLine();
                        if (callbacks.Keys.Any(line.Contains))
                        {
                            foreach (string sign in callbacks.Keys)
                            {
                                if (line.Contains(sign))
                                {
                                    file.Write(line.Substring(0, line.IndexOf(sign)));
                                    callbacks[sign](file);
                                    file.WriteLine(line.Substring(line.LastIndexOf(sign) + sign.Length));
                                }
                            }
                        }
                        else
                        {
                            file.WriteLine(line);
                        }


                    } while (!template.EndOfStream);
                    line = template.EndOfStream ? null : line;
                }
            }
            return true;
        }
        public string RunTemplating(Stream templateStream)
        {
            StringBuilder sb = new StringBuilder();
            using (StreamReader template = new StreamReader(templateStream))
            {
                using (TextWriter file =
                 new StringWriter(sb))
                {
                    string line;
                    do
                    {
                        line = template.ReadLine();
                        if (callbacks.Keys.Any(line.Contains))
                        {
                            foreach (string sign in callbacks.Keys)
                            {
                                if (line.Contains(sign))
                                {
                                    file.Write(line.Substring(0, line.IndexOf(sign)));
                                    callbacks[sign](file);
                                    file.WriteLine(line.Substring(line.LastIndexOf(sign) + sign.Length));
                                }
                            }
                        }
                        else
                        {
                            file.WriteLine(line);
                        }


                    } while (!template.EndOfStream);
                    line = template.EndOfStream ? null : line;
                }
            }
            return sb.ToString();
        }

        private void InitializeCallbacks()
        {
            callbacks.Add("{emg1}", GenerateEMGGraph1);
            callbacks.Add("{emg2}", GenerateEMGGraph2);
            callbacks.Add("<!-- {level} -->", GenerateLevelLayout);
            callbacks.Add("<!-- {timeline_emg} -->", GenerateEmgTimeline);
            callbacks.Add("{j_contractions}", GenerateEMG1Count);
            callbacks.Add("{j_duration}", GenerateEMG1Average);
            callbacks.Add("{s_contractions}", GenerateEMG2Count);
            callbacks.Add("{s_duration}", GenerateEMG2Average);
            callbacks.Add("{level_num}", FillLevel);
            callbacks.Add("{date}", FillDate);
        }


        #region Generator callbacks
        private void GenerateEmgTimeline(TextWriter writer)
        {
            C3dReader reader = new C3dReader();
            reader.Open(_c3dFile);
            int samples = (reader.FramesCount * reader.Header.AnalogSamplingRate);
            double step = (PAGE_WIDTH) / samples;
            reader.Close();

            for (int i = 0; i < samples; i += 5000)
            {
                int seconds = i / 1000;
                writer.WriteLine("<path style=\"fill:none;stroke:#eeeeee;stroke-width:0.20px;stroke-linecap:butt;stroke-linejoin:miter;stroke-opacity:1\" " +
                    "d=\"m " + (20 + (step * i)).ToString("F5", CultureInfo.InvariantCulture) + ",150 v 30\" />");

                writer.WriteLine("<text style=\"font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:1.5px;line-height:1.25;font-family:Ubuntu;-inkscape-font-specification:Ubuntu;text-align:start;letter-spacing:0px;word-spacing:0px;text-anchor:start;fill:#cccccc;fill-opacity:1;stroke:none;stroke-width:0.26458332\"" +
                    " x=\"" + (20 + (step * i)).ToString("F5", CultureInfo.InvariantCulture) + "\" y=\"181.7\">" + (seconds.ToString() + " s") + "</text>");

                writer.WriteLine("<path style=\"fill:none;stroke:#eeeeee;stroke-width:0.20px;stroke-linecap:butt;stroke-linejoin:miter;stroke-opacity:1\" " +
                    "d=\"m " + (20 + (step * i)).ToString("F5", CultureInfo.InvariantCulture) + ",192.64585 v 30\" />");
                writer.WriteLine("<text style=\"font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:1.5px;line-height:1.25;font-family:Ubuntu;-inkscape-font-specification:Ubuntu;text-align:start;letter-spacing:0px;word-spacing:0px;text-anchor:start;fill:#cccccc;fill-opacity:1;stroke:none;stroke-width:0.26458332\"" +
                    " x=\"" + (20 + (step * i)).ToString("F5", CultureInfo.InvariantCulture) + "\" y=\"224.3\">" + (seconds.ToString() + " s") + "</text>");

                writer.WriteLine("<path style=\"fill:none;stroke:#eeeeee;stroke-width:0.20px;stroke-linecap:butt;stroke-linejoin:miter;stroke-opacity:1\" " +
                    "d=\"m " + (20 + (step * i)).ToString("F5", CultureInfo.InvariantCulture) + ",248.5 v 2\" />");
                writer.WriteLine("<text style=\"font-style:normal;font-variant:normal;font-weight:normal;font-stretch:normal;font-size:1.5px;line-height:1.25;font-family:Ubuntu;-inkscape-font-specification:Ubuntu;text-align:start;letter-spacing:0px;word-spacing:0px;text-anchor:start;fill:#cccccc;fill-opacity:1;stroke:none;stroke-width:0.26458332\"" +
                    " x=\"" + (20 + (step * i)).ToString("F5", CultureInfo.InvariantCulture) + "\" y=\"251.7\">" + (seconds.ToString() + " s") + "</text>");


            }
        }

        static int numActivationsEmg1 = -1;
        static int numActivationsEmg2 = -1;

        static double averageActivationsEmg1 = -1;
        static double averageActivationsEmg2 = -1;

        private void GenerateEMG1Count(TextWriter writer)
        {
            if (numActivationsEmg1 < 0) GenerateEMGStats();
            writer.Write(numActivationsEmg1 + " contractions");
        }
        private void GenerateEMG2Count(TextWriter writer)
        {
            if (numActivationsEmg2 < 0) GenerateEMGStats();
            writer.Write(numActivationsEmg2 + " contractions");
        }

        private void GenerateEMG1Average(TextWriter writer)
        {
            if (averageActivationsEmg1 < 0) GenerateEMGStats();
            writer.Write(averageActivationsEmg1.ToString("F2", CultureInfo.InvariantCulture) + " ms (avg. duration)");
        }
        private void GenerateEMG2Average(TextWriter writer)
        {
            if (averageActivationsEmg2 < 0) GenerateEMGStats();
            writer.Write(averageActivationsEmg2.ToString("F2", CultureInfo.InvariantCulture) + " ms (avg. duration)");
        }

        private void GenerateEMGStats()
        {

            double lengthActivationsEmg1 = 0;
            double lengthActivationsEmg2 = 0;


            C3dReader reader = new C3dReader();
            reader.Open(_c3dFile);
            int activation1Started = -1;
            int activation2Started = -1;
            int count = 0;
            for (int i = 0; i < reader.FramesCount; i++)
            {

                reader.ReadFrame();
                var data = reader.AnalogData.Data;
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[2, j] != 0)
                    {
                        if (activation1Started == -1)
                        {
                            activation1Started = count;
                        }
                    }
                    else
                    {
                        if (activation1Started != -1)
                        {
                            lengthActivationsEmg1 += count - activation1Started;
                            activation1Started = -1;
                            numActivationsEmg1++;
                        }
                    }


                    if (data[3, j] != 0)
                    {
                        if (activation2Started == -1)
                        {
                            activation2Started = count;
                        }
                    }
                    else
                    {
                        if (activation2Started != -1)
                        {
                            lengthActivationsEmg2 += count - activation2Started;
                            activation2Started = -1;
                            numActivationsEmg2++;
                        }
                    }
                    count++;
                }
            }
            averageActivationsEmg1 = lengthActivationsEmg1 / numActivationsEmg1;
            averageActivationsEmg2 = lengthActivationsEmg2 / numActivationsEmg2;
        }
        private void GenerateEMGGraph(TextWriter writer, int datastream)
        {
            double max = 0;
            double min = 0;
            C3dReader reader = new C3dReader();
            reader.Open(_c3dFile);
            for (int i = 0; i < reader.FramesCount; i++)
            {
                reader.ReadFrame();
                var data = reader.AnalogData.Data;
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    max = Math.Max(data[datastream, j], max);
                    min = Math.Min(data[datastream, j], min);
                }
            }
            reader.Close();

            double scale = EMG_HALF_HEIGHT / Math.Max(Math.Abs(max), Math.Abs(min));

            reader = new C3dReader();
            reader.Open(_c3dFile);
            double previous = 0;

            double step = (PAGE_WIDTH) / (reader.FramesCount * reader.Header.AnalogSamplingRate);
            var culture = CultureInfo.InvariantCulture;

            for (int i = 0; i < reader.FramesCount; i++)
            {
                Vector4[] frame = reader.ReadFrame();
                var data = reader.AnalogData.Data;
                double sum = 0;
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    double y = (-data[datastream, j] * scale);
                    writer.Write("l " + step.ToString("F7", culture) + " " + (y - previous).ToString("F7", culture));
                    previous = y;

                }


            }
            reader.Close();
        }

        private void FillDate(TextWriter writer)
        {
            C3dReader reader = new C3dReader();
            reader.Open(_c3dFile);
            string[] dateTime = reader.GetParameter<string[]>("INFO:TIME");
            writer.Write("Date: " + int.Parse(dateTime[2]).ToString() + "/" + int.Parse(dateTime[1]).ToString() + "/" + int.Parse(dateTime[0]).ToString() + "    " +
                int.Parse(dateTime[3]).ToString() + ":" + dateTime[4]);
            reader.Close();
        }

        private void FillLevel(TextWriter writer)
        {
            C3dReader reader = new C3dReader();
            reader.Open(_c3dFile);
            writer.Write("Level: " + reader.GetParameter<Int16>("INFO:GAME_LEVEL").ToString());
            reader.Close();
        }

        private void GenerateEMGGraph1(TextWriter writer)
        {
            GenerateEMGGraph(writer, 0);
        }

        private void GenerateEMGGraph2(TextWriter writer)
        {
            GenerateEMGGraph(writer, 1);
        }


        private void GenerateLevelLayout(TextWriter writer)
        {


            double maxx = 0;
            double halt = 0;
            double running = 0;
            var culture = CultureInfo.InvariantCulture;
            C3dReader reader = new C3dReader();
            reader.Open(_c3dFile);
            float previous = 0;
            Int16[] pos = new Int16[reader.FramesCount];
            for (int i = 0; i < reader.FramesCount; i++)
            {
                Vector4[] frame = reader.ReadFrame();
                if (frame[0].Z == 0)
                {
                    halt++;
                }
                else
                {
                    running++;
                }

                previous = frame[0].X;
                if (frame[0].X > maxx)
                {
                    maxx = frame[0].X;
                }
            }
            int levelNum = -1;
            try
            {
                if ((levelNum = reader.GetParameter<Int16>("INFO:GAME_LEVEL")) < 1) {
                    return;
                }
            }
            catch (ApplicationException e)
            {
                return;
            }
            finally { 
                reader.Close();
            }

            

            List<string> map = new List<string>();

            var assembly = Assembly.GetCallingAssembly();
            string templateName = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith("map" + levelNum + ".txt"));
            using (StreamReader mapReader = new StreamReader(assembly.GetManifestResourceStream(templateName)))
            {
                for (int i = 0; i < LEVEL_LINES && !mapReader.EndOfStream; i++)
                {
                    map.Add(mapReader.ReadLine());
                }
            }


            double tileSpace = PAGE_WIDTH * (running / (halt + running));
            double tile = tileSpace / (maxx / TILE_PIXEL_WIDTH);

            StringBuilder playerPath = new StringBuilder();

            reader = new C3dReader();
            reader.Open(_c3dFile);
            double step = PAGE_WIDTH / reader.FramesCount;
            int lastTile = -1;
            double currentPosition = 0;
            double lastTilePosition = 0;
            int lastx = 0;
            for (int i = 0; i < reader.FramesCount; i++)
            {
                Vector4[] frame = reader.ReadFrame();
                int x = (int)(frame[0].X / TILE_PIXEL_WIDTH);
                if (x > lastTile)
                {
                    lastTile = x;
                    if (currentPosition > (lastTilePosition + tile))
                    {
                        double width = currentPosition - (lastTilePosition + tile);
                        writer.WriteLine("<rect x=\"" + (20 + lastTilePosition + tile).ToString("F7", culture) + "\" y=\"" + (235 + (0 * tile)).ToString("F7", culture) + "\" width=\"" + width.ToString("F7", culture) + "\" height=\"" + (18 * tile).ToString("F7", culture) + "\" rx=\"" + (tile / 10).ToString("F7", culture) + "\" style=\"fill: rgb(222, 222, 222)\" />");
                    }
                    for (int y = 0; y < map.Count; y++)
                    {
                        if (map[y].Length > x &&
                        (map[y][x] == 'l' || map[y][x] == 'm' || map[y][x] == 'r' || map[y][x] == '\\'))
                        {
                            writer.WriteLine("<rect x=\"" + (20 + currentPosition).ToString("F7", culture) + "\" y=\"" + (235 + (y * tile)).ToString("F7", culture) + "\" width=\"" + tile.ToString("F7", culture) + "\" height=\"" + tile.ToString("F7", culture) + "\" rx=\"" + (tile / 10).ToString("F7", culture) + "\" style=\"fill: rgb(102,102,102)\" />");
                            lastTilePosition = currentPosition;
                            if (lastx < x) lastx = x;
                        }
                    }
                    currentPosition += tile;
                }
                if (frame[0].Z == 0)
                {
                    currentPosition += step;
                }
            }
            PAGE_WIDTH = currentPosition;
            reader.Close();
        }

        #endregion Generator callbacks
    }
}
