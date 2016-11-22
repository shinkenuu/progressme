using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

[RequireComponent(typeof(AnimationScript))]
public class PanelGUIScript : GenericGUI {



    #region References

    [SerializeField]
    private Image employeeInFocusImage;
    [SerializeField]
    private Transform panelTransform;

    [SerializeField]
    private Transform panelOppThumbnailPrefab;

    #endregion

    private PanelCmnScript _panelCmn;
    private AnimationScript _animation;

    private EMPLOYEE[] _employeesArray;
    private byte _employeesArrayIdx;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Setup());

        _animation = gameObject.GetComponent<AnimationScript>();
    }






    #region Panel Manipulation
    
    private IEnumerator RefreshPanel()
    {
        while (true)
        {
            if (panelTransform.childCount > 0)
            {
                ClearPanel();
            }

            GameMng._employeeInFocus = FetchEmployee();

            RequestScript assignedOpportunitiesRequest = RequestScript.CreateInstance<RequestScript>();
            assignedOpportunitiesRequest.SetupRequest(RequestScript.RequestType.CONSULT_ASSIGNED_OPPORTUNITIES, GameMng._employeeInFocus.ToJson().Print());
            _panelCmn.PerformRequest(ref assignedOpportunitiesRequest);

            yield return new WaitUntil(assignedOpportunitiesRequest.IsClosed);

            IEnumerable<OPPORTUNITY> assignedOpportunities = UnpackageOpps(assignedOpportunitiesRequest.GetJsonArray());

            if (assignedOpportunities == null)
            {
                StartCoroutine(RefreshPanel());
                yield break;
            }

            RefreshAvatar();
            byte taskIdx = 0;

            foreach (OPPORTUNITY task in assignedOpportunities)
            {
                AddPanelOppThumbnailToPanel(task, (40 / assignedOpportunities.Count()) * taskIdx++, 40 / assignedOpportunities.Count());
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForSeconds(40f);
        }
    }



    private void RefreshAvatar()
    {
        _animation.SetupAnimation(employeeInFocusImage, GameMng._employeeInFocus);
    }

    public void RefreshAvatarStatus(TimeSpan timeSpan)
    {
        if(timeSpan.Days < -2)
        {
            _animation.SetAction(AnimationScript.Action.prone);
            _animation.SetExpression(AnimationScript.Expression.despair);
        }

        else if(timeSpan.Days < -1)
        {
            _animation.SetAction(AnimationScript.Action.alert);
            _animation.SetExpression(AnimationScript.Expression.troubled);
        }

        else if (timeSpan.Days < 1)
        {
            _animation.SetAction(AnimationScript.Action.stand1);
            _animation.SetExpression(AnimationScript.Expression.blink);
        }

        else if (timeSpan.TotalDays < 2)
        {
            _animation.SetAction(AnimationScript.Action.alert);
            _animation.SetExpression(AnimationScript.Expression.glitter);
        }

        else
        {
            _animation.SetAction(AnimationScript.Action.shootF);
            _animation.SetExpression(AnimationScript.Expression.dam);
        }
    }
    
    private void AddPanelOppThumbnailToPanel(OPPORTUNITY opp, float highlighDelaySeconds, float highlightDurationSeconds)
    {
        GameObject panelOppThumbnailGo = Instantiate(panelOppThumbnailPrefab).gameObject;
        panelOppThumbnailGo.name = panelOppThumbnailGo.name.Replace("(Clone)", "") + opp.id.ToString();
        panelOppThumbnailGo.transform.SetParent(panelTransform, false);
        panelOppThumbnailGo.GetComponent<PanelOppThumbnailScript>().Setup(opp, highlighDelaySeconds, highlightDurationSeconds, this);
    }
    
    private void ClearPanel()
    {
        List<Transform> childrenTransform = new List<Transform>();
        foreach (Transform child in panelTransform.GetComponentInChildren<Transform>())
        {
            childrenTransform.Add(child);
        }

        foreach (Transform child in childrenTransform)
        {
            Destroy(child.gameObject);
        }
    }

    private EMPLOYEE FetchEmployee()
    {
        if (_employeesArrayIdx == _employeesArray.Count())
        {
            _employeesArrayIdx = 0;
        }

        return _employeesArray[_employeesArrayIdx++];
    }

    private IEnumerable<OPPORTUNITY> UnpackageOpps(string[] tasksJson)
    {
        if (tasksJson == null || string.IsNullOrEmpty(tasksJson[0]))
        {
            return null;
        }

        List<OPPORTUNITY> opps = new List<OPPORTUNITY>();

        foreach (string json in tasksJson)
        {
            OPPORTUNITY opportunity = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            opportunity.FromJson(json);
            opps.Add(opportunity);
        }

        return opps as IEnumerable<OPPORTUNITY>;
    }

    #endregion








    #region Setup
    
    private IEnumerator Setup()
    {
        _employeesArrayIdx = 0;

        StartCoroutine(LookForPlayerGo());

        yield return new WaitWhile(() => (GameMng == null));
        yield return new WaitWhile(() => (_panelCmn == null));
        yield return new WaitWhile(() => (RepReader == null));
        yield return new WaitUntil(() => (RepositoryWriter.Fresh));
        
        _employeesArray = RepReader.GetAllEmployees().ToArray();
        StartCoroutine(RefreshPanel());

        yield break;
    }
    
    #endregion



    #region LookFor

    private IEnumerator LookForPlayerGo()
    {
        GameObject[] playerGOs;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            playerGOs = GameObject.FindGameObjectsWithTag("Player");

            //If found any playerGO
            if (playerGOs != null && playerGOs.Count() > 0)
            {
                foreach (GameObject playerGo in playerGOs)
                {
                    try
                    {
                        //Look for the playerGo of this connection
                        if (playerGo.GetComponent<NetworkIdentity>().isLocalPlayer)
                        {
                            //And set the references
                            _panelCmn = playerGo.GetComponent<PanelCmnScript>();
                            yield break;
                        }
                    }

                    catch (UnityException)
                    {
                        continue;
                    }

                    yield return new WaitForEndOfFrame();
                }

                playerGOs = null;
            }

            yield return new WaitForEndOfFrame();
        }

    }
    
    #endregion


}
