using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

public class OpportunityManagerScript : MonoBehaviour
{
    private RepositoryWriter _repWriter;

    private void OnEnable()
    {
        StartCoroutine(LookForRepositoryReader());
    }    
    
    #region Creation and Destruction

    /// <summary>
    /// Creates a new task if all the criteria is met. Oppis set to null otherwise.
    /// </summary>
    /// <param name="newOpp"></param>
    public void CreateOpportunity(ref OPPORTUNITY creatingOpportunity)
    {
        if (creatingOpportunity == null)
        {
            creatingOpportunity = null;
            return;
        }

        #region Validation of product and account

        //Checks product existence
        PRODUCT taskProduct = _repWriter.GetProduct(creatingOpportunity.product_id);
        if (taskProduct == null)
        {
            Debug.Log("::CRITICAL:: Attempt to add task with an unexisting product. product_id informed was " + creatingOpportunity.product_id.ToString());
            creatingOpportunity = null;
            return;
        }

        //Checks account existence
        ACCOUNT task
            = _repWriter.GetAccount(creatingOpportunity.account_id);
        if (taskAccount == null)
        {
            Debug.Log("::CRITICAL:: Attempt to add task for an unexisting account. account_id informed was " + creatingOpportunity.account_id.ToString());
            creatingOpportunity = null;
            return;
        }

        #endregion

        #region if first_process or final_process comes 0, consider the first/final process of given product

        if (creatingOpportunity.current_process_id == null || creatingOpportunity.current_process_id == 0)
        {
            creatingOpportunity.current_process_id = _repWriter.GetProcess(taskProduct.first_process_id).id;
        }

        else
        {
            PROCESS firstProcess = _repWriter.GetProcess((int)creatingOpportunity.current_process_id);

            if (firstProcess == null)
            {
                Debug.Log("::CRITICAL:: Attempt to add a new task with an unexisting process. current_process_id informed was " + creatingOpportunity.current_process_id.ToString());
                creatingOpportunity = null;
                return;
            }
        }

        //------------------------------------------------------------------------------------------------------------------------

        if (creatingOpportunity.final_process_id == 0)
        {
            creatingOpportunity.final_process_id = _repWriter.GetProcess(taskProduct.final_process_id).id;
        }

        else
        {
            PROCESS finalProcess = _repWriter.GetProcess(creatingOpportunity.final_process_id);

            if (finalProcess == null)
            {
                Debug.Log("::CRITICAL:: Attempt to add a new task with an unexisting process. final_process_id informed was " + creatingOpportunity.final_process_id.ToString());
                creatingOpportunity = null;
                return;
            }
        }

        #endregion

        #region Validate responsable worker and the bussiness layer association with the first process

        EMPLOYEE wProfile = _repWriter.GetEmployee(creatingOpportunity.responsable_employee_id ?? 0);

        if (wProfile == null)
        {
            Debug.Log("::CRITICAL:: Attempt to add new task with an unexisting responsable worker. responsable_worker_id infomed was " + creatingOpportunity.responsable_employee_id.ToString());
            creatingOpportunity = null;
            return;
        }

        // if the worker business layer is not the same as the first process business layer
        else if (wProfile.department_id != _repWriter.GetProcess(creatingOpportunity.current_process_id ?? 0).department_id)
        {
            Debug.Log("::CRITICAL:: Attempt to add task with the first process business' layer unequal with the responsable's worker department");
            creatingOpportunity = null;
            return;
        }

        #endregion

        creatingOpportunity.start_date = DateTime.Now;

        //If the name came empty, creates the task with the concatenation of the account and the product
        if (string.IsNullOrEmpty(creatingOpportunity.opportunity_name.Trim()))
        {
            Debug.Log("::NOTE:: Creating a task without a set name. (Using the name of the account and product instead)");
            creatingOpportunity.opportunity_name = taskAccount.account_name + " - " + taskProduct.product_name;
        }

        creatingOpportunity.id = _repWriter.GetAllOpps().Max(t => t.id) + 1;
        _repWriter.AddOpportunity(creatingOpportunity);

        AddNewHistoryOfOpportunity(creatingOpportunity.id, (int)creatingOpportunity.current_process_id, (int)creatingOpportunity.responsable_employee_id);
    }

