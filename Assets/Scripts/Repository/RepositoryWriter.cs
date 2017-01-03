using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Model;
using System.Text;

public class RepositoryWriter : RepositoryReader
{
    
    public void LoadServerDataFromDisc()
    {
        //InstantiateDepartments();
        //SaveDepartmentsToDisc();
        //InstantiateProcesses();
        //SaveProcessesToDisc();
        //InstantiateProducts();
        //SaveProductsToDisc();
        //InstantiateAccounts();
        //SaveAccountsToDisc();
        //InstantiateEmployees();
        //SaveEmployeesToDisc();

        //return;

        LoadDepartments(SafeReadFromDisc("DEPARTMENT.json"));
        LoadProcesses(SafeReadFromDisc("PROCESS.json"));
        LoadProducts(SafeReadFromDisc("PRODUCT.json"));
        LoadAccounts(SafeReadFromDisc("ACCOUNT.json"));
        LoadEmployees(SafeReadFromDisc("EMPLOYEE.json"));
        LoadOpportunities(SafeReadFromDisc("OPPORTUNITY.json"));
        LoadOppHistories(SafeReadFromDisc("OPPORTUNITY_HISTORY.json"));

        StartCoroutine(ScheduleBackup(1f));
    }
    
    #region Departments
    
    public void SetDepartments(IEnumerable<DEPARTMENT> data) { Departments = data; }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SaveDepartmentsToDisc()
    {
        StringBuilder sBuilder = new StringBuilder();

        foreach(DEPARTMENT jsonable in Departments)
        {
            sBuilder.AppendLine(jsonable.ToJson().Print());
        }

        SafeWriteToDisc(sBuilder.ToString(), FilesDirectory.Server, "DEPARTMENT.json");
    }

    public void LoadDepartments(string[] jsonArray)
    {
        if(jsonArray == null || jsonArray.Count() == 0)
        {
            Debug.Log("::CRITICAL:: Error loading DEPARTMENT");
            return;
        }
        
        List<DEPARTMENT> jsonableList = new List<DEPARTMENT>();
        DEPARTMENT jsonable;

        foreach(string json in jsonArray)
        {
            jsonable = DEPARTMENT.CreateInstance<DEPARTMENT>();
            jsonable.FromJson(json);
            jsonableList.Add(jsonable);
        }

        Departments = jsonableList as IEnumerable<DEPARTMENT>;
    }

    #endregion

    #region Process
    
    public void SetProcesses(IEnumerable<PROCESS> data) { Processes = data; }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SaveProcessesToDisc()
    {
        StringBuilder sBuilder = new StringBuilder();

        foreach (PROCESS jsonable in Processes)
        {
            sBuilder.AppendLine(jsonable.ToJson().Print());
        }

        SafeWriteToDisc(sBuilder.ToString(), FilesDirectory.Server, "PROCESS.json");
    }

    public void LoadProcesses(string[] jsonArray)
    {
        if (jsonArray == null || jsonArray.Count() == 0)
        {
            Debug.Log("::CRITICAL:: Error loading PROCESS");
            return;
        }

        List<PROCESS> jsonableList = new List<PROCESS>();
        PROCESS jsonable;

        foreach (string json in jsonArray)
        {
            jsonable = PROCESS.CreateInstance<PROCESS>();
            jsonable.FromJson(json);
            jsonableList.Add(jsonable);
        }

        Processes = jsonableList as IEnumerable<PROCESS>;
    }

    #endregion
    
    #region Product
    
    public void SetProducts(IEnumerable<PRODUCT> data) { Products = data; }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SaveProductsToDisc()
    {
        StringBuilder sBuilder = new StringBuilder();

        foreach (PRODUCT jsonable in Products)
        {
            sBuilder.AppendLine(jsonable.ToJson().Print());
        }

        SafeWriteToDisc(sBuilder.ToString(), FilesDirectory.Server, "PRODUCT.json");
    }

