using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts.GUI.User.Selection.Employee
{
    public class EmployeeSelectionGUIScript : SelectionGuiScript<EMPLOYEE, EmployeeSelectionGUIScript.ActionDisplay>
    {
        
        public enum ActionDisplay
        {
            AllEmployees,
            ManagerOverview,
            SalesEmployees,
            EllegibleEmployeeForAdvancingOpp,
            EllegibleEmployeeForTransferingOpp
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
                case ActionDisplay.AllEmployees:
                    Handle_AllEmployees();
                    break;
                case ActionDisplay.EllegibleEmployeeForAdvancingOpp:
                    Handle_EllegibleEmployeeForAdvancingOpp();
                    break;
                case ActionDisplay.EllegibleEmployeeForTransferingOpp:
                    Handle_EllegibleEmployeeForTransferingOpp();
                    break;
                case ActionDisplay.ManagerOverview:
                    Handle_ManagerOverview();
                    break;
                case ActionDisplay.SalesEmployees:
                    Handle_SalesEmployees();
                    break;
                default:
                    throw new System.InvalidOperationException("ScheduledActionId " + actionsQueue.First().ToString() + " not identified in " + this.ToString());
            }

            RefreshPanel();
            actionsQueue.Dequeue();
        }


        protected override void PresetPanelElement(GameObject panelElem, EMPLOYEE elemSubject)
        {
            panelElem.name = "btn_employee_selection_option_" + elemSubject.employee_name;
            panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
                "Sprites/Employees/" + elemSubject.employee_name + "/profile", 50, 50);
        }

        #endregion







        private void Handle_AllEmployees()
        {
            Selectables = RepReader.GetAllEmployees().ToList();
        }

        private void Handle_EllegibleEmployeeForAdvancingOpp()
        {
            if (GameMng._tempProcess == null)
            {
                Selectables = RepReader.GetEmployeesIn(RepReader.GetProcess(GameMng._tempOpp.current_process_id ?? 0).department_id).ToList();
            }
            else
            {
                Selectables = RepReader.GetEmployeesIn(GameMng._tempProcess.department_id).ToList();
            }
        }

        private void Handle_EllegibleEmployeeForTransferingOpp()
        {
            if (GameMng._tempProcess == null)
            {
                Selectables = RepReader.GetEmployeesIn(RepReader.GetProcess(GameMng._tempOpp.current_process_id ?? 0).department_id).ToList();
            }
            else
            {
                Selectables = RepReader.GetEmployeesIn(GameMng._tempProcess.department_id).ToList();
            }
            //Removes employee in focus
            Selectables.RemoveAll(e => e.id == GameMng._employeeInFocus.id);
        }

        private void Handle_ManagerOverview()
        {
            EMPLOYEE loggedEmployee = NetMng.GetLoggedEmployee();
            Selectables.Add(loggedEmployee);
            IEnumerable<EMPLOYEE> reportingWorkers = RepReader.GetWorkersReportingToRecursive(loggedEmployee.id);

            if (reportingWorkers != null)
            {
                Selectables.AddRange(reportingWorkers);
            }
        }

        private void Handle_SalesEmployees()
        {
            Selectables.Add(NetMng.GetLoggedEmployee());
            IEnumerable<EMPLOYEE> reportingSalesEmployees = RepReader.GetWorkersReportingToRecursive(NetMng.GetLoggedEmployee().id);

            if (reportingSalesEmployees != null)
            {
                Selectables.AddRange(reportingSalesEmployees);
            }

            Selectables.RemoveAll(e => e.department_id != RepReader.GetDepartment("sales").id);
        }






        #region Buttons


        protected override void OnPanelButtonClick(EMPLOYEE param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.employee_name.Trim().Replace('.', ' ');
        }
        

        #endregion
        
    }
}
