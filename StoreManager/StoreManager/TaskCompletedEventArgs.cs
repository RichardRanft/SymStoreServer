using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace StoreManager
{
    // Basic EventArgs class.
    public class TaskCompletedEventArgs : AsyncCompletedEventArgs
    {
        private bool output;

        // accessor
        public bool Output
        {
            get
            {
                RaiseExceptionIfNecessary();
                return output;
            }
        }

        public TaskCompletedEventArgs(bool output, Exception e, bool cancelled, object state)
            : base(e, cancelled, state)
        {
            this.output = output;
        }
    }
}
