using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

public class HomeGUIScript : UserGUIScript
{

    #region Reference

    [SerializeField]
    private Image userProfileImage;
    [SerializeField]
    private GameObject managerOverviewButtonGo;
    [SerializeField]
    private GameObject createOppButtonGo;
    [SerializeField]
    private Transform oppPanelTransform;
    [SerializeField]
    private Transform oppThumbnailPrefab;
    [SerializeField]
    private Text msgTxt;

    #endregion
    
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    #region Buttons

    public void BtnActionNewOpp()
    {
        Debug.Log("New Opp btn was clicked");
        GameMng.ScheduleProcedure(GameManagerScript.Procedures.OppCreation);
        GameMng.LoadNextWindow();
    }

    public void BtnActionManagerOverview()
    {
        GameMng.ScheduleProcedure(GameManagerScript.Procedures.ManagerOverview);
        GameMng.LoadNextWindow();
    }

    /// <summary>
    /// Quit application
    /// </summary>
    public void BtnActionLogOff()
    {
        Debug.Log("Logoff btn was clicked");
        Application.Quit();
    }

    //-----------------------------------------------------
    
    #endregion



    #region User info
    
    private IEnumerator FillInUserInfo()
    {
        yield return new WaitWhile(() => ( GameMng == null));
        userProfileImage.sprite = Sprite.Create((Texture2D)Resources.Load("Sprites/Employees/" + GameMng._employeeInFocus.employee_name + "/profile"), new Rect(0, 0, 50, 50), new Vector2(0.5f, 0.5f));
        scheduledActions.RemoveAt(0);
        FetchNextAction();
        yield break;
    }
    
    #endregion


    public override void ScheduleAction(UserGuiAction action)
    {
        //IF is not a HomeGui action
        if((int)action > 9)
        {
            return;
        }

        scheduledActions.Add(action);

        if(scheduledActions.Count == 1)
        {
            FetchNextAction();
        }
    }

    protected override void FetchNextAction()
    {
        if(scheduledActions.Count == 0)
        {
            return;
        }

        switch(scheduledActions.First())
        {
            case UserGuiAction.SETUP_HOME:
                StartCoroutine(CheckLoggedEmployeeManagerOverviewCapability());
                StartCoroutine(CheckLoggedEmployeeCreateOppCapability());
                ScheduleAction(UserGuiAction.FILL_USER_INFO);
                ScheduleAction(UserGuiAction.REFRESH_OPPORTUNITY_PANEL);
                scheduledActions.RemoveAt(0);
                FetchNextAction();
                break;
            case UserGuiAction.FILL_USER_INFO:
                StartCoroutine(FillInUserInfo());
                break;
            case UserGuiAction.REFRESH_OPPORTUNITY_PANEL:
                StartCoroutine(RefreshOppsPanel());
                break;
        }
    }



    private IEnumerator CheckLoggedEmployeeManagerOverviewCapability()
    {
        yield return new WaitWhile(() => (RepReader == null));

        IEnumerable<EMPLOYEE> reportingEmployees = RepReader.GetEmployeesReportingTo(NetMng.GetLoggedEmployee().id);

        if (reportingEmployees != null && reportingEmployees.Count() > 0)
        {
            managerOverviewButtonGo.SetActive(true);
            yield break;
        }

        managerOverviewButtonGo.SetActive(false);
        yield break;
    }

    private IEnumerator CheckLoggedEmployeeCreateOppCapability()
    {
        yield return new WaitWhile(() => (RepReader == null));

        if (RepReader.GetDepartment(NetMng.GetLoggedEmployee().department_id).department_name == "sales")
        {
            createOppButtonGo.SetActive(true);
            yield break;
        }

        createOppButtonGo.SetActive(false);
        yield break;
    }



    #region Opps Panel

    private IEnumerator RefreshOppsPanel()
    {
        msgTxt.text = "Refreshing Opportunity Panel";
        if (oppPanelTransform.childCount > 0)
        {
            ClearPanel(oppPanelTransform);
        }

        yield return new WaitWhile(() => (RepReader == null));
        yield return new WaitUntil(() => (RepositoryWriter.Fresh));

        IEnumerable<OPPORTUNITY> assignedOpps = RepReader.GetOppsAssignedTo(GameMng._employeeInFocus.id);

        if (assignedOpps == null || assignedOpps.Count() < 1)
        {
            msgTxt.text = "No tasks assigned to " + GameMng._employeeInFocus.employee_name + " were received";
            yield break;
        }
        
        foreach (OPPORTUNITY task in assignedOpps)
        {
            AddOppThumbnailToPanel(task);
            yield return new WaitForEndOfFrame();
        }

        if(msgTxt.text == "Refreshing Opp Panel")
        {
            msgTxt.text = "Opp Panel refreshed";
        }

        scheduledActions.RemoveAt(0);
        FetchNextAction();
        yield break;
    }

    #endregion



    #region Thumbnail

    private void AddOppThumbnailToPanel(OPPORTUNITY opp)
    {
        GameObject taskThumbnailGo = Instantiate(oppThumbnailPrefab).gameObject;
        taskThumbnailGo.name = taskThumbnailGo.name.Replace("(Clone)", opp.id.ToString());
        taskThumbnailGo.transform.SetParent(oppPanelTransform, false);
        taskThumbnailGo.GetComponent<OppThumbnailScript>().Setup(opp);
    }

    /// <summary>
    /// NEEDS TESTING
    /// </summary>
    /// <param name="task"></param>
    private void RemoveOppThumbnailFromPanel(OPPORTUNITY opp)
    {
        if (oppPanelTransform.childCount > 0)
        {
            foreach (Transform child in oppPanelTransform.GetComponentInChildren<Transform>())
            {
                if(child.name.EndsWith(opp.id.ToString()))
                {
                    Destroy(child.gameObject);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// NEEDS TESTING
    /// </summary>
    /// <param name="task"></param>
    private void UpdateOppThumbnail(OPPORTUNITY opp)
    {
        if (oppPanelTransform.childCount > 0)
        {
            foreach (Transform child in oppPanelTransform.GetComponentInChildren<Transform>())
            {
                if (child.name.EndsWith(opp.id.ToString()))
                {
                    child.GetComponent<OppThumbnailScript>().Setup(opp);
                }
            }
        }
    }

    #endregion
        
    public override void InformUser(string info)
    {
        msgTxt.text = info;
    }

}
