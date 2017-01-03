using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GUI.Server
{
    public enum ServerGuis
    {
        None,
        LogPanel
    }

    public class ServerGuiFactory : MonoBehaviour
    {        
        private IServerGui CurrentGui;
        
        public IServerGui CreateServerGui(ServerGuis gui)
        {
            if (CurrentGui != null)
            {
                CurrentGui.Close();
            }

            GameObject serverGuiGo;

            switch (gui)
            {
                case ServerGuis.None:
                    CurrentGui = null;
                    return CurrentGui as IServerGui;
                case ServerGuis.LogPanel:
                    serverGuiGo = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/Server/LogPanelGui")) as GameObject;
                    serverGuiGo.transform.SetParent(transform);
                    CurrentGui = serverGuiGo.GetComponent<LogPanelScript>();
                    return CurrentGui as IServerGui;
                default:
                    throw new NotImplementedException(gui.ToString());
            }
        }

    }
}
