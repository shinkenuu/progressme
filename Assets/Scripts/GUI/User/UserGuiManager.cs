using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;


namespace Assets.Scripts.GUI.User
{
    public enum Guis
    {
        None,
        OppSelection,
        EmployeeSelection,
        AccountSelection,
        ProductSelection,
        OppDetails
    };

    public abstract class UserGuiManager : GenericGUI
    {

        private HomeGUIScript _homeGui;
        private OppDetailsGUIScript _oppDetailGui;

        private Selection.Opportunity.OppSelectionGuiScript OppSelection;
        private Selection.Employee.EmployeeSelectionGUIScript EmployeeSelection;
        private Selection.Account.AccountSelectionGUIScript AccountSelection;
        private Selection.Product.ProductSelectionGUIScript ProductSelection;
        




        public IUserGui Show(Guis guiToShow)
        {
            switch(guiToShow)
            {
                case Guis.None:
                    break;
                case Guis.OppSelection:
                    break;
                case Guis.EmployeeSelection:
                    break;
                case Guis.AccountSelection:
                    break;
                case Guis.ProductSelection:
                    break;
                case Guis.OppDetails:
                    break;
            }
        }

        





        #region Gui Control


        public void LoadPreviousWindow()
        {
            switch (curProcedure)
            {
                case Procedures.OppCreation:
                    switch (curGui)
                    {
                        case Guis.WorkerSelection:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            curProcedure = Procedures.None;
                            break;
                        case Guis.AccountSelection:
                            StartCoroutine(ActivateGui(Guis.EmployeeSelection, UserGUIScript.UserGuiAction.DISPLAY_LOGGED_AND_ITS_REPORTING_SALES_EMPLOYEES));
                            break;
                        case Guis.ProductSelection:
                            StartCoroutine(ActivateGui(Guis.AccountSelection, UserGUIScript.UserGuiAction.REFRESH_CUSTOMER_PANEL));
                            break;
                    }
                    break;

                case Procedures.OppAdvance:
                    switch (curGui)
                    {
                        case Guis.OppDetails:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            curProcedure = Procedures.None;
                            break;
                        case Guis.WorkerSelection:
                            StartCoroutine(ActivateGui(Guis.OppDetails, UserGUIScript.UserGuiAction.SETUP_OPPORTUNITY_DETAILS));
                            curProcedure = Procedures.OppDetail;
                            break;
                    }
                    break;

                case Procedures.OppTransfer:
                    switch (curGui)
                    {
                        case Guis.OppDetails:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            curProcedure = Procedures.None;
                            break;
                        case Guis.WorkerSelection:
                            StartCoroutine(ActivateGui(Guis.OppDetails, UserGUIScript.UserGuiAction.SETUP_OPPORTUNITY_DETAILS));
                            curProcedure = Procedures.OppDetail;
                            break;
                    }
                    break;

                case Procedures.OppDetail:
                    switch (curGui)
                    {
                        case Guis.OppDetails:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            curProcedure = Procedures.None;
                            break;
                    }
                    break;

                case Procedures.ManagerOverview:
                    switch (curGui)
                    {
                        case Guis.WorkerSelection:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            curProcedure = Procedures.None;
                            break;
                    }
                    break;
            }
        }


        public void LoadNextWindow()
        {
            switch (curProcedure)
            {
                case Procedures.OppCreation:
                    switch (curGui)
                    {
                        case Guis.Home:
                            StartCoroutine(ActivateGui(Guis.WorkerSelection, UserGUIScript.UserGuiAction.DISPLAY_LOGGED_AND_ITS_REPORTING_SALES_EMPLOYEES));
                            break;
                        case Guis.WorkerSelection:
                            StartCoroutine(ActivateGui(Guis.AccountSelection, UserGUIScript.UserGuiAction.REFRESH_CUSTOMER_PANEL));
                            break;
                        case Guis.AccountSelection:
                            StartCoroutine(ActivateGui(Guis.ProductSelection, UserGUIScript.UserGuiAction.REFRESH_PRODUCT_PANEL));
                            break;
                        case Guis.ProductSelection:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            StartCoroutine(CreateNewOpportunity());
                            curProcedure = Procedures.None;
                            break;
                    }
                    break;

                case Procedures.OppAdvance:
                    switch (curGui)
                    {
                        case Guis.OppDetails:
                            StartCoroutine(ActivateGui(Guis.WorkerSelection, UserGUIScript.UserGuiAction.DISPLAY_ELEGIBLE_EMPLOYEES_FOR_NEXT_PROCESS));
                            break;
                        case Guis.WorkerSelection:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            StartCoroutine(AdvanceOpportunity());
                            curProcedure = Procedures.None;
                            break;
                    }
                    break;

                case Procedures.OppTransfer:
                    switch (curGui)
                    {
                        case Guis.OppDetails:
                            StartCoroutine(ActivateGui(Guis.WorkerSelection, UserGUIScript.UserGuiAction.DISPLAY_ELEGIBLE_EMPLOYEES_FOR_NEXT_PROCESS_BUT_EMPLOYEE_IN_FOCUS));
                            break;
                        case Guis.WorkerSelection:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            StartCoroutine(TransferOpportunity());
                            curProcedure = Procedures.None;
                            break;
                    }
                    break;

                case Procedures.OppEnd:
                    StartCoroutine(RefreshFocusedWorkerOpps());
                    StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                    StartCoroutine(EndOpportunity());
                    curProcedure = Procedures.None;
                    break;

                case Procedures.OppDetail:
                    switch (curGui)
                    {
                        case Guis.Home:
                            StartCoroutine(ActivateGui(Guis.OppDetails, UserGUIScript.UserGuiAction.SETUP_OPPORTUNITY_DETAILS));
                            break;
                        case Guis.OppDetails:
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.REFRESH_OPPORTUNITY_PANEL));
                            curProcedure = Procedures.None;
                            break;
                    }
                    break;

