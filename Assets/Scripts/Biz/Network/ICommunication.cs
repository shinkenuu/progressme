using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Biz.Network
{
    public interface ICommunication
    {
        void Communicate(ref Command.Command cmd);
    }
}
