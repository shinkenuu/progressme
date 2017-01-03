using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GUI.Server
{
    public interface IServerGui
    {
        void Close();

        void Log(string msg);
    }
}
