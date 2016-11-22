using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using System;

namespace Assets.Scripts.GUI.User.Selection.Opportunity
{
    public class OppSelectionGuiScript : SelectionGuiScript<OPPORTUNITY, OppSelectionGuiScript.ActionDisplay>
    {
        public enum ActionDisplay
        {
            OppsWithFocusedEmployee
        }
                

        protected override void FetchNextAction()
        {
            if (actionsQueue.Count < 1)
            {
                return;
            }

            switch (actionsQueue.First())
            {
                case ActionDisplay.OppsWithFocusedEmployee:
                    Handle_OppsWithFocusedEmployee();
                    break;
                default:
                    throw new System.InvalidOperationException("ScheduledActionId " + actionsQueue.First().ToString() + " not identified in " + this.ToString());
            }

            RefreshPanel();
            actionsQueue.Dequeue();
        }


        protected override void PresetPanelElement(GameObject panelElem, OPPORTUNITY elemSubject)
        {
            panelElem.name = "btn_opp_selection_option_" + elemSubject.opportunity_name;

            string accountOwnerName = RepReader.GetAccount(elemSubject.account_id).account_name;

            panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
                "file:///" + AccountManagerScript.AccountsFolder.Replace('\\', '/')
                    + accountOwnerName + "/logo.png",
                100,
                100);
        }




        private void Handle_OppsWithFocusedEmployee()
        {
            EMPLOYEE employeeInFocus = GameMng.EmployeeInFocus;
            Selectables = RepReader.GetAllOpps().Where(opp => opp.responsable_employee_id == employeeInFocus.id).ToList();
        }




        #region Buttons

        protected override void OnPanelButtonClick(OPPORTUNITY param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.opportunity_name.Trim();
        }

        #endregion


    }
}
