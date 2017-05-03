using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrangeUpdater
{
    public class ValChangedEventArgs : EventArgs
    {
        public object OldValue;
        public object NewValue;

        public ValChangedEventArgs()
        {
                
        }
        public ValChangedEventArgs(object oldVal, object newVal)
        {
            OldValue = oldVal;
            NewValue = newVal;
        }

        public string GetStatus()
        {
            return $" {OldValue.ToString()} -> {NewValue.ToString()} ";
        }
    }
    public delegate void ValChangedEventHandler(object source, ValChangedEventArgs ea);
}
