using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StrangeUpdater;
using Xunit;
namespace UpdaterTests
{
    public class Tests
    {
        [Fact]
        public void VersionParserForFileWorks()
        {
            VersionParser parser = new VersionParser();
            Assert.Equal(parser.GetVersionFromDisc(@"C:\Users\piatk\Desktop\VERSION.txt"), 2);
        }
        [Fact]
        public void VersionParserForNetworkWorks()
        {
            VersionParser parser = new VersionParser();
            Assert.Equal(parser.GetVersionFromNet(@"http://77.81.226.38/patcher/VERSION"), 2);
        }
        [Fact]
        public void UpdateRequiredWorks()
        {
            Updater up = new Updater();
            bool req = up.UpdateRequired(@"C:\Users\piatk\Desktop\VERSION.txt", @"http://77.81.226.38/patcher/VERSION");
            Assert.False(req);
        }
    }
}
