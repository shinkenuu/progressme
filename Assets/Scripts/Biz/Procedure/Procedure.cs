using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI;

namespace Assets.Scripts.Biz.Procedure
{
    public abstract class Procedure : MonoBehaviour, IProcedure
    {
        protected WatcherOfAll Watcher;
        
        public abstract IEnumerator Proceed();

        protected virtual void OnEnable()
        {
            Watcher = GameObject.FindGameObjectWithTag("Watcher").GetComponent<WatcherOfAll>();
        }        
    }
}