    /// <summary>
    /// NEEDS ATTENTION ON LOG! Ends task, stops timer, free responsable worker and destroys the pen
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns><b>true</b> if the task was succesfully ended.<b>false</b> otherwise</returns>
    public void EndOpportunity(ref OPPORTUNITY endingOpp)
    {        
        if (_repWriter.GetOpportunity(endingOpp.id) == null)
        {
            Debug.Log("::CRITICAL:: Attempt to finish unexisting opportunity. task_id informed was " + endingOpp.ToString());
            endingOpp = null;
            return;
        }

        Debug.Log("Finishing task " + endingOpp.opportunity_name);

        endingOpp.end_date = DateTime.Now;
        endingOpp.responsable_employee_id = null;

        _repWriter.UpdateOpportunity(endingOpp);
        AddNewHistoryOfOpportunity(endingOpp.id, endingOpp.current_process_id ?? 0, endingOpp.responsable_employee_id ?? 0);

        Debug.Log("Attention! Is this task updated on the list? " + (_repWriter.GetOpportunity(endingOpp.id).end_date == endingOpp.end_date));
    }

    #endregion
    
    #region Change current process

    /// <summary>
    /// Advances to next process if all the critetia is met. 
    /// </summary>
    /// <param name="advancingOpp">Becomes null if criteria was not met</param>
    public void AdvanceToNextProcess(ref OPPORTUNITY advancingOpp)
    {
        if(advancingOpp == null)
        {
            Debug.Log("::CRITICAL:: Attempt to advance an null opportunity");
            return;
        }

        OPPORTUNITY currentOpp = _repWriter.GetOpportunity(advancingOpp.id);

        if (currentOpp == null)
        {
            Debug.Log("::CRITICAL:: Attempt to advance an unexistent opportunity. Informed opportunity_id was: " + advancingOpp.id.ToString());
            advancingOpp = null;
            return;
        }

        PROCESS nextProcess = _repWriter.GetProcess(advancingOpp.current_process_id ?? 0);
        
        if(nextProcess == null)
        {
            Debug.Log("::CRITICAL:: Attempt to advance opportunity to a process which is not the next of the current process. Next process' id was: " + advancingOpp.current_process_id.ToString());
            advancingOpp = null;
            return;
        }

        else if (!_repWriter.GetProcess(currentOpp.current_process_id ?? 0).next_processes_ids.Contains(nextProcess.id))
        {
            Debug.Log("::CRITICAL:: Attempt to advance opportunity to a process which is not the next of the current process. Next process' id was: " + nextProcess.id.ToString());
            advancingOpp = null;
            return;
        }

        else if(!nextProcess.repetable)
        {
            if(advancingOpp.unrepetable_past_processes.Contains(nextProcess.id))
            {
                Debug.Log("::CRITICAL:: Attempt to advance opportunity to an unrepetable process already past. Unrepetable process' id was: " + nextProcess.id.ToString());
                advancingOpp = null;
                return;
            }

            else
            {
                List<int> unrepetableProcessesPastList = advancingOpp.unrepetable_past_processes.ToList();
                unrepetableProcessesPastList.Add(nextProcess.id);
                advancingOpp.unrepetable_past_processes = unrepetableProcessesPastList.ToArray();
            }
        }
                
        if (_repWriter.GetEmployee(advancingOpp.responsable_employee_id ?? 0) == null)
        {
            Debug.Log("::CRITICAL:: Attempt to advance opportunity to unfound responsable worker. responsable_worker_id was: " + advancingOpp.responsable_employee_id.ToString());
            advancingOpp = null;
            return;
        }

        //Validation complete

        _repWriter.UpdateOpportunity(advancingOpp);
        AddNewHistoryOfOpportunity(advancingOpp.id, advancingOpp.current_process_id ?? 0, advancingOpp.responsable_employee_id ?? 0);
        return;
    }

