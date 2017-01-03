using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GUI.Offline
{
    public interface IOfflineGui
    {
        void InformUser(string msg);
        void Close();
    }
}
