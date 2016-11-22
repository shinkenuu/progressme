using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using Assets.Scripts;
using Assets.Scripts.Model;


public class LoginCmnScript : NetworkBehaviour
{
    
    private EmployeeManagerScript _employeeMng;

    private GameManagerScript GameMng;

    private void OnEnable()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Offline":
                StartCoroutine(LookForGameManager());
                break;
            case "Server":
                StartCoroutine(LookForGameManager());
                StartCoroutine(LookForWorkerManager());
                break;
        }
    }
    
    /// <summary>
    /// Consult Worker's Manager for credentials.
    /// </summary>
    /// <param name="requesterNetId">NetworkInstanceId of the object requesting</param>
    /// <param name="username">Logging in username</param>
    /// <param name="password"></param>
    [Command]
    public void CmdLogin(string username, string password)
    {
        //Validate credentials
        EMPLOYEE loginEmployee = _employeeMng.ValidateLogIn(username, password);

        if (loginEmployee == null)
        {
            Debug.Log("Login attempt denied for username: " + username + " with password: " + password);
            RpcLogin(null);
            return;
        }

        Debug.Log("Login accepted for username: " + username + " with password: " + password);
        RpcLogin(loginEmployee.ToJson().Print());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="netId"></param>
    /// <param name="workerProfileJson"></param>
    [ClientRpc]
    public void RpcLogin(string workerProfileJson)
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(string.IsNullOrEmpty(workerProfileJson))
        {
            Debug.Log("Login denied.");
            GameMng.AnswerLogin(null);
            return;
        }

        Debug.Log("Login accepted.");
        DontDestroyOnLoad(this);
        EMPLOYEE loggingEmployee = EMPLOYEE.CreateInstance<EMPLOYEE>();
        loggingEmployee.FromJson(workerProfileJson);

        GameMng.AnswerLogin(loggingEmployee);
        return;
    }
    
    [ClientRpc]
    public void RpcLoginPanel()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        GameMng.LoadPanelScene();
    }


    private IEnumerator LookForGameManager()
    {
        GameObject mngGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            mngGo = GameObject.Find("MngGO");

            if (mngGo != null)
            {
                GameMng = mngGo.GetComponent<GameManagerScript>();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator LookForWorkerManager()
    {
        GameObject mngGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            mngGo = GameObject.Find("SrvMngGO");

            if (mngGo != null)
            {
                _employeeMng = mngGo.GetComponent<EmployeeManagerScript>();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }
        
}
