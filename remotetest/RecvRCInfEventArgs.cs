using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace remotetest
{
    delegate void RecvRCInfEventHandler(object sender, RecvRCInfEventArgs e);
    internal class RecvRCInfEventArgs: EventArgs
    {
    }
}
