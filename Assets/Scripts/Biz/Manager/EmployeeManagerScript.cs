using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Biz.Manager;
using System;

namespace Assets.Scripts.Biz.Manager
{

    public class EmployeeManagerScript : EntityManager<EMPLOYEE>
    {

        /// <summary>
        /// Assigns task to worker
        /// </summary>
        /// <param name="responsableWorkerId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool AssignNewOpp(int responsableWorkerId, long taskId)
        {
            EMPLOYEE responsableEmployee = RepWriter.GetEmployee(responsableWorkerId);

            if (responsableEmployee == null)
            {
                Debug.Log("::CRITICAL:: Unable to find employee to assign opportunity to. responsable_employee_id informed was " + responsableWorkerId.ToString());
                return false;
            }

            OPPORTUNITY assignedOpp = RepWriter.GetOpportunity(taskId);

            if (assignedOpp == null)
            {
                Debug.Log("::CRITICAL:: Unable to find opportunity to assign employee to. opportunity_id informed was " + taskId.ToString());
                return false;
            }

            if (responsableEmployee.department_id != RepWriter.GetProcess(assignedOpp.current_process_id ?? 0).department_id)
            {
                Debug.Log("::CRITICAL:: Assigning opportunity which first process' department is not the same as the responsable employee's deparment.");
                return false;
            }

            assignedOpp.responsable_employee_id = responsableEmployee.id;
            RepWriter.UpdateOpportunity(assignedOpp);
            return true;
        }
        
        /// <summary>
        /// Dissasigns task from worker
        /// </summary>
        /// <param name="responsableWorkerId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public bool DisassignOpp(int responsableWorkerId, long taskId)
        {
            EMPLOYEE responsableEmployee = RepWriter.GetEmployee(responsableWorkerId);

            if (responsableEmployee == null)
            {
                Debug.Log("::CRITICAL:: Unable to find worker to disassign opportunity from. responsable_worker_id informed was " + responsableWorkerId.ToString());
                return false;
            }

            OPPORTUNITY assignedOpp = RepWriter.GetOpportunity(taskId);

            if (assignedOpp == null)
            {
                Debug.Log("::CRITICAL:: Unable to find opportunity to disassign employee from. opportunity_id informed was " + taskId.ToString());
                return false;
            }

            assignedOpp.responsable_employee_id = null;
            RepWriter.UpdateOpportunity(assignedOpp);

            return true;
        }
        
        public EMPLOYEE ValidateLogIn(string username, string password)
        {
            if (username != null && username != "" && password != null && password != "")
            {
                EMPLOYEE loggingEmployee = RepWriter.GetEmployee(username);

                if (loggingEmployee == null)
                {
                    return null;
                }

                else if (password.Equals(loggingEmployee.password))
                {
                    return loggingEmployee;
                }
            }

            return null;
        }
        
    }
}