    public void LoadProducts(string[] jsonArray)
    {
        if (jsonArray == null || jsonArray.Count() == 0)
        {
            Debug.Log("::CRITICAL:: Error loading PRODUCT");
            return;
        }

        List<PRODUCT> jsonableList = new List<PRODUCT>();
        PRODUCT jsonable;

        foreach (string json in jsonArray)
        {
            jsonable = PRODUCT.CreateInstance<PRODUCT>();
            jsonable.FromJson(json);
            jsonableList.Add(jsonable);
        }

        Products = jsonableList as IEnumerable<PRODUCT>;
    }


    #endregion

    #region Account

    public void SetAccounts(IEnumerable<ACCOUNT> data) { Accounts = data.ToList(); }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void AddAccount(ACCOUNT newAccount) { Accounts.Add(newAccount); }

    public ACCOUNT AddNewAccount(ACCOUNT newAccount)
    {
        newAccount.id = Accounts.Max(c => c.id) + 1;
        Accounts.Add(newAccount);
        return newAccount;
    }

    public void SaveAccountsToDisc()
    {
        StringBuilder sBuilder = new StringBuilder();

        foreach (ACCOUNT jsonable in Accounts)
        {
            sBuilder.AppendLine(jsonable.ToJson().Print());
        }

        SafeWriteToDisc(sBuilder.ToString(), FilesDirectory.Server, "ACCOUNT.json");
    }

    public void LoadAccounts(string[] jsonArray)
    {
        if (jsonArray == null || jsonArray.Count() == 0)
        {
            Debug.Log("::CRITICAL:: Error loading CUSTOMER");
            return;
        }

        List<ACCOUNT> jsonableList = new List<ACCOUNT>();
        ACCOUNT jsonable;

        foreach (string json in jsonArray)
        {
            jsonable = ACCOUNT.CreateInstance<ACCOUNT>();
            jsonable.FromJson(json);
            jsonableList.Add(jsonable);
        }

        Accounts = jsonableList;
    }

    #endregion

    #region Employee
    
    public void SetEmployees(IEnumerable<EMPLOYEE> data) { Employees = data; }

    // ------------------------------------------------------------------------------------------------------------------------------------------------------------------

    public void SaveEmployeesToDisc()
    {
        StringBuilder sBuilder = new StringBuilder();

        foreach (EMPLOYEE jsonable in Employees)
        {
            sBuilder.AppendLine(jsonable.ToJson().Print());
        }

        SafeWriteToDisc(sBuilder.ToString(), FilesDirectory.Server, "EMPLOYEE.json");
    }

    public void LoadEmployees(string[] jsonArray)
    {
        if (jsonArray == null || jsonArray.Count() == 0)
        {
            Debug.Log("::CRITICAL:: Error loading EMPLOYEE");
            return;
        }

        List<EMPLOYEE> jsonableList = new List<EMPLOYEE>();
        EMPLOYEE jsonable;

        foreach (string json in jsonArray)
        {
            jsonable = EMPLOYEE.CreateInstance<EMPLOYEE>();
            jsonable.FromJson(json);
            jsonableList.Add(jsonable);
        }

        Employees = jsonableList as IEnumerable<EMPLOYEE>;
    }

    #endregion
    
    #region Opportunity

    public void SetOpportunities(List<OPPORTUNITY> opportunityList) { Opportunities = opportunityList; }
    
    public void AddOpportunity(OPPORTUNITY newOpportunity) { Opportunities.Add(newOpportunity); }

    public void UpdateOpportunity(OPPORTUNITY opportunity) { Opportunities.FirstOrDefault(t => t.id == opportunity.id).SetOpportunity(opportunity); }

    public void RemoveOpportunity(OPPORTUNITY opportunity) { Opportunities.RemoveAll(t => t.id == opportunity.id); }

    public void SaveOpportunitiesToDisc()
    {
        StringBuilder sBuilder = new StringBuilder();

        foreach (OPPORTUNITY jsonable in Opportunities)
        {
            sBuilder.AppendLine(jsonable.ToJson().Print());
        }

        SafeWriteToDisc(sBuilder.ToString(), FilesDirectory.Server, "OPPORTUNITY.json");
    }

