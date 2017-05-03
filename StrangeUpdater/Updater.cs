using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeUpdater
{
    public class Updater
    {
        public bool UpdateRequired(string localPath, string RemotePath)
        {
            VersionParser parser = new VersionParser();
            int localVer = parser.GetVersionFromDisc(localPath);
            int remoteVer = parser.GetVersionFromNet(RemotePath);
            if (localVer == 0 || remoteVer == 0) throw new ApplicationException();
            if (localVer < remoteVer) return true;
            return false;
        }
    }
}
