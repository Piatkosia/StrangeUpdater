using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StrangeUpdater
{
    public class VersionParser
    {
        public int GetVersionFromDisc(string Path)
        {
            string raw = FileReader.GetTextFromFile(Path).Split('\n').FirstOrDefault(x => x.StartsWith("VER"));
            if (String.IsNullOrEmpty(raw) == false) return ParseVersionNumber(raw);
            return 0;
        }

        public int ParseVersionNumber(string line)
        {
            Regex re = new Regex(@"\d+");
            Match m = re.Match(line);
            if (m.Success)
            {
                return Int32.Parse(m.Value);
            } 
            return 0;
        }

        public int GetVersionFromNet(string Path)
        {
            string raw = NetReader.GetTextFromNet(Path).Split('\n').FirstOrDefault(x => x.StartsWith("VER"));
            if (String.IsNullOrEmpty(raw) == false) return ParseVersionNumber(raw);
            return 0;
        }
    }
}
