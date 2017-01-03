using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.Offline.Login;

namespace Assets.Scripts.GUI.Offline
{
    public class OfflineGuiFactory : MonoBehaviour
    {


        private IOfflineGui CurrentGui;
                

        public ILoginGui CreateLoginGui()
        {
            if (CurrentGui != null)
            {
                CurrentGui.Close();
            }

            GameObject loginGuiGo = Instantiate(Resources.Load<GameObject>("Prefabs/GUI/Offline/LoginGui"));
            CurrentGui = loginGuiGo.GetComponent<LoginGuiScript>();
            return CurrentGui as ILoginGui;
        }

    }
}
