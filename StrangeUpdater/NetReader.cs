using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace StrangeUpdater
{
    class NetReader
    {
        internal static string GetTextFromNet(string path)
        {
            try
            {
                WebClient client = new WebClient();
                Uri URL = path.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                    ? new Uri(path)
                    : new Uri("http://" + path);
                Stream stream = client.OpenRead(URL);
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "";
            }
        }
    }
}
