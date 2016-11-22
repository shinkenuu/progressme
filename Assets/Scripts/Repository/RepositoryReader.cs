using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Model;

public class RepositoryReader : MonoBehaviour
{
    protected readonly string DocumentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ProgressMe\";
    protected readonly string AppDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ProgressMe\";

    public enum FilesDirectory
    {
        Client = 0,
        Server = 1
    }

    public static bool Fresh
    {
        get;
        protected set;
    }

    protected static IEnumerable<DEPARTMENT> Departments;
    protected static IEnumerable<PROCESS> Processes;
    protected static IEnumerable<PRODUCT> Products;
    protected static List<ACCOUNT> Accounts;
    protected static IEnumerable<EMPLOYEE> Employees;
    protected static List<OPPORTUNITY> Opportunities;
    protected static List<OPPORTUNITY_HISTORY> OpportunityHistories;
    
    #region Departments
    
    public IEnumerable<DEPARTMENT> GetAllDepartments() { return Departments; }

    public DEPARTMENT GetDepartment(int departmentId) { return Departments.FirstOrDefault(b => b.id == departmentId); }

    public DEPARTMENT GetDepartment(string departmentName) { return Departments.FirstOrDefault(b => b.department_name == departmentName); }

    #endregion

    #region Process

    public IEnumerable<PROCESS> GetAllProcesses() { return Processes; }

    public PROCESS GetProcess(int processId) { return Processes.FirstOrDefault(p => p.id == processId); }

    public PROCESS GetProcess(string processName) { return Processes.FirstOrDefault(p => p.process_name == processName); }
    
    public IEnumerable<PROCESS> GetNextProcessesOf(int processId)
    {
        PROCESS curProcess = Processes.FirstOrDefault(p => p.id == processId);

        if(curProcess == null)
        {
            return null;
        }

        List<PROCESS> processesAfter = new List<PROCESS>();

        foreach (int nextProcessId in curProcess.next_processes_ids)
        {
            processesAfter.Add(Processes.FirstOrDefault(p => p.id == nextProcessId));
        }

        return processesAfter as IEnumerable<PROCESS>;
    }
    
    public int SumOfMinimalIdealHours()
    {
        return Processes.Where(p => !p.optional).Sum(p => p.ideal_total_hours);
    }

    /// <summary>
    /// Calculates the sum of ideal total hours between two processes. It doesn't counts optinal process in the way, unless it is the ealier or later process
    /// </summary>
    /// <param name="earlierProcess"></param>
    /// <param name="laterProcess"></param>
    /// <returns>The sum of the ideal_total_hours between the processes. -1 if the calcutation failed</returns>
    public int CalculateHoursBetween(PROCESS earlierProcess, PROCESS laterProcess, PRODUCT product)
    {
        if (!Processes.Contains(earlierProcess) || !Processes.Contains(laterProcess) || GetProduct(product.id))
        {
            return -1;
        }

        List<PROCESS> visitedProcesses = new List<PROCESS>();
        PROCESS curProcess = GetProcess(earlierProcess.id);
        PROCESS tempProcess;

        visitedProcesses.Add(curProcess);

        while (curProcess.id != product.final_process_id && curProcess.id != laterProcess.id)
        {
            //stress all the possibilities of next process of current process
            foreach (int nextProcessId in curProcess.next_processes_ids)
            {
                tempProcess = GetProcess(nextProcessId);
                
                //if this process has NOT been visited 
                if(visitedProcesses.FirstOrDefault(p => p.id == tempProcess.id) == null)
                {
                    curProcess = tempProcess;
                    visitedProcesses.Add(curProcess);
                    break;
                }     
            }
        }

        if (laterProcess.id == curProcess.id)
        {
            return visitedProcesses.Sum(p => p.ideal_total_hours);
        }

        else
        {
            return -1;
        }

    }

