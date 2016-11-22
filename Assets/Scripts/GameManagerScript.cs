using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts
{

    [RequireComponent((typeof(NetManagerScript)))]
    public class GameManagerScript : MonoBehaviour
    {

        private delegate void Informer(string msg);
        private Informer _informer;

        #region References

        private NetManagerScript NetMng;
        private LoginCmnScript _loginCmn;
        private PanelCmnScript _panelCmn;
        private UserCmnScript _userCmn;
        private RepositoryWriter _repWriter;

        private LoginGUIScript _loginGui;
        private ServerGUIScript _srvGui;
        private PanelGUIScript _panelGui;

        #endregion


        private void OnEnable()
        {
            NetMng = gameObject.GetComponent<NetManagerScript>();
            NetMng.ResetClient();
            StartCoroutine(LookForPlayerGo());
        }

        #region Procedure Auxiliar


        public enum Procedures
        {
            None,
            OppCreation,
            OppAdvance,
            OppTransfer,
            OppEnd,
            OppDetail,
            ManagerOverview
        };
        private Procedures curProcedure;




        public EMPLOYEE EmployeeInFocus;

        public void ScheduleProcedure(Procedures procedure)
        {
            Debug.Log("Scheduling procedure " + procedure.ToString());
            curProcedure = procedure;
        }


        #endregion



        #region Login

        /// <summary>
        /// Tries to login with given username and password.
        /// </summary>
        /// <param name="username">The username credential</param>
        /// <param name="password">The user's password credential</param>
        public void RequestLogin(string username, string password)
        {
            // The loginGui becomes null even after looking for it OnEnable
            if (_loginGui == null)
            {
                StartCoroutine(LookForLoginGuiGo());
                _informer = _loginGui.InformUser;
            }

            //If client is not connected to a server
            if (!NetMng.IsClientConnected())
            {
                //and the credentials are for setting up the server
                if (username == "server" && password == "cerver")
                {
                    _loginGui.InformUser("Trying to set server up...");
                    NetMng.StartServerSafely();

                    if (NetMng.IsServerUp())
                    {
                        Debug.Log("Server is up.");
                        LoadServerScene();
                        return;
                    }

                    Debug.Log("Couldn't start server.");
                    _loginGui.InformUser("Couldn't setup server.");
                    _loginGui.ControlInputFields(true);
                    return;
                }

                //if the credentials are not for server setup
                Debug.Log("Login request failed because client is not connected.");
                _loginGui.InformUser("Connection to server not estabilished. Resetting connection. Please, try to login again...");
                NetMng.ResetClient();
                _loginGui.ControlInputFields(true);
                return;
            }

            if (username == "panel")
            {
                _loginGui.InformUser("Trying to set panel up...");
                LoadPanelScene();
                return;
            }

            //If client is connected to a server
            Debug.Log("Sending login request for user: " + username);
            _loginGui.InformUser("Validating your credentials...");
            _loginCmn.CmdLogin(username, password);
            return;
        }

        /// <summary>
        /// Answer login attempt and inform user of the results. If succesful, loads Home.
        /// </summary>
        /// <param name="loggingEmployee"></param>
        public void AnswerLogin(EMPLOYEE loggingEmployee)
        {
            //Login denied
            if (loggingEmployee == null)
            {
                Debug.Log("Login denied.");
                _loginGui.InformUser("Credentials either are invalid or do not exist");
                _loginGui.ControlInputFields(true);
                return;
            }

            NetMng.SetLoggegWorkerProfile(loggingEmployee);
            LoadUserScene();
            return;
        }

        #endregion





        #region Server

        public void LogIntoServerDisplay(string msg)
        {
            if (_srvGui == null)
            {
                if (SceneManager.GetActiveScene().name == "Server")
                {
                    StartCoroutine(LookForServerGuiGo());
                }
            }

            _srvGui.Log(msg);
        }

        #endregion






        #region Opps

        private IEnumerator CreateNewOpportunity()
        {
            #region Validate selected employee

            if (_tempEmployee == null)
            {
                Debug.Log("::CRITICAL:: Attempt to create a opportunity with no selected responsable employee");
                yield break;
            }

            #endregion

            #region Validate selected account

            if (_tempAccount == null)
            {
                Debug.Log("::CRITICAL:: Attempt to create a opportunity with no selected account");
                yield break;
            }

            #endregion

            #region Validate selected product

            if (_tempProduct == null)
            {
                Debug.Log("::CRITICAL:: Attempt to create a opportunity with no selected product");
                yield break;
            }

            #endregion

            OPPORTUNITY newOpp = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            newOpp.SetOpportunity(0, "", "", System.DateTime.Now, _tempEmployee.id, 0, 0, _tempProduct.id, _tempAccount.id);

            RequestScript addOpportunityRequest = RequestScript.CreateInstance<RequestScript>();
            addOpportunityRequest.SetupRequest(RequestScript.RequestType.ADD_OPPORTUNITY, newOpp.ToJson().Print());

            if (!addOpportunityRequest.IsReady() && _homeGui != null)
            {
                Debug.LogError("Opp creation were setup but wasn't ready. Aborting");
                _homeGui.InformUser("Unable to create task");
                yield break;
            }

            _userCmn.PerformRequest(ref addOpportunityRequest);
            Debug.Log("Sent 'create opportunity' command to server. Waiting for the reply...");
            yield return new WaitUntil(addOpportunityRequest.IsClosed);
            Debug.Log("Received 'create opportunity' command reply");

            if (addOpportunityRequest.GetJsonArray() == null)
            {
                Debug.Log("::CRITICAL:: Attempt to create opportunity failed");
                _homeGui.InformUser("Unable to create task");
                yield break;
            }

            Debug.Log("Opportunity created");
            newOpp.FromJson(addOpportunityRequest.GetJsonArray()[0]);

            if (newOpp.responsable_employee_id != _employeeInFocus.id)
            {
                yield break;
            }

            _repWriter.AddOpportunity(newOpp);
            yield return new WaitForEndOfFrame();

            if (_homeGui.gameObject.activeSelf)
            {
                _homeGui.InformUser("Opp created");
                _homeGui.ScheduleAction(UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL);
            }

            yield break;
        }

        private IEnumerator AdvanceOpportunity()
        {
            if (_tempOpp == null)
            {
                Debug.LogError("Impossible advance null opportunity. Aborting");
                _homeGui.InformUser("Unable to advance task");
                yield break;
            }

            if (_tempProcess == null)
            {
                Debug.LogError("Impossible advance opportunity to null process. Aborting");
                _homeGui.InformUser("Unable to advance task");
                yield break;
            }

            if (_tempEmployee == null)
            {
                Debug.LogError("Impossible advance opportunity to null employee. .Aborting");
                _homeGui.InformUser("Unable to advance task");
                yield break;
            }

            _tempOpp.current_process_id = _tempProcess.id;
            _tempOpp.responsable_employee_id = _tempEmployee.id;

            RequestScript advancingRequest = RequestScript.CreateInstance<RequestScript>();
            advancingRequest.SetupRequest(RequestScript.RequestType.UPDATE_ADVANCING_OPPORTUNITY, _tempOpp.ToJson().Print());

            if (!advancingRequest.IsReady())
            {
                Debug.LogError("Opp advance were setup but wasn't ready. Aborting");
                _homeGui.InformUser("Unable to advance opportunity");
                yield break;
            }

            _userCmn.PerformRequest(ref advancingRequest);
            Debug.Log("Sent 'advance opportunity' command to server. Waiting for the reply...");
            _homeGui.InformUser("Advancing opportunity...");

            yield return new WaitUntil(advancingRequest.IsClosed);

            if (advancingRequest.GetJsonArray() == null)
            {
                Debug.Log("::CRITICAL:: Attempt to advance opportunity failed");
                _homeGui.InformUser("Unable to advance opportunity");
                yield break;
            }

            Debug.Log("The opportunity has been advanced");
            OPPORTUNITY advancedOpp = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            advancedOpp.FromJson(advancingRequest.GetJsonArray()[0]);

            if (advancedOpp.responsable_employee_id != _employeeInFocus.id)
            {
                yield break;
            }

            _repWriter.UpdateOpportunity(advancedOpp);
            yield return new WaitForEndOfFrame();

            _homeGui.InformUser("Opp advanced");
            if (_homeGui.isActiveAndEnabled)
            {
                _homeGui.ScheduleAction(UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL);
            }

            yield break;
        }

        private IEnumerator TransferOpportunity()
        {
            if (_tempOpp == null)
            {
                Debug.LogError("Impossible transfer null opportunity. Aborting");
                _homeGui.InformUser("Unable to transfer task");
                yield break;
            }

            if (_tempEmployee == null)
            {
                Debug.LogError("Impossible transfer opportunity to null employee. Aborting");
                _homeGui.InformUser("Unable to transfer task");
                yield break;
            }

            _tempOpp.responsable_employee_id = _tempEmployee.id;

            RequestScript transferRequest = RequestScript.CreateInstance<RequestScript>();
            transferRequest.SetupRequest(RequestScript.RequestType.UPDATE_TRANSFERING_OPPORTUNITY, _tempOpp.ToJson().Print());

            if (!transferRequest.IsReady())
            {
                Debug.LogError("opportunity transfer were setup but wasn't ready. Aborting");
                _homeGui.InformUser("Unable to transfer opportunity");
                yield break;
            }

            _userCmn.PerformRequest(ref transferRequest);
            Debug.Log("Sent 'transfer opportunity' command to server. Waiting for the reply...");
            _homeGui.InformUser("Transfering opportunity...");

            yield return new WaitUntil(transferRequest.IsClosed);

            if (transferRequest.GetJsonArray() == null)
            {
                Debug.Log("::CRITICAL:: Attempt to transfer opportunity failed");
                _homeGui.InformUser("Unable to transfer opportunity");
                yield break;
            }

            Debug.Log("The opportunity has been transfered");
            OPPORTUNITY transferedOpportunity = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            transferedOpportunity.FromJson(transferRequest.GetJsonArray()[0]);

            if (transferedOpportunity.responsable_employee_id != _employeeInFocus.id)
            {
                _repWriter.RemoveOpportunity(transferedOpportunity);
                yield break;
            }

            _repWriter.UpdateOpportunity(transferedOpportunity);
            yield return new WaitForEndOfFrame();

            _informer.Invoke("Opportunity transfered");
            if (_homeGui.isActiveAndEnabled)
            {
                _homeGui.ScheduleAction(UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL);
            }

            yield break;
        }

        private IEnumerator EndOpportunity()
        {
            if (_tempOpp == null)
            {
                Debug.LogError("Impossible to end a null opportunity. Aborting");
                _homeGui.InformUser("Unable to end task");
                yield break;
            }

            RequestScript endingRequest = RequestScript.CreateInstance<RequestScript>();
            endingRequest.SetupRequest(RequestScript.RequestType.REMOVE_OPPORTUNITY, _tempOpp.ToJson().Print());

            if (!endingRequest.IsReady())
            {
                Debug.LogError("Opp ending were setup but wasn't ready. Aborting");
                _homeGui.InformUser("Unable to end opportunity");
                yield break;
            }

            _userCmn.PerformRequest(ref endingRequest);
            Debug.Log("Sent 'end task' command to server. Waiting for the reply...");
            _informer.Invoke("Ending opportunity...");
            yield return new WaitUntil(endingRequest.IsClosed);

            if (endingRequest.GetJsonArray() == null)
            {
                Debug.Log("::CRITICAL:: Attempt to transfer opportunity failed");
                _homeGui.InformUser("Unable to end opportunity");
                yield break;
            }

            Debug.Log("The opportunity has been ended");
            OPPORTUNITY endedOpportunity = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            endedOpportunity.FromJson(endingRequest.GetJsonArray()[0]);

            _repWriter.RemoveOpportunity(endedOpportunity);
            yield return new WaitForEndOfFrame();

            _informer.Invoke("Opportunity ended");
            if (_homeGui.isActiveAndEnabled)
            {
                _homeGui.ScheduleAction(UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL);
            }

            yield break;
        }

        #endregion

        #region Account

        public IEnumerator CreateAccount()
        {
            if (string.IsNullOrEmpty(_tempAccount.account_name) || string.IsNullOrEmpty(_tempAccount.account_type))
            {
                _accountSelectionGui.InformUser("Please write the account's name and type before trying to create it");
                yield break;
            }

            ACCOUNT newAccount = _tempAccount;
            _tempAccount = null;

            RequestScript createAccountRequest = RequestScript.CreateInstance<RequestScript>();
            createAccountRequest.SetupRequest(RequestScript.RequestType.ADD_CUSTOMER, newAccount.ToJson().Print());

            if (!createAccountRequest.IsReady())
            {
                Debug.LogError("Account creating request setup but wasn't ready. Aborting");
                _accountSelectionGui.InformUser("Unable to create account");
                yield break;
            }

            _userCmn.PerformRequest(ref createAccountRequest);
            Debug.Log("Sent 'create account' command to server. Waiting for the reply...");
            _accountSelectionGui.InformUser("Creating account...");

            yield return new WaitUntil(createAccountRequest.IsClosed);

            if (createAccountRequest.GetJsonArray() == null)
            {
                Debug.Log("::CRITICAL:: Attempt to create account failed");
                _accountSelectionGui.InformUser("Unable to create account");
                yield break;
            }

            Debug.Log("The account has been created");
            newAccount.FromJson(createAccountRequest.GetJsonArray()[0]);
            _repWriter.AddAccount(newAccount);

            _accountSelectionGui.ScheduleAction(UserGUIScript.UserGuiAction.REFRESH_CUSTOMER_PANEL);

            yield break;
        }

        #endregion

        #region Report

        public void ScheduleOppReport()
        {
            StartCoroutine(CreateReport());
        }

        private IEnumerator CreateReport()
        {
            if (_tempOpp == null)
            {
                Debug.LogError("::CRITICAL:: Impossible to report a null task. Aborting");
                yield break;
            }

            RequestScript taskHistoryRequest = RequestScript.CreateInstance<RequestScript>();
            taskHistoryRequest.SetupRequest(RequestScript.RequestType.CONSULT_ENTIRE_OPPORTUNITY_HISTORY, _tempOpp.ToJson().Print());

            if (!taskHistoryRequest.IsReady())
            {
                Debug.LogError("::CRITICAL:: Entire Opp History Request was setup but was not ready. Aborting");
                yield break;
            }

            _userCmn.PerformRequest(ref taskHistoryRequest);
            yield return new WaitUntil(taskHistoryRequest.IsClosed);

            string[] replyArray = taskHistoryRequest.GetJsonArray();

            if (replyArray == null || string.IsNullOrEmpty(replyArray[0]))
            {
                Debug.LogError("::CRITICAL:: Request reply came null or empty. Aborting");
                yield break;
            }

            Debug.Log("Opp History request replied correctly.");
            StringBuilder strBuilder = new StringBuilder();

            foreach (string reply in replyArray)
            {
                strBuilder.AppendLine(reply);
            }

            _repWriter.SafeWriteToDisc(strBuilder.ToString(), RepositoryReader.FilesDirectory.Client, "Opp_Report_" + _tempOpp.id.ToString() + ".json");
        }

        #endregion





        #region Scene Control

        private void LoadServerScene()
        {
            StopAllCoroutines();
            SceneManager.LoadScene("Server", LoadSceneMode.Single);
            StartCoroutine(LookForServerGuiGo());
            return;
        }

        private void LoadUserScene()
        {
            SceneManager.LoadScene("User", LoadSceneMode.Single);
            StartCoroutine(LookForDataGo());

            StartCoroutine(RefreshUserStorage());
            StartCoroutine(LookForUserGuiGo());

            _employeeInFocus = NetMng.GetLoggedEmployee();
            StartCoroutine(RefreshFocusedWorkerOpps());
            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.SETUP_HOME));
            return;
        }

        public void LoadPanelScene()
        {
            DontDestroyOnLoad(_panelCmn.gameObject);
            SceneManager.LoadScene("Panel", LoadSceneMode.Single);
            StartCoroutine(LookForDataGo());

            StartCoroutine(RefreshPanelStorage());
            StartCoroutine(LookForPanelGuiGo());
            return;
        }

        public void LoadOfflineScene()
        {
            SceneManager.LoadScene("Offline", LoadSceneMode.Single);
            return;
        }

        #endregion





        #region Look For

        private IEnumerator LookForPlayerGo()
        {
            GameObject[] playerGOs;

            //While the GameObject has not been Instantiated yet by the Scene
            while (true)
            {
                playerGOs = GameObject.FindGameObjectsWithTag("Player");

                //If found any playerGO
                if (playerGOs != null && playerGOs.Count() > 0)
                {
                    foreach (GameObject playerGo in playerGOs)
                    {
                        try
                        {
                            //Look for the playerGo of this connection
                            if (playerGo.GetComponent<NetworkIdentity>().isLocalPlayer)
                            {
                                //And set the references
                                _loginCmn = playerGo.GetComponent<LoginCmnScript>();
                                _panelCmn = playerGo.GetComponent<PanelCmnScript>();
                                _userCmn = playerGo.GetComponent<UserCmnScript>();
                                yield break;
                            }
                        }

                        catch (UnityException)
                        {
                            continue;
                        }

                        yield return new WaitForEndOfFrame();
                    }

                    playerGOs = null;
                }

                yield return new WaitForEndOfFrame();
            }

        }

        private IEnumerator LookForLoginGuiGo()
        {
            GameObject loginGuiGo;

            //While the GameObject has not been Instantiated yet by the Scene
            while (true)
            {
                loginGuiGo = GameObject.Find("LoginGUI");

                if (loginGuiGo != null)
                {
                    _loginGui = loginGuiGo.GetComponent<LoginGUIScript>();
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LookForServerGuiGo()
        {
            GameObject srvGuiGo;

            //While the GameObject has not been Instantiated yet by the Scene
            while (true)
            {
                srvGuiGo = GameObject.Find("ServerGUI");

                if (srvGuiGo != null)
                {
                    _srvGui = srvGuiGo.GetComponent<ServerGUIScript>();
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LookForPanelGuiGo()
        {
            GameObject panelGuiGo;

            //While the GameObject has not been Instantiated yet by the Scene
            while (true)
            {
                panelGuiGo = GameObject.Find("PanelGUI");

                if (panelGuiGo != null)
                {
                    _panelGui = panelGuiGo.GetComponent<PanelGUIScript>();
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LookForUserGuiGo()
        {
            GameObject userGuiGo;

            //While the GameObject has not been Instantiated yet by the Scene
            while (true)
            {
                userGuiGo = GameObject.Find("GUIs");

                if (userGuiGo != null)
                {
                    _homeGui = GameObject.Find("HomeGUI").GetComponent<HomeGUIScript>();
                    _oppDetailGui = GameObject.Find("OppDetailsGUI").GetComponent<OppDetailsGUIScript>();
                    _accountSelectionGui = GameObject.Find("AccountSelectionGUI").GetComponent<AccountSelectionGUIScript>();
                    _employeeSelectionGui = GameObject.Find("EmployeeSelectionGUI").GetComponent<EmployeeSelectionGUIScript>();
                    _productSelectionGui = GameObject.Find("ProductSelectionGUI").GetComponent<ProductSelectionGUIScript>();
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator LookForDataGo()
        {
            GameObject dataGo;

            //While the GameObject has not been Instantiated yet by the Scene
            while (true)
            {
                dataGo = GameObject.Find("DataGO");

                if (dataGo != null)
                {
                    _repWriter = dataGo.GetComponent<RepositoryWriter>();
                    yield break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        #endregion







        #region Refreshers

        private IEnumerator RefreshUserStorage()
        {
            yield return new WaitWhile(() => (_repWriter == null));

            _repWriter.SetFresh(false);

            RequestScript blRequest = RequestScript.CreateInstance<RequestScript>();
            blRequest.SetupRequest(RequestScript.RequestType.REFRESH_DEPARTMENTS, null);
            _userCmn.PerformRequest(ref blRequest);

            RequestScript cstRequest = RequestScript.CreateInstance<RequestScript>();
            cstRequest.SetupRequest(RequestScript.RequestType.REFRESH_CUSTOMER, null);
            _userCmn.PerformRequest(ref cstRequest);

            RequestScript procRequest = RequestScript.CreateInstance<RequestScript>();
            procRequest.SetupRequest(RequestScript.RequestType.REFRESH_PROCESS, null);
            _userCmn.PerformRequest(ref procRequest);

            RequestScript prdRequest = RequestScript.CreateInstance<RequestScript>();
            prdRequest.SetupRequest(RequestScript.RequestType.REFRESH_PRODUCT, null);
            _userCmn.PerformRequest(ref prdRequest);

            RequestScript wProfileRequest = RequestScript.CreateInstance<RequestScript>();
            wProfileRequest.SetupRequest(RequestScript.RequestType.REFRESH_WORKER_PROFILE, null);
            _userCmn.PerformRequest(ref wProfileRequest);

            yield return new WaitUntil(blRequest.IsClosed);
            _repWriter.LoadDepartments(blRequest.GetJsonArray());

            yield return new WaitUntil(cstRequest.IsClosed);
            _repWriter.LoadAccounts(cstRequest.GetJsonArray());

            yield return new WaitUntil(procRequest.IsClosed);
            _repWriter.LoadProcesses(procRequest.GetJsonArray());

            yield return new WaitUntil(prdRequest.IsClosed);
            _repWriter.LoadProducts(prdRequest.GetJsonArray());

            yield return new WaitUntil(wProfileRequest.IsClosed);
            _repWriter.LoadEmployees(wProfileRequest.GetJsonArray());

            _repWriter.SetFresh(true);
            yield break;
        }

        private IEnumerator RefreshPanelStorage()
        {
            yield return new WaitWhile(() => (_repWriter == null));
            yield return new WaitWhile(() => (_panelCmn == null));

            _repWriter.SetFresh(false);

            RequestScript deptRequest = RequestScript.CreateInstance<RequestScript>();
            deptRequest.SetupRequest(RequestScript.RequestType.REFRESH_DEPARTMENTS, null);
            _panelCmn.PerformRequest(ref deptRequest);

            RequestScript cstRequest = RequestScript.CreateInstance<RequestScript>();
            cstRequest.SetupRequest(RequestScript.RequestType.REFRESH_CUSTOMER, null);
            _panelCmn.PerformRequest(ref cstRequest);

            RequestScript procRequest = RequestScript.CreateInstance<RequestScript>();
            procRequest.SetupRequest(RequestScript.RequestType.REFRESH_PROCESS, null);
            _panelCmn.PerformRequest(ref procRequest);

            RequestScript prdRequest = RequestScript.CreateInstance<RequestScript>();
            prdRequest.SetupRequest(RequestScript.RequestType.REFRESH_PRODUCT, null);
            _panelCmn.PerformRequest(ref prdRequest);

            RequestScript wProfileRequest = RequestScript.CreateInstance<RequestScript>();
            wProfileRequest.SetupRequest(RequestScript.RequestType.REFRESH_WORKER_PROFILE, null);
            _panelCmn.PerformRequest(ref wProfileRequest);

            yield return new WaitUntil(deptRequest.IsClosed);
            _repWriter.LoadDepartments(deptRequest.GetJsonArray());

            yield return new WaitUntil(cstRequest.IsClosed);
            _repWriter.LoadAccounts(cstRequest.GetJsonArray());

            yield return new WaitUntil(procRequest.IsClosed);
            _repWriter.LoadProcesses(procRequest.GetJsonArray());

            yield return new WaitUntil(prdRequest.IsClosed);
            _repWriter.LoadProducts(prdRequest.GetJsonArray());

            yield return new WaitUntil(wProfileRequest.IsClosed);
            _repWriter.LoadEmployees(wProfileRequest.GetJsonArray());

            _repWriter.SetFresh(true);
            yield break;
        }

        private IEnumerator RefreshFocusedWorkerOpps()
        {
            yield return new WaitWhile(() => (_repWriter == null));
            yield return new WaitUntil(() => (RepositoryWriter.Fresh == true));

            _repWriter.SetFresh(false);

            RequestScript assignedOppRequest = RequestScript.CreateInstance<RequestScript>();
            assignedOppRequest.SetupRequest(RequestScript.RequestType.CONSULT_ASSIGNED_OPPORTUNITIES, _employeeInFocus.ToJson().Print());
            _userCmn.PerformRequest(ref assignedOppRequest);

            yield return new WaitUntil(assignedOppRequest.IsClosed);
            _repWriter.LoadOpportunities(assignedOppRequest.GetJsonArray());

            if (_repWriter.GetAllOpps() == null)
            {
                _repWriter.SetOpportunities(new List<OPPORTUNITY>());
            }

            _repWriter.SetOppHistories(new List<OPPORTUNITY_HISTORY>());

            _repWriter.SetFresh(true);


            _repWriter.SetFresh(true);
        }

        #endregion



    }

}
