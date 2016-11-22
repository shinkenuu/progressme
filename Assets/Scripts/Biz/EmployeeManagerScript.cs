using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

public class EmployeeManagerScript : MonoBehaviour
{
    
    private RepositoryWriter _repWriter;

    private void OnEnable()
    {
        StartCoroutine(LookForRepositoryReader());
    }


    /// <summary>
    /// Assigns task to worker
    /// </summary>
    /// <param name="responsableWorkerId"></param>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public bool AssignNewOpp(int responsableWorkerId, long taskId)
    {
        EMPLOYEE responsableEmployee = _repWriter.GetEmployee(responsableWorkerId);

        if (responsableEmployee == null)
        {
            Debug.Log("::CRITICAL:: Unable to find employee to assign opportunity to. responsable_employee_id informed was " + responsableWorkerId.ToString());
            return false;
        }

        OPPORTUNITY assignedOpp = _repWriter.GetOpportunity(taskId);

        if (assignedOpp == null)
        {
            Debug.Log("::CRITICAL:: Unable to find opportunity to assign employee to. opportunity_id informed was " + taskId.ToString());
            return false;
        }

        if (responsableEmployee.department_id != _repWriter.GetProcess(assignedOpp.current_process_id ?? 0).department_id)
        {
            Debug.Log("::CRITICAL:: Assigning opportunity which first process' department is not the same as the responsable employee's deparment.");
            return false;
        }

        assignedOpp.responsable_employee_id = responsableEmployee.id;
        _repWriter.UpdateOpportunity(assignedOpp);
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
        EMPLOYEE responsableEmployee = _repWriter.GetEmployee(responsableWorkerId);

        if (responsableEmployee == null)
        {
            Debug.Log("::CRITICAL:: Unable to find worker to disassign opportunity from. responsable_worker_id informed was " + responsableWorkerId.ToString());
            return false;
        }

        OPPORTUNITY assignedOpp = _repWriter.GetOpportunity(taskId);

        if (assignedOpp == null)
        {
            Debug.Log("::CRITICAL:: Unable to find opportunity to disassign employee from. opportunity_id informed was " + taskId.ToString());
            return false;
        }

        assignedOpp.responsable_employee_id = null;
        _repWriter.UpdateOpportunity(assignedOpp);

        return true;
    }
    
    public EMPLOYEE ValidateLogIn(string username, string password)
    {
        if (username != null && username != "" && password != null && password != "")
        {
            EMPLOYEE loggingEmployee = _repWriter.GetEmployee(username);

            if(loggingEmployee == null)
            {
                return null;
            }

            else if(password.Equals(loggingEmployee.password))
            {
                return loggingEmployee;
            }
        }

        return null;
    }

    
    #region LookFor

    private IEnumerator LookForRepositoryReader()
    {
        GameObject dataGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            dataGo = GameObject.FindWithTag("Storage");

            if (dataGo != null)
            {
                _repWriter = dataGo.GetComponent<RepositoryWriter>();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    #endregion
}
