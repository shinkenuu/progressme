using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Biz.Network;
using Assets.Scripts.GUI.Offline;

namespace Assets.Scripts.Biz.Procedure.Offline
{
    public class LoginProcedureScript : OfflineProcedure, IProcedure
    {
        private NetManagerScript NetMng;

        private GUI.Offline.Login.ILoginGui LoginGui;

        private string Username;
        private string Password;


        protected override void OnEnable()
        {
            base.OnEnable();
            NetMng = Watcher.NetMng;
            LoginGui = GuiFactory.CreateLoginGui();
            LoginGui.SetupInputFields(OnUsernameSelected, OnPasswordSelected);
        }
     

        /// <summary>
        /// Tries to login with given username and password.
        /// </summary>
        /// <returns></returns>
        public override IEnumerator Proceed()
        {
            //If client is not connected to a server
            if (!NetMng.IsClientConnected())
            {
                if(ValidateForServer(Username, Password))
                {
                    LoginGui.InformUser("Trying to set server up...");
                    NetMng.StartServerSafely();

                    if (NetMng.IsServerUp())
                    {
                        Debug.Log("Server is up.");
                        Watcher.LoadScene(ProgressMeScenes.Server);
                        yield break;
                    }

                    Debug.Log("Couldn't start server.");
                    LoginGui.InformUser("Couldn't setup server.");
                    LoginGui.SetInputFieldsActive(true);
                    yield break;
                }

                else if(ValidateForPanel(Username))
                {
                    LoginGui.InformUser("Trying to set panel up...");
                    Watcher.LoadScene(ProgressMeScenes.Panel);
                    yield break;
                }
                
                //if the credentials are not for server setup
                Debug.Log("Login request failed because client is not connected.");
                LoginGui.InformUser("Connection to server not estabilished. Resetting connection. Please, try to login again...");
                NetMng.ResetClient();
                LoginGui.SetInputFieldsActive(true);
                yield break;
            }

            //If client is connected to a server
            if (ValidateForEmployee(Username, Password))
            {
                Debug.Log("Sending login request for employee: " + Username);
                LoginGui.InformUser("Validating your credentials...");
                
                if(OffCmn == null)
                {
                    OffCmn = Watcher.OfflineCmn;
                }

                OffCmn.Login(Username, Password, AnswerLogin);
            }
            
            yield break;
        }

        
        /// <summary>
        /// Answer login attempt and inform user of the results. Call GameMng.LoadUserScene() if OK.
        /// </summary>
        /// <param name="emp"></param>
        public void AnswerLogin(Network.Command.Command cmd)
        {
            //Login denied
            if (cmd.JsonArray == null)
            {
                Debug.Log("Login denied.");
                LoginGui.InformUser("Invalid credentials");
                LoginGui.SetInputFieldsActive(true);
                return;
            }

            EMPLOYEE emp = EMPLOYEE.CreateInstance<EMPLOYEE>();
            emp.FromJson(cmd.JsonArray[0]);
            NetMng.LoggedEmployee = emp;
            Watcher.LoadScene(ProgressMeScenes.User);
            return;
        }
        
        

        #region Validation

        private bool ValidateForServer(string username, string password)
        {
            return username == "server" && password == "cerver";
        }

        private bool ValidateForPanel(string username)
        {
            return username == "panel";
        }

        private bool ValidateForEmployee(string username, string password)
        {
            return !string.IsNullOrEmpty(username.Trim()) && !string.IsNullOrEmpty(password);
        }

        #endregion
        
        
        #region Setters

        public void OnUsernameSelected(string username)
        {
            username = username.Trim();

            if (string.IsNullOrEmpty(username))
            {
                Username = string.Empty;
                LoginGui.InformUser("Invalid username");
                return;
            }

            Username = username.ToLower();
        }

        public void OnPasswordSelected(string password)
        {
            if (Username == string.Empty)
            {
                Password = string.Empty;
                LoginGui.InformUser("Please enter with a username before the password");
                return;
            }

            Password = password;
            Proceed().MoveNext();
        }

        #endregion
        
    }
}