    /// <summary>
    /// Gets the Process that a task should ideally be at after some hoursPastSinceBeginning
    /// </summary>
    /// <param name="hoursPastSinceBeginning">Hours past since the beggining of the task</param>
    /// <param name="product"></param>
    /// <returns>The process in wich the task should be after given hoursPastHours for given product</returns>
    public PROCESS GetIdealProcessAfterPastHours(int hoursPastSinceBeginning, PRODUCT product)
    {
        if (hoursPastSinceBeginning < 0 || GetProduct(product.id) == null)
        {
            return null;
        }

        if (hoursPastSinceBeginning >= SumOfMinimalIdealHours())
        {
            return GetProcess(product.final_process_id);
        }

        List<PROCESS> visitedProcesses = new List<PROCESS>();
        PROCESS curProcess = GetProcess(product.first_process_id);
        PROCESS tempProcess;
        int finalProcessOfProductId = GetProcess(product.final_process_id).id;

        visitedProcesses.Add(curProcess);

        while (visitedProcesses.Sum(p => p.ideal_total_hours) < hoursPastSinceBeginning)
        {
            foreach (int nextProcessId in curProcess.next_processes_ids)
            {
                tempProcess = GetProcess(nextProcessId);

                //If the next process wasn't visited yet
                if (visitedProcesses.FirstOrDefault(p => p.id == nextProcessId) == null)
                {
                    curProcess = GetProcess(tempProcess.id);

                    //If current process is final
                    if(curProcess.id == finalProcessOfProductId)
                    {
                        return curProcess;
                    }

                    visitedProcesses.Add(curProcess);
                    break;
                }
            }
        }
        
        return curProcess;
    }
    
    #endregion

    #region Products

    public IEnumerable<PRODUCT> GetAllProducts() { return Products; }

    public PRODUCT GetProduct(int id) { return Products.FirstOrDefault(p => p.id == id); }

    public PRODUCT GetProduct(string productName) { return Products.FirstOrDefault(p => p.product_name == productName); }
    
    #endregion

    #region Accounts

    public IEnumerable<ACCOUNT> GetAllAccounts() { return Accounts as IEnumerable<ACCOUNT>; }

    public ACCOUNT GetAccount(int accountId) { return Accounts.FirstOrDefault(c => c.id == accountId); }

    public ACCOUNT GetAccount(string accountName) { return Accounts.FirstOrDefault(c => c.account_name == accountName); }

    #endregion
    
    #region Employees

    public IEnumerable<EMPLOYEE> GetAllEmployees() { return Employees; }

    public EMPLOYEE GetEmployee(int employeeId) { return Employees.FirstOrDefault(wp => wp.id == employeeId); }

    public EMPLOYEE GetEmployee(string employeeName) { return Employees.FirstOrDefault(wp => wp.employee_name == employeeName); }

    public IEnumerable<EMPLOYEE> GetEmployeesIn(int departmentId) { return Employees.Where(wp => wp.department_id == departmentId); }

    public IEnumerable<EMPLOYEE> GetEmployeesReportingTo(int managerId) { return Employees.Where(wp => wp.manager_id == managerId); }
    
    /// <summary>
    /// NEEDS TESTING !!!!!!!!!
    /// </summary>
    /// <param name="managerId"></param>
    /// <returns></returns>
    public IEnumerable<EMPLOYEE> GetWorkersReportingToRecursive(int managerId)
    {
        List<EMPLOYEE> allReporters = GetEmployeesReportingTo(managerId).ToList();

        if(allReporters == null || allReporters.Count == 0)
        {
            return null;
        }

        //Removes self manager
        allReporters.RemoveAll(wp => wp.id == managerId);

        EMPLOYEE[] immediateReporters = allReporters.ToArray();

        int lastCheckedWorkerIdx = 0;
        int totalChecksNeeded = immediateReporters.Count();

        while(lastCheckedWorkerIdx < totalChecksNeeded)
        {
            foreach (EMPLOYEE worker in immediateReporters.Skip(lastCheckedWorkerIdx))
            {
                allReporters.AddRange(GetEmployeesReportingTo(worker.id));
                lastCheckedWorkerIdx++;
            }

            immediateReporters = allReporters.ToArray();
            totalChecksNeeded = allReporters.Count();
        }


        return allReporters;
    }

    #endregion

    #region Opportunities

    public IEnumerable<OPPORTUNITY> GetAllOpps() { return Opportunities; }

    public IEnumerable<OPPORTUNITY> GetOppsAssignedTo(int employeeId) { return Opportunities.Where(t => t.responsable_employee_id == employeeId); }

    public OPPORTUNITY GetOpportunity(long id) { return Opportunities.FirstOrDefault(t => t.id == id); }

    #endregion

    #region Opp Histories

    public IEnumerable<OPPORTUNITY_HISTORY> GetAllOppHistories() { return OpportunityHistories; }

