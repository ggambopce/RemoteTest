using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remotetest
{
    delegate void RecvImageEventHandler(object sender, RecvImageEventArgs e);
    internal class RecvImageEventArgs:EventArgs
    {
    }
}
