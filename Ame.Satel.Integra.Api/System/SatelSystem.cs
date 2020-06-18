using System;
using System.Collections.Generic;
using System.Text;

namespace Ame.Satel.Integra.Api.System
{
    class SatelSystem
    {
        private SatelConnection connection;

        public SatelSystem(string hostname, int port = 7094)
        {
            connection = new SatelConnection(hostname, port);
        }
    }
}
