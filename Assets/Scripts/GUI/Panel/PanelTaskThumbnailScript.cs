using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;


public class PanelOppThumbnailScript : ThumbnailScript
{
        
    #region References
        
    private RepositoryReader RepReader;

    private RequestScript _requestHistory;
    private UserCmnScript _panelCmn;

    //------------------------------------------
    
    [SerializeField]
    private Text taskDetailsText;

    [SerializeField]
    private GameObject poitingArrowGo;

    [SerializeField]
    private GameObject ghostGo;
    [SerializeField]
    private RectTransform progressBar;
    [SerializeField]
    private RectTransform fullProgressBar;
    [SerializeField]
    private RectTransform emptyProgressBar;
    

    [SerializeField]
    private Image accountImage;

    #endregion

    private PanelGUIScript _panelGui;

    private OPPORTUNITY _opp;
    private List<OPPORTUNITY_HISTORY> _oppHistories;
    private bool _refreshed;

    private DateTime _idealEndDate;
    
    private readonly TimeSpan oneMin = new TimeSpan(0, 1, 0);



    private void OnEnable()
    {
        StartCoroutine(LookForDataGo());
        StartCoroutine(LookForPlayerGo());
        
        _oppHistories = new List<OPPORTUNITY_HISTORY>();
    }





    #region Setup & Refresh

    /// <summary>
    /// Sets up the task info into the thumbanail. Schedules when this thumbnail will be highlighted, and for how long.
    /// </summary>
    /// <param name="task">The task this thumbnail will present</param>
    /// <param name="secondsDelay">The seconds to take BEFORE highligting this thumbnail</param>
    /// <param name="secondsLit">The seconds to be kept highlighted</param>
    public void Setup(OPPORTUNITY opportunity, float secondsDelay, float secondsLit,  PanelGUIScript panelGui)
    {
        if (opportunity == null)
        {
            Debug.Log("::CRITICAL:: task sent to OppThumbnail.Setup() came null");
            Destroy(gameObject);
            return;
        }

        _panelGui = panelGui;
        _opp = opportunity;

        #region Opp History

        _requestHistory = null;
        _requestHistory = RequestScript.CreateInstance<RequestScript>();
        _requestHistory.SetupRequest(RequestScript.RequestType.CONSULT_FIRST_PENULTIMATE_LAST_OPPORTUNITY_HISTORY, _opp.ToJson().Print());

        _refreshed = false;
        StartCoroutine(RefreshThumbnail());

        #endregion

        StartCoroutine(Highlight(secondsDelay, secondsLit));

    }

    private IEnumerator RefreshThumbnail()
    {
        yield return new WaitWhile(() => (_panelCmn == null));
        _panelCmn.PerformRequest(ref _requestHistory);

        yield return new WaitUntil(_requestHistory.IsClosed);

        _oppHistories.Clear();
        foreach (string json in _requestHistory.GetJsonArray())
        {
            OPPORTUNITY_HISTORY h = OPPORTUNITY_HISTORY.CreateInstance<OPPORTUNITY_HISTORY>();
            h.FromJson(json);
            _oppHistories.Add(h);
        }

        _oppHistories = _oppHistories.OrderBy(th => th.history_date).ToList();
        _requestHistory = null;

        yield return new WaitWhile(() => (RepReader == null));
        
        //account
        ACCOUNT account = RepReader.GetAccount(_opp.account_id);

        SetImageComponent("file:///" + AccountManagerScript._accountFolder.Replace('\\', '/') + account.account_name + "/logo.png");
        
        int minimalIdealHours = RepReader.SumOfMinimalIdealHours();

        //Opp Details
        _idealEndDate = _opp.start_date.AddHours(minimalIdealHours);
        taskDetailsText.text = _opp.opportunity_name + " -> " + RepReader.GetProcess(_opp.current_process_id ?? 0).description + " <- " + _idealEndDate.ToString("MMM/yy");

        float progressPercent = (float)(DateTime.Now - _oppHistories.FirstOrDefault().history_date).TotalHours * 100
                                / RepReader.SumOfMinimalIdealHours();

        if (progressPercent > 100)
        {
            progressPercent = 100;
        }
        
        #region Ghost
        /*
        Texture2D ghostTexture2d = (Texture2D)Resources.Load("Sprites/Employees/" + RepReader.GetEmployee(_opp.responsable_employee_id ?? 0).employee_name + "/ghost");
        ghostGo.GetComponent<Image>().sprite = Sprite.Create(ghostTexture2d, new Rect(0, 0, ghostTexture2d.width, ghostTexture2d.height), new Vector2(0.5f, 0.5f));

        PROCESS ghostProcess = RepReader.GetIdealProcessAfterPastHours((int)(DateTime.Now - _oppHistories.FirstOrDefault().history_date).TotalHours, RepReader.GetProduct(_opp.product_id));
        
        //Ghost Percent
        float ghostPercent = RepReader.CalculateHoursBetween( RepReader.GetProcess(_oppHistories.FirstOrDefault().process_id), ghostProcess, RepReader.GetProduct(_opp.product_id)) * 100
                                / minimalIdealHours;

        if (ghostPercent > 100)
        {
            ghostPercent = 100;
        }
        */
        #endregion

        //progressBar
        progressBar.offsetMax = new Vector2(emptyProgressBar.offsetMax.x - (progressPercent * (emptyProgressBar.offsetMax.x - fullProgressBar.offsetMax.x) / 100), progressBar.offsetMax.y); // Left
        //ghost
        //ghostGo.GetComponent<RectTransform>().offsetMax = new Vector2(emptyProgressBar.offsetMax.x - (ghostPercent * (emptyProgressBar.offsetMax.x - fullProgressBar.offsetMax.x) / 100), progressBar.offsetMax.y); // Left

        _refreshed = true;
        yield break;
    }
    

    #endregion



    #region Highlight
    
    private IEnumerator Highlight(float secondsDelay, float secondsLit)
    {
        yield return new WaitForSeconds(secondsDelay);
        //Wait until request for task history has been completed
        yield return new WaitUntil(() => _refreshed == true);
        _panelGui.RefreshAvatarStatus((_oppHistories.LastOrDefault().history_date.AddHours(RepReader.GetProcess(_oppHistories.LastOrDefault().process_id).ideal_total_hours))
            - DateTime.Now);
        poitingArrowGo.SetActive(true);
        yield return new WaitForSeconds(secondsLit);
        poitingArrowGo.SetActive(false);
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
                            _panelCmn = playerGo.GetComponent<UserCmnScript>();
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
    
    private IEnumerator LookForDataGo()
    {
        GameObject dataGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            dataGo = GameObject.Find("DataGO");

            if (dataGo != null)
            {
                RepReader = dataGo.GetComponent<RepositoryReader>();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    #endregion

}