    public IEnumerable<OPPORTUNITY_HISTORY> GetOppHistoryOfWith(long taskId, int responsableEmployeeId) { return OpportunityHistories.Where(h => h.task_id == taskId && h.responsable_employee_id == responsableEmployeeId); }

    public IEnumerable<OPPORTUNITY_HISTORY> GetOppHistoryOfOn(long taskId, int processId) { return OpportunityHistories.Where(h => h.task_id == taskId && h.process_id == processId); }

    public IEnumerable<OPPORTUNITY_HISTORY> GetEntireOppHistoryOf(long taskId) { return OpportunityHistories.Where(h => h.task_id == taskId); }


    public OPPORTUNITY_HISTORY GetFirstOppHistoryOf(long taskId) { return OpportunityHistories.Where(th => th.task_id == taskId).OrderBy(th => th.history_date).FirstOrDefault(); }

    public OPPORTUNITY_HISTORY GetPenultimateOppHistoryOf(long taskId) { return OpportunityHistories.Where(th => th.task_id == taskId).OrderByDescending(th => th.history_date).ElementAt(1); }

    public OPPORTUNITY_HISTORY GetLatestOppHistoryOf(long taskId) { return OpportunityHistories.Where(th => th.task_id == taskId).OrderBy(th => th.history_date).LastOrDefault(); }


    public IEnumerable<OPPORTUNITY_HISTORY> GetFirstAndLastOppHistoryOf(long taskId)
    {
        List<OPPORTUNITY_HISTORY> askedHistory = new List<OPPORTUNITY_HISTORY>();

        OPPORTUNITY_HISTORY tHistory = OpportunityHistories.Where(th => th.task_id == taskId).OrderBy(th => th.history_date).FirstOrDefault();

        if(tHistory == null)
        {
            return null;
        }

        tHistory = OpportunityHistories.Where(th => th.task_id == taskId).OrderBy(th => th.history_date).LastOrDefault();
        
        if(tHistory == null)
        {
            return null;
        }

        askedHistory.Add(tHistory);
        return askedHistory as IEnumerable<OPPORTUNITY_HISTORY>;
            
    }

    public IEnumerable<OPPORTUNITY_HISTORY> GetFirstAndPenultimateAndLastOppHistoryOf(long taskId)
    {
        List<OPPORTUNITY_HISTORY> askedHistory = OpportunityHistories.Where(h => h.task_id == taskId).OrderBy(h => h.id).ToList();

        //If there is distinct first and penultimate and last history
        if (askedHistory.Count > 2)
        {
            //long penultimateHistoryIdOfAskedOpp = askedHistory[askedHistory.Count - 2].id;

            //Skips the first, remove a amount of history equal to (totalCount - firstHistory) - (penultiHistory + lastHistory)
            askedHistory.RemoveRange(1, askedHistory.Count() - 3);
            //askedHistory.RemoveAll(th => th != askedHistory.First() && th != askedHistory.Last() && th.id != penultimateHistoryIdOfAskedOpp);
        }

        //If there is first and last only
        else if (askedHistory.Count == 2)
        {
            //duplicate the first, so it turns to be the penultimate
            askedHistory.Add(askedHistory.First());
            //duplicate the last to maintain the order position
            askedHistory.Add(askedHistory[1]);
            //delete the last duplicate in thw wrong position
            askedHistory.RemoveAt(1);
        }

        //else the same history is the first, penultimate and last
        else //there is only 1 history
        {
            askedHistory.Add(askedHistory.First());
            askedHistory.Add(askedHistory.First());
        }

        return askedHistory as IEnumerable<OPPORTUNITY_HISTORY>;

    }

    public IEnumerable<OPPORTUNITY_HISTORY> GetPenultimateAndLastOppHistoryOf(long taskId)
    {
        List<OPPORTUNITY_HISTORY> askedHistory = OpportunityHistories.Where(h => h.task_id == taskId).OrderBy(h => h.id).ToList();

        //If there is distinct penultimate and last history, or even just the penultimate and the last
        if (askedHistory.Count > 1)
        {
            askedHistory.RemoveRange(0, askedHistory.Count() - 2);
            //askedHistory.RemoveAll(th => th != askedHistory.First() && th != askedHistory.Last());
        }

        //else if the penultimate is the same as the last
        else
        {
            //duplicate the last, because it is the penultimate and the last too
            askedHistory.Add(askedHistory.Last());
        }

        return askedHistory as IEnumerable<OPPORTUNITY_HISTORY>;

    }

    #endregion
     

}
