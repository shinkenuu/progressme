using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using Assets.Scripts;
using Assets.Scripts.Biz.Network;

namespace Assets.Scripts.GUI
{
    public abstract class GenericGUI : MonoBehaviour
    {
        protected WatcherOfAll Watcher;
        protected NetManagerScript NetMng;    

        protected virtual void OnEnable()
        {
            Watcher = GameObject.FindGameObjectWithTag("Watcher").GetComponent<WatcherOfAll>();
            NetMng = Watcher.NetMng;
            //RepReader = Watcher.RepReader;

            gameObject.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        }

        public void Close()
        {
            Destroy(this);
        }

        protected void ClearPanel(Transform panelTransform)
        {
            List<Transform> childrenTransform = new List<Transform>();
            foreach (Transform child in panelTransform.GetComponentInChildren<Transform>())
            {
                childrenTransform.Add(child);
            }

            foreach (Transform child in panelTransform)
            {
                Destroy(child.gameObject);
            }
        }

        
        
    }
}