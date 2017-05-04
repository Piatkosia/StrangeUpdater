﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeUpdater
{
    public class RunParser
    {
        public static string GetBin()
        {
            string path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\VERSION";
            string raw = FileReader.GetTextFromFile(path).Split('\n').FirstOrDefault(x => x.StartsWith("startup"));
            if (String.IsNullOrEmpty(raw) == false)
            {
                return raw.Split('=')[1];
            }
            return "";
        }
    }
}