    /// <summary>
    /// Retreats the process.
    /// </summary>
    /// <returns><c>true</c>, if task was retreated, <c>false</c> otherwise.</returns>
    public void RetreatToPreviousProcess(ref OPPORTUNITY retreatingOpp)
    {
        if (retreatingOpp == null)
        {
            Debug.Log("::CRITICAL:: Attempt to retreat an null opportunity");
            return;
        }

        OPPORTUNITY currentOpp = _repWriter.GetOpportunity(retreatingOpp.id);

        if (currentOpp == null)
        {
            Debug.Log("::CRITICAL:: Attempt to retreat an unexistent opportunity. Informed opportunity_id was: " + retreatingOpp.id.ToString());
            retreatingOpp = null;
            return;
        }

        int nextProcessId = retreatingOpp.current_process_id ?? 0;
        
        if (_repWriter.GetNextProcessesOf(retreatingOpp.current_process_id ?? 0).FirstOrDefault(p => p.id == nextProcessId) == null)
        {
            Debug.Log("::CRITICAL:: Attempt to retreat opportunity to process which is not the previous to the current process. Next process' id was: " + nextProcessId.ToString());
            retreatingOpp = null;
            return;
        }

        if (_repWriter.GetEmployee(retreatingOpp.responsable_employee_id ?? 0) == null)
        {
            Debug.Log("::CRITICAL:: Attempt to retreat opportunity to unfound responsable worker. responsable_employee_id was: " + retreatingOpp.responsable_employee_id.ToString());
            retreatingOpp = null;
            return;
        }

        //Validation complete

        _repWriter.UpdateOpportunity(retreatingOpp);
        AddNewHistoryOfOpportunity(retreatingOpp.id, retreatingOpp.current_process_id ?? 0, retreatingOpp.responsable_employee_id ?? 0);
        return;
    }

    #endregion

    #region Responsable Opportunity

    public void TransferOpportunity(ref OPPORTUNITY transferingOpp)
    {
        if (transferingOpp == null)
        {
            Debug.Log("::CRITICAL:: Attempt to transfer an unexistent opportunity. Informed opportunity_id was: " + transferingOpp.id.ToString());
            return;
        }

        if (_repWriter.GetEmployee(transferingOpp.responsable_employee_id ?? 0) == null)
        {
            Debug.Log("::CRITICAL:: Attempt to tranfer opportunity to unexisting employee. Informed opportunity_id was: " + transferingOpp.id.ToString() + ". Informed responsable_employee_id was: " + transferingOpp.id.ToString());
            transferingOpp = null;
            return;
        }

        //Validation complete

        _repWriter.UpdateOpportunity(transferingOpp);
        AddNewHistoryOfOpportunity(transferingOpp.id, transferingOpp.current_process_id ?? 0, transferingOpp.responsable_employee_id ?? 0);
        return;
    }

    #endregion

    #region Add History

    private void AddNewHistoryOfOpportunity(long taskId, int currentProcessId, int responsableWorkerId)
    {


        OPPORTUNITY_HISTORY newOppHistory = OPPORTUNITY_HISTORY.CreateInstance<OPPORTUNITY_HISTORY>();
        newOppHistory.SetOppHistory(_repWriter.GetAllOppHistories().Max(h => h.id) + 1, taskId, currentProcessId, responsableWorkerId, DateTime.Now);
        
        OPPORTUNITY_HISTORY latestHistoryOfOpp = _repWriter.GetLatestOppHistoryOf(taskId);

        //if task has previous history
        if (latestHistoryOfOpp != null)
        {
            //And makes the penultimate point to the last (the now)
            latestHistoryOfOpp.next_history_id = newOppHistory.id;
            //Add the new History
            _repWriter.AddOppHistory(newOppHistory);
            //Updates the previously latest taskHistory so it points to the new OppHistory
            _repWriter.UpdateOppHistory(latestHistoryOfOpp);
        }

        else
        {
            _repWriter.AddOppHistory(newOppHistory);
        }
    }

    #endregion


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
