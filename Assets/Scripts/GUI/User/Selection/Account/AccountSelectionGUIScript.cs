using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts.GUI.User.Selection.Account
{
    public class AccountSelectionGUIScript : SelectionGui<ACCOUNT, AccountSelectionGUIScript.ActionDisplay>
    {

        #region GUI Vars

        [SerializeField]
        private GameObject addAccountButtonGo;
        [SerializeField]
        private GameObject cancelAccountCreationButtonGo;

        [SerializeField]
        private GameObject accountTypeIfGo;
        [SerializeField]
        private Text accountTypeText;
        [SerializeField]
        private GameObject accountNameIfGo;
        [SerializeField]
        private Text accountNameText;

        #endregion

        public enum ActionDisplay
        {
            AllAccounts
        }

        

        #region Selection Panel

        protected override void FetchNextAction()
        {
            if (actionsQueue.Count < 1)
            {
                return;
            }

            switch (actionsQueue.First())
            {
                case ActionDisplay.AllAccounts:
                    Handle_AllAccounts();
                    break;
                default:
                    throw new System.InvalidOperationException("ScheduledActionId " + actionsQueue.First().ToString() + " not identified in " + this.ToString());
            }

            RefreshPanel();
            actionsQueue.Dequeue();
        }



        protected override void PresetPanelElement(GameObject panelElem, ACCOUNT elemSubject)
        {
            panelElem.name = "btn_account_selection_option_" + elemSubject.account_name;
            panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
                "file:///" + AccountManagerScript.AccountsFolder.Replace('\\', '/')
                    + elemSubject.account_name + "/logo.png",
                100,
                100);
        }


        #endregion



        private void Handle_AllAccounts()
        {
            Selectables = RepReader.GetAllAccounts().ToList();
        }





        #region Buttons


        protected override void OnPanelButtonClick(ACCOUNT param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.account_name.Trim();
        }
        








        public void BtnActionAddNewAccount()
        {
            SetupAccountCreationGUI();
        }

        public void BtnActionCancelNewAccount()
        {
            SetupAccountSelectionGUI();
        }

        #endregion


        #region GUI Setup

        private void SetupAccountCreationGUI()
        {
            selectedAccountNameText.text = string.Empty;
            DisplayCancelAccountCreationBtn();
            DisplayInputFields();
        }

        private void SetupAccountSelectionGUI()
        {
            DisplayAddAccountBtn();
            HideInputFields();
        }


        private void DisplayAddAccountBtn()
        {
            addAccountButtonGo.SetActive(true);
            cancelAccountCreationButtonGo.SetActive(false);
        }

        private void DisplayCancelAccountCreationBtn()
        {
            addAccountButtonGo.SetActive(false);
            cancelAccountCreationButtonGo.SetActive(true);
        }

        private void DisplayInputFields()
        {


        accountNameText.text = string.Empty;
            accountNameIfGo.SetActive(true);
            accountTypeText.text = string.Empty;
            accountTypeIfGo.SetActive(true);
        }

        private void HideInputFields()
        {
            accountNameIfGo.SetActive(false);
            accountTypeIfGo.SetActive(false);
        }

        #endregion




    }
}