                case Procedures.ManagerOverview:
                    switch (curGui)
                    {
                        case Guis.Home:
                            StartCoroutine(ActivateGui(Guis.WorkerSelection, UserGUIScript.UserGuiAction.DISPLAY_LOGGED_AND_ITS_REPORTING_EMPLOYEES));
                            break;
                        case Guis.WorkerSelection:
                            _employeeInFocus = _tempEmployee;
                            StartCoroutine(RefreshFocusedWorkerOpps());
                            StartCoroutine(ActivateGui(Guis.Home, UserGUIScript.UserGuiAction.SETUP_HOME));
                            curProcedure = Procedures.None;
                            break;
                    }
                    break;

            }
        }


        private IEnumerator ActivateGui(Guis activatingGui)
        {
            if (userAction != UserGUIScript.UserGuiAction.NONE)
            {
                yield return new WaitWhile(() => (_homeGui == null));
                yield return new WaitWhile(() => (_oppDetailGui == null));
                yield return new WaitWhile(() => (_employeeSelectionGui == null));
                yield return new WaitWhile(() => (_accountSelectionGui == null));
                yield return new WaitWhile(() => (_productSelectionGui == null));
                yield return new WaitUntil(() => (RepositoryWriter.Fresh));
            }

            switch (activatingGui)
            {
                case Guis.Home:
                    _informer = _homeGui.InformUser;
                    _homeGui.gameObject.SetActive(true);
                    _employeeSelectionGui.gameObject.SetActive(false);
                    _accountSelectionGui.gameObject.SetActive(false);
                    _productSelectionGui.gameObject.SetActive(false);

                    _oppDetailGui.gameObject.SetActive(false);
                    curGui = Guis.Home;

                    _homeGui.ScheduleAction(userAction);
                    break;
                case Guis.WorkerSelection:
                    _informer = _employeeSelectionGui.InformUser;
                    _homeGui.gameObject.SetActive(false);
                    _employeeSelectionGui.gameObject.SetActive(true);
                    _accountSelectionGui.gameObject.SetActive(false);
                    _productSelectionGui.gameObject.SetActive(false);
                    _oppDetailGui.gameObject.SetActive(false);
                    curGui = Guis.WorkerSelection;

                    _employeeSelectionGui.ScheduleAction(userAction);
                    break;
                case Guis.AccountSelection:
                    _informer = _accountSelectionGui.InformUser;
                    _homeGui.gameObject.SetActive(false);
                    _employeeSelectionGui.gameObject.SetActive(false);
                    _accountSelectionGui.gameObject.SetActive(true);
                    _productSelectionGui.gameObject.SetActive(false);
                    _oppDetailGui.gameObject.SetActive(false);
                    curGui = Guis.AccountSelection;

                    _accountSelectionGui.ScheduleAction(userAction);
                    break;
                case Guis.ProductSelection:
                    _informer = _accountSelectionGui.InformUser;
                    _homeGui.gameObject.SetActive(false);
                    _employeeSelectionGui.gameObject.SetActive(false);
                    _accountSelectionGui.gameObject.SetActive(false);
                    _productSelectionGui.gameObject.SetActive(true);
                    _oppDetailGui.gameObject.SetActive(false);
                    curGui = Guis.ProductSelection;

                    _productSelectionGui.ScheduleAction(userAction);
                    break;
                case Guis.OppDetails:
                    _informer = _oppDetailGui.InformUser;
                    _homeGui.gameObject.SetActive(false);
                    _employeeSelectionGui.gameObject.SetActive(false);
                    _accountSelectionGui.gameObject.SetActive(false);
                    _productSelectionGui.gameObject.SetActive(false);
                    _oppDetailGui.gameObject.SetActive(true);
                    curGui = Guis.OppDetails;

                    _oppDetailGui.ScheduleAction(userAction);
                    break;
                default:
                    _informer = Debug.Log;
                    Debug.LogError("Attempt to load user GUI not implemented in GameManager.ActivateUserGui()");
                    _homeGui.gameObject.SetActive(false);
                    _employeeSelectionGui.gameObject.SetActive(false);
                    _accountSelectionGui.gameObject.SetActive(false);
                    _productSelectionGui.gameObject.SetActive(false);
                    curGui = Guis.None;
                    break;
            }

            yield break;
        }



        #endregion






        
        
    }
}
