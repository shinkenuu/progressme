using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Biz.Manager;
using Assets.Scripts.Biz.Network;
using Assets.Scripts.Biz.Procedure;
using Assets.Scripts.Model;


/*
 * 
 * Instantiation order:
 * 1 ) Watcher
 * 2 ) Repositories
 * 3 ) Managers
 * 4 ) Player
 * 5 ) Guis
 * 6 ) Procedures
 * 
 */


namespace Assets.Scripts
{

    public enum ProgressMeScenes
    {
        Offline,
        Server,
        Panel,
        User        
    }
    

    [RequireComponent(typeof(NetManagerScript))]
    public class WatcherOfAll : MonoBehaviour
    {
        [SerializeField]
        private GameObject ManagerPrefab;
        [SerializeField]
        private GameObject RepositoriesPrefab;
        [SerializeField]
        private GameObject PlayerPrefab;
        

        private GameObject _repositories;
        private GameObject _managers;
        private GameObject _player;
        private GameObject _gui;
        private GameObject _proc;

        
        public EMPLOYEE EmployeeInFocus;


        #region Repository

        public RepositoryReader RepReader
        {
            get
            {
                return _repositories.GetComponent<RepositoryReader>();
            }
        }

        public RepositoryWriter RepWriter
        {
            get
            {
                return _repositories.GetComponent<RepositoryWriter>();
            }
        }

        #endregion
        

        #region Communication

        public Biz.Network.Offline.OfflineCmnScript OfflineCmn
        {
            get
            {
                return _player.GetComponent<Biz.Network.Offline.OfflineCmnScript>();
            }
        }

        public Biz.Network.User.UserCmnScript UserCmn
        {
            get
            {
                return _player.GetComponent<Biz.Network.User.UserCmnScript>();
            }
        }

        public Biz.Network.Panel.PanelCmnScript PanelCmn
        {
            get
            {
                return _player.GetComponent<Biz.Network.Panel.PanelCmnScript>();
            }
        }

        #endregion


        #region Managers
        
        public NetManagerScript NetMng
        {
            get
            {
                return gameObject.GetComponent<NetManagerScript>();
            }
        }

        public AccountManagerScript AccMng
        {
            get
            {
                return _managers.GetComponent<AccountManagerScript>();
            }
        }

        public EmployeeManagerScript EmpMng
        {
            get
            {
                return _managers.GetComponent<EmployeeManagerScript>();
            }
        }

        public OpportunityManagerScript OppMng
        {
            get
            {
                return _managers.GetComponent<OpportunityManagerScript>();
            }
        }

        #endregion


        #region Gui

        public GUI.Offline.OfflineGuiFactory OfflineGuiFactory
        {
            get
            {
                return _gui.GetComponent<GUI.Offline.OfflineGuiFactory>();
            }
        }


        public GUI.User.UserGuiFactory UserGuiFactory
        {
            get
            {
                return _gui.GetComponent<GUI.User.UserGuiFactory>();
            }
        }

        #endregion


        #region Procedure

        public Biz.Procedure.Offline.OfflineProcedureFactory OfflineProcedureFactory
        {
            get
            {
                return _proc.GetComponent<Biz.Procedure.Offline.OfflineProcedureFactory>();
            }
        }

        public Biz.Procedure.User.UserProcedureFactory UserProcedureFactory
        {
            get
            {
                return _proc.GetComponent<Biz.Procedure.User.UserProcedureFactory>();
            }
        }

        #endregion


        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        


        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ResetGo(ref _gui, "Guis");
            ResetGo(ref _proc, "Procedures");

            switch (scene.name)
            {
                case "Offline":
                    SetupForOfflineScene();
                    break;
                case "User":
                    SetupForUserScene();
                    break;
                case "Panel":
                    SetupForPanelScene();
                    break;
                case "Server":
                    SetupForServerScene();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }





        public void LoadScene(ProgressMeScenes pmScene)
        {
            switch (pmScene)
            {
                case ProgressMeScenes.Offline:
                    SceneManager.LoadScene("Offline", LoadSceneMode.Single);
                    break;
                case ProgressMeScenes.Server:
                    SceneManager.LoadScene("Server", LoadSceneMode.Single);
                    break;
                case ProgressMeScenes.User:
                    SceneManager.LoadScene("User", LoadSceneMode.Single);
                    break;
                case ProgressMeScenes.Panel:
                    SceneManager.LoadScene("Panel", LoadSceneMode.Single);
                    break;
                default:
                    throw new NotImplementedException(pmScene.ToString());

            }
            return;
        }



        




        private void ResetGo(ref GameObject go, string goName)
        {
            if (go != null)
            {
                Destroy(go);
            }

            go = new GameObject(goName);
        }





        private void SetupForOfflineScene()
        {
            _managers = GameObject.Instantiate(ManagerPrefab) as GameObject;

            StartCoroutine(LookForPlayerGo());

            _gui.AddComponent<GUI.Offline.OfflineGuiFactory>();
            var offlineProcFact = _proc.AddComponent<Biz.Procedure.Offline.OfflineProcedureFactory>();
            offlineProcFact.CreateOfflineProcedure(Biz.Procedure.Offline.OfflineProcedures.Login);
        }

        


        private void SetupForPanelScene()
        {
            StartCoroutine(LookForPlayerGo());
            _repositories = Instantiate(RepositoriesPrefab) as GameObject;
            _managers = GameObject.Instantiate(ManagerPrefab) as GameObject;
            //_gui.AddComponent<GUI.Panel.PanelGuiFactory>();
        }



        private void SetupForUserScene()
        {
            StartCoroutine(LookForPlayerGo());
            _repositories = Instantiate(RepositoriesPrefab) as GameObject;
            _managers = GameObject.Instantiate(ManagerPrefab) as GameObject;
            _gui.AddComponent<GUI.User.UserGuiFactory>();
            _proc.AddComponent<Biz.Procedure.User.UserProcedureFactory>();
        }



        private void SetupForServerScene()
        {
            _repositories = Instantiate(RepositoriesPrefab) as GameObject;
            RepWriter.LoadServerDataFromDisc();
            _managers = GameObject.Instantiate(ManagerPrefab) as GameObject;
            _gui.AddComponent<GUI.Server.ServerGuiFactory>().CreateServerGui(GUI.Server.ServerGuis.LogPanel);
        }


        





        private IEnumerator LookForPlayerGo()
        {
            //While the GameObject has not been Instantiated yet by the Scene
            while (true)
            {
                _player = FindLocalPlayer();

                if(_player != null)
                {
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }

        }
        
        private GameObject FindLocalPlayer()
        {
            GameObject localPlayer = null;

            foreach (GameObject playerGo in GameObject.FindGameObjectsWithTag("Player"))
            {
                try
                {
                    //Look for the playerGo of this connection
                    if (playerGo.GetComponent<NetworkIdentity>().isLocalPlayer)
                    {
                        //And set the references
                        localPlayer = playerGo;
                        break;
                    }
                }

                catch (UnityException)
                {
                    continue;
                }
            }

            return localPlayer;
        }
        
    }
}
