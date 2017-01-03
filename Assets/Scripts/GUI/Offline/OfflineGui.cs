using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Events;

namespace Assets.Scripts.GUI.Offline
{
    public abstract class OfflineGui : GenericGUI, IOfflineGui
    {
        [SerializeField]
        private Text Message;



        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public void InformUser(string msg)
        {
            Message.text = msg;
        }

    }
}
