using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GUI.Server
{
    public abstract class ServerGui : GenericGUI, IServerGui
    {
        public abstract void Log(string msg);
    }
}
