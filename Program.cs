using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGLogParser
{
    class Program
    {
        private const string UsageMessage = "Usage: IGLogParser.exe<game_log_file> <output_file> <upc|upv>";

        static void Main(string[] args)
        {
            if (args == null || args.Length < 3)
            {
                Console.WriteLine(UsageMessage);
                return;
            }

            var output = args[1];
            var up = args[2];
            if(up != "upc" && up != "upv")
            {
                Console.WriteLine(UsageMessage);
                return;
            }

            Stopwatch sw = Stopwatch.StartNew();
            var actions = new Dictionary<string, ISet<string>>();
            var logs = File.ReadAllLines(args[0]);
            
            for (int i = 0; i < logs.Length; i++)
            {
                if(i == 0) continue; // header
                var line = logs[i].Split('\t');
                if (line.Length > 0)
                {
                    if (!actions.ContainsKey(line[3]))
                        actions.Add(line[3], new HashSet<string>());
                    actions[line[3]].Add($"{line[1]};{line[2]}"); // for kml it's {line[2]},{line[1]}
                }
            }


            // KML
            //  Console.WriteLine(
            //      @"<?xml version=""1.0"" encoding=""utf-8"" ?>
            //      <kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom"">
            //      <Document>
            //      <name>My portals</name>
            //      <open>1</open>
            //      <Folder>
            //      <name>Portals</name>
            //      <open>1</open>");
            //  foreach (var value in actions["captured portal"])
            //  {
            //      Console.WriteLine(@"<Placemark>
            //      <name>Portal</name>
            //      <Point>
            //      <coordinates>" + value + @",0</coordinates>
            //      </Point>
            //      </Placemark>");
            //  }
            //  Console.WriteLine(@"</Folder></Document></kml>");

            ISet<string> a = null;
            switch (up)
            {
                case "upc":
                    a = actions["captured portal"];
                    break;
                case "upv":
                    a = actions["hacked friendly portal"];
                    a.UnionWith(actions["created link"]);
                    a.UnionWith(actions["captured portal"]);
                    a.UnionWith(actions["resonator deployed"]);
                    a.UnionWith(actions["resonator upgraded"]);
                    a.UnionWith(actions["hacked enemy portal"]);
                    a.UnionWith(actions["hacked neutral portal"]);
                    break;
                default:
                    Console.WriteLine(UsageMessage);
                    return;
            }

            // HeatMap Leflet
            //int j = 0;
            //var sb = new StringBuilder("var addressPoints = [");
            //foreach (var p in a)
            //{
            //    sb.Append($"[{p},1.0]");
            //    if (++j < cp.Count) sb.Append(",");
            //}
            //sb.Append("];");

            // HeatMap IITC
            var sb = new StringBuilder();
            foreach (var p in a)
            {
                sb.AppendLine(p);
            }

            File.WriteAllText(output, sb.ToString());

            Console.WriteLine($"File '{output}' has been generated with success in {sw.ElapsedMilliseconds.ToString()} ms.");
        }
    }
}

