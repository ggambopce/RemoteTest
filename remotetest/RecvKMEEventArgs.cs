using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remotetest
{
    delegate void RecvKMEEventHandler(object sender, RecvKMEEventArgs e);
    internal class RecvKMEEventArgs:EventArgs
    {
    }
}