    public void LoadOpportunities(string[] jsonArray)
    {
        if (jsonArray == null || jsonArray.Count() == 0)
        {
            Debug.Log("::CRITICAL:: Error loading OPPORTUNITY");
            return;
        }

        OPPORTUNITY jsonable;
        Opportunities = new List<OPPORTUNITY>();

        foreach (string json in jsonArray)
        {
            jsonable = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            jsonable.FromJson(json);
            Opportunities.Add(jsonable);
        }
    }

    #endregion

    #region Opp History

    public void SetOppHistories(List<OPPORTUNITY_HISTORY> historiesList) { OpportunityHistories = historiesList; }

    public void AddOppHistory(OPPORTUNITY_HISTORY taskHistory) { OpportunityHistories.Add(taskHistory); }

    public void UpdateOppHistory(OPPORTUNITY_HISTORY taskHistory) { OpportunityHistories.FirstOrDefault(th => th.id == taskHistory.id).SetOppHistory(taskHistory); }

    public void RemoveOppHistory(OPPORTUNITY_HISTORY taskHistory) { OpportunityHistories.Remove(taskHistory); }

    public void SaveOppHistoryToDisc()
    {
        StringBuilder sBuilder = new StringBuilder();

        foreach (OPPORTUNITY_HISTORY jsonable in OpportunityHistories)
        {
            sBuilder.AppendLine(jsonable.ToJson().Print());
        }

        SafeWriteToDisc(sBuilder.ToString(), FilesDirectory.Server, "OPPORTUNITY_HISTORY.json");
    }

    public void LoadOppHistories(string[] jsonArray)
    {
        if (jsonArray == null || jsonArray.Count() == 0)
        {
            Debug.Log("::CRITICAL:: Error loading OPPORTUNITY_HISTORY");
            return;
        }
        
        OPPORTUNITY_HISTORY jsonable;
        OpportunityHistories = new List<OPPORTUNITY_HISTORY>();

        foreach (string json in jsonArray)
        {
            jsonable = OPPORTUNITY_HISTORY.CreateInstance<OPPORTUNITY_HISTORY>();
            jsonable.FromJson(json);
            OpportunityHistories.Add(jsonable);
        }
    }

    #endregion

    public void SetFresh(bool fresh)
    {
        Fresh = fresh;
    }

    #region IO
    
    public void SafeWriteToDisc(string content, FilesDirectory fileDirectory, string fileNameWithExtension)
    {
        string fullPathToFileDirectory;

        if (fileDirectory == FilesDirectory.Server)
        {
            fullPathToFileDirectory = AppDataFolderPath;
        }

        else
        {
            fullPathToFileDirectory = DocumentsFolderPath;
        }
        
        if (!Directory.Exists(fullPathToFileDirectory))
        {
            Directory.CreateDirectory(fullPathToFileDirectory);
        }

        if (!File.Exists(fullPathToFileDirectory + fileNameWithExtension))
        {
            using (new FileStream(fullPathToFileDirectory + fileNameWithExtension, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.ReadWrite)) { }
        }

        using (FileStream fs = new FileStream(fullPathToFileDirectory + fileNameWithExtension, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(content);
            }
        }
    }

    private string[] SafeReadFromDisc(string fileNameWithExtension)
    {
        if (File.Exists(AppDataFolderPath + fileNameWithExtension))
        {
            List<string> content = new List<string>();

            using (FileStream fs = new FileStream(AppDataFolderPath + fileNameWithExtension, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    while(!sr.EndOfStream)
                    {
                        content.Add(sr.ReadLine());
                    }                    
                }
            }

            return content.ToArray();
        }

        return null;
    }

    private IEnumerator ScheduleBackup(float hours)
    {
        while(true)
        {
            yield return new WaitForSeconds(hours * 60 * 60);
            SaveBackup();
        }

        yield break;
    }

    private void SaveBackup()
    {
        SaveAccountsToDisc();
        SaveOpportunitiesToDisc();
        SaveOppHistoryToDisc();
    }

    #endregion

}
