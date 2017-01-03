using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts;
using Assets.Scripts.Biz.Procedure.Offline;

namespace Assets.Scripts.Biz.Procedure.Offline
{
    
    public enum OfflineProcedures
    {
        None,
        Login
    }


    public class OfflineProcedureFactory : MonoBehaviour
    {

        public IProcedure CreateOfflineProcedure(OfflineProcedures proc)
        {
            switch(proc)
            {
                case OfflineProcedures.None:
                    return null;
                case OfflineProcedures.Login:
                    LoginProcedureScript loginProc = null;
                    try
                    {
                        loginProc = gameObject.GetComponent<LoginProcedureScript>();
                    }
                    catch(NullReferenceException)
                    {
                    }
                    if (loginProc == null)
                    {
                        return gameObject.AddComponent<LoginProcedureScript>() as IProcedure;
                    }
                    return loginProc as IProcedure;
                default:
                    throw new NotImplementedException();
            }
        }
    }   

}
