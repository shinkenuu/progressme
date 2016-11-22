using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

[RequireComponent(typeof(OpportunityManagerScript))]
[RequireComponent(typeof(EmployeeManagerScript))]
[RequireComponent(typeof(AccountManagerScript))]
public class ServerManagerScript : MonoBehaviour {
    
    private RepositoryWriter _repWriter;
    private GameManagerScript GameMng;

    private OpportunityManagerScript _oppMng;
    private AccountManagerScript _custmrMng;

    private void OnEnable()
	{
        _oppMng = gameObject.GetComponent<OpportunityManagerScript>();
        _custmrMng = gameObject.GetComponent<AccountManagerScript>();
        StartCoroutine(LookForManagerGo());

        _repWriter = GameObject.Find("DataGO").GetComponent<RepositoryWriter>();
        _repWriter.LoadServerDataFromDisc();
    }
    

    #region Opps
    
    /// <summary>
    /// Checks task data and created if everything was OK. Set task to null otherwise.
    /// </summary>
    /// <param name="newOpp"></param>
    public void CreateOpportunity(ref OPPORTUNITY newOpportunity)
    {
        GameMng.LogIntoServerDisplay("Opp " + newOpportunity.opportunity_name + " has arrived for creation");
        _oppMng.CreateOpportunity(ref newOpportunity);

        if (newOpportunity == null)
        {
            GameMng.LogIntoServerDisplay("Opp wasn't created");
            Debug.Log("::ATTENTION:: Opp asked to be created was not created");
            return;
        }

        GameMng.LogIntoServerDisplay("Opp " + newOpportunity.opportunity_name + " created");
        //puppetManager.OrderSigningOfPen(newOpp, true);
    }

    /// <summary>
    /// Checks task data and advances if everything was OK. Set task to null otherwise.
    /// </summary>
    /// <param name="newOpp"></param>
    public void AdvanceOpp(ref OPPORTUNITY advancingOpp)
    {
        GameMng.LogIntoServerDisplay("Opportunity " + advancingOpp.opportunity_name + " has arrived for advancement");
        _oppMng.AdvanceToNextProcess(ref advancingOpp);

        if (advancingOpp == null)
        {
            Debug.Log("::ATTENTION:: Opportunity asked to be advanced was not");
            return;
        }

        GameMng.LogIntoServerDisplay("Opportunity " + advancingOpp.opportunity_name + " advanced");
        //puppetManager.OrderSigningOfPen(newOpp, true);
    }
    
    /// <summary>
    /// Checks task data and retreats if everything was OK. Set task to null otherwise.
    /// </summary>
    /// <param name="newOpp"></param>
    public void RetreatOpp(ref OPPORTUNITY retreatingOpp)
    {
        GameMng.LogIntoServerDisplay("Opp " + retreatingOpp.opportunity_name + " has arrived for retreatment");
        _oppMng.RetreatToPreviousProcess(ref retreatingOpp);

        if (retreatingOpp == null)
        {
            Debug.Log("::ATTENTION:: Opportunity asked to be retreated was not");
            return;
        }

        GameMng.LogIntoServerDisplay("Opportunity " + retreatingOpp.opportunity_name + " retreated");
        //puppetManager.OrderSigningOfPen(newOpp, true);
    }
    
    /// <summary>
    /// Checks task data and transfer if everything was OK. Set task to null otherwise.
    /// </summary>
    /// <param name="newOpp"></param>
    public void TransferOpp(ref OPPORTUNITY transferingOpp)
    {
        GameMng.LogIntoServerDisplay("Opportunity " + transferingOpp.opportunity_name + " has arrived for transfering");
        _oppMng.TransferOpportunity(ref transferingOpp);

        if (transferingOpp == null)
        {
            Debug.Log("::ATTENTION:: Opportunity asked to be transfered");
            return;
        }

        GameMng.LogIntoServerDisplay("Opportunity " + transferingOpp.opportunity_name + " transfered");
        //puppetManager.OrderSigningOfPen(newOpp, true);
    }
    
    /// <summary>
    /// Checks task data and ends if everything was OK. Set task to null otherwise.
    /// </summary>
    /// <param name="newOpp"></param>
    public void EndOpp(ref OPPORTUNITY endingOpp)
    {
        GameMng.LogIntoServerDisplay("Opportunity " + endingOpp.opportunity_name + " has arrived for ending");
        _oppMng.EndOpportunity(ref endingOpp);

        if (endingOpp == null)
        {
            Debug.Log("::ATTENTION:: Opportunity asked to be ended was not");
            return;
        }

        GameMng.LogIntoServerDisplay("Opportunity " + endingOpp.opportunity_name + " ended");
        //puppetManager.OrderSigningOfPen(newOpp, true);
    }

    #endregion


    #region Account

    public void CreateAccount(ref ACCOUNT newAccount)
    {
        GameMng.LogIntoServerDisplay("Account " + newAccount.account_name + ", of type " + newAccount.account_type + " has arrived for creation");
        _custmrMng.CreateAccount(ref newAccount);

        if (newAccount == null)
        {
            GameMng.LogIntoServerDisplay("Account wasn't created");
            Debug.Log("::ATTENTION:: Account asked to be created was not created");
            return;
        }

        GameMng.LogIntoServerDisplay("Account " + newAccount.account_name + " has been created");
    }

    #endregion
    
    // -----------------------------------------------------------------

    #region ReportsOrdering

    /// <summary>
    /// NEEDS CODING!!
    /// </summary>
    /// <param name="taskId"></param>
    /// <returns></returns>
    public bool OrderReport(long taskId)
    {
        GameMng.LogIntoServerDisplay("Reporting is not implemented yet!");
        return false;
    }

    #endregion


    #region LookFor

    private IEnumerator LookForManagerGo()
    {
        GameObject mngsGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            mngsGo = GameObject.Find("MngGO");
            
            if (mngsGo != null)
            {
                GameMng = mngsGo.GetComponent<GameManagerScript>(); 
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }

    }

    #endregion
      

}
