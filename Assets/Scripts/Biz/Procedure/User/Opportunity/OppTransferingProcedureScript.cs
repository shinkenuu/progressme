using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User;
using Assets.Scripts.Biz.Procedure;

namespace Assets.Scripts.Biz.Procedure.User.Opportunity
{
    public class OppTransferingProcedureScript : UserProcedure<OPPORTUNITY>, IProcedure
    {
        private OPPORTUNITY UserSelectedOpp;
        private EMPLOYEE UserSelectedEmployee;

        protected override IEnumerator UserProceed()
        {
            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(GUI.User.Selection.SelectionGui.Opp);
            CurrentSelectionGui.SetSelectables<OPPORTUNITY>(
                RepReader.GetOppsAssignedTo(Watcher.EmployeeInFocus.id));
            CurrentSelectionGui.RefreshPanel();
            yield return null;

            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(GUI.User.Selection.SelectionGui.Employee);
            CurrentSelectionGui.SetSelectables<EMPLOYEE>(
                RepReader.GetEmployeesIn(
                    RepReader.GetProcess(UserSelectedOpp.current_process_id ?? 0).id
                    ));
            CurrentSelectionGui.RefreshPanel();
            yield return null;
            
            if(!ValidateSelectedEntities())
            {
                throw new InvalidOperationException("Cannot proceed with invalid entities");
            }
            UserSelectedOpp.responsable_employee_id = UserSelectedEmployee.id;
            Command(UserSelectedOpp);
            yield break;
        }
        
        protected override void Command(OPPORTUNITY opp)
        {
            UserCmn.TransferOpp(opp, ReceiveCommandBack);
        }


        #region Validation

        protected override bool ValidateSelectedEntities()
        {
            return ValidateSelectedEmployee() && ValidateSelectionOpportunity();
        }

        private bool ValidateSelectedEmployee()
        {
            if (UserSelectedEmployee == null || RepReader.GetDepartment(UserSelectedEmployee.department_id).department_name == "sales")
            {
                Debug.Log("::CRITICAL:: Attempt to tranfer a opportunity with no selected responsable employee");
                return false;
            }

            return true;
        }
        
        private bool ValidateSelectionOpportunity()
        {
            if (UserSelectedEmployee == null || RepReader.GetDepartment(UserSelectedEmployee.department_id).department_name == "sales")
            {
                Debug.Log("::CRITICAL:: Attempt to transfer a null opportunity");
                return false;
            }

            return true;
        }

        #endregion
        

        #region Setters

        public void SetUserSelectedOpp(OPPORTUNITY opp)
        {
            UserSelectedOpp = opp;
        }

        public void SetUserSelectedEmployee(EMPLOYEE emp)
        {
            UserSelectedEmployee = emp;
        }
        
        #endregion

    }
}
