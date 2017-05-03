using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeUpdater
{
    public class ValSetEventArgs : EventArgs
    {
        public object NewValue;

        public ValSetEventArgs()
        {
            
        }

        public ValSetEventArgs(object newValue)
        {
            NewValue = newValue;
        }
        public string GetStatus()
        {
            return $"{NewValue.ToString()} ";
        }
    }
    public delegate void ValSetEventHandler(object source, ValSetEventArgs ea);
}
