using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using Assets.Scripts.GUI.Offline;

namespace Assets.Scripts.GUI.Offline.Login
{
    public interface ILoginGui : Offline.IOfflineGui
    {
        void SetInputFieldsActive(bool active);

        void SetupInputFields(UnityAction<string> usernameCall, UnityAction<string> passwordCall);       

    }
}
