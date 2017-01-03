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
    public class OppAdvancingProcedureScript : UserProcedure<OPPORTUNITY>, IProcedure
    {
        private OPPORTUNITY UserSelectedOpp;
        private EMPLOYEE UserSelectedEmployee;
        private PROCESS UserSelectedProcess;


        protected override IEnumerator UserProceed()
        {
            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(GUI.User.Selection.SelectionGui.Opp);
            CurrentSelectionGui.SetSelectables<OPPORTUNITY>(
                RepReader.GetOppsAssignedTo(Watcher.EmployeeInFocus.id));
            yield return null;

            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(GUI.User.Selection.SelectionGui.Process);
            CurrentSelectionGui.SetSelectables<PROCESS>(
                RepReader.GetNextProcessesOf(
                    RepReader.GetProcess(UserSelectedOpp.current_process_id ?? 0).id
                    ));
            yield return null;

            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(GUI.User.Selection.SelectionGui.Employee);
            CurrentSelectionGui.SetSelectables<EMPLOYEE>(
                RepReader.GetEmployeesIn(UserSelectedProcess.id));
            yield return null;
            
            if(!ValidateSelectedEntities())
            {
                throw new InvalidOperationException("Cannot proceed with invalid entities");
            }

            UserSelectedOpp.responsable_employee_id = UserSelectedEmployee.id;
            UserSelectedOpp.current_process_id = UserSelectedProcess.id;
            Command(UserSelectedOpp);

            yield break;
        }

        



        protected override void Command(OPPORTUNITY opp)
        {
            UserCmn.AdvanceOpp(opp, ReceiveCommandBack);
        }






        #region Validation


        protected override bool ValidateSelectedEntities()
        {
            return ValidateUserSelectedOpp() && ValidateUserSelectedProcess() && ValidateUserSelectedEmployee();
        }


        private bool ValidateUserSelectedOpp()
        {
            if (UserSelectedOpp == null)
            {
                Debug.LogError("Invalid opp");
                return false;
            }

            return true;
        }

        private bool ValidateUserSelectedProcess()
        {
            if (UserSelectedProcess == null || RepReader.GetNextProcessesOf(UserSelectedOpp.current_process_id ?? 0).Contains(UserSelectedProcess))
            {
                Debug.LogError("Invalid selected next process for opp " + UserSelectedOpp.opportunity_name);
                return false; 
            }

            return true;
        }

        private bool ValidateUserSelectedEmployee()
        {
            if (UserSelectedEmployee == null || UserSelectedEmployee.department_id != UserSelectedProcess.department_id)
            {
                Debug.LogError("Invalid selected employee for process " + UserSelectedProcess.process_name + " of opportunity " + UserSelectedOpp.opportunity_name);
                return false;
            }

            return true;
        }

        #endregion



        #region Setters

        /// <summary>
        /// To be used by the GUIs selected button
        /// </summary>
        /// <param name="opp"></param>
        public void SetUserSelectedOpp(OPPORTUNITY opp)
        {
            UserSelectedOpp = opp;
        }

        /// <summary>
        /// To be used by the GUIs selected button
        /// </summary>
        /// <param name="pcs"></param>
        public void SetUserSelectedProcesss(PROCESS pcs)
        {
            UserSelectedProcess = pcs;
        }

        /// <summary>
        /// To be used by the GUIs selected button
        /// </summary>
        /// <param name="emp"></param>
        public void SetUserSelectedEmployee(EMPLOYEE emp)
        {
            UserSelectedEmployee = emp;
        }

        #endregion


    }
}
