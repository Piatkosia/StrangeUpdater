using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeUpdater
{
    public class LinkParser
    {
        public static string GetLink()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\VERSION";
            string raw = FileReader.GetTextFromFile(path).Split('\n').FirstOrDefault(x => x.StartsWith("Link"));
            if (String.IsNullOrEmpty(raw) == false)
            {
                return raw.Split('=')[1];
            }
            return "";
        }
    }
}
