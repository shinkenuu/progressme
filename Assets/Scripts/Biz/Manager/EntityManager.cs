using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.Server;

namespace Assets.Scripts.Biz.Manager
{
    public abstract class EntityManager<T> : MonoBehaviour where T : JSONable
    {
        protected WatcherOfAll Watcher;
        protected RepositoryWriter RepWriter;

        protected virtual void OnEnable()
        {
            Watcher = GameObject.FindGameObjectWithTag("Watcher").GetComponent<WatcherOfAll>();
            
            switch (SceneManager.GetActiveScene().name)
            {
                case "Offline":
                    break;
                case "Server":
                case "Panel":
                case "User":
                    RepWriter = Watcher.RepWriter;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }


        //public abstract T Insert(T entity);
        //public abstract T Update(T entity);
        //public abstract T Delete(T entity);
        //public abstract IEnumerable<T> Select(T entity);
        

        protected IEnumerator LookFor()
        {
            GameObject watcherGo;

            while(true)
            {
                watcherGo = GameObject.FindWithTag("Main Camera");

                if(watcherGo != null)
                {
                    WatcherOfAll watcher = watcherGo.GetComponent<WatcherOfAll>();
                    RepWriter = watcher.RepWriter;
                }

                yield return new WaitForEndOfFrame();
            }
        }





    }
}
