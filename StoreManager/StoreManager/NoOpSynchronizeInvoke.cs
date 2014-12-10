using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;

namespace StoreManager
{
    public class NoOpSynchronizeInvoke : ISynchronizeInvoke
    {
        private delegate object GeneralDelegate(Delegate method,
                                                object[] args);

        public bool InvokeRequired { get { return false; } }

        public Object Invoke(Delegate method, object[] args)
        {
            return method.DynamicInvoke(args);
        }

        public IAsyncResult BeginInvoke(Delegate method,
                                        object[] args)
        {
            GeneralDelegate x = Invoke;
            return x.BeginInvoke(method, args, null, x);
        }

        public object EndInvoke(IAsyncResult result)
        {
            GeneralDelegate x = (GeneralDelegate)result.AsyncState;
            return x.EndInvoke(result);
        }
    }
}
