//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Networking;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using Assets.Scripts.Model;

//namespace Assets.Scripts.GUI.User.Selection.Opportunity
//{
//    public class OppThumbnailScript : ThumbnailScript
//    {

//        // RECODE CalculateHoursBetweenProcesses !!!!!!!!!!!!!!!!!!!!!!!!!!

//        #region References

//        private GameManagerScript GameMng;
//        private RepositoryReader RepReader;

//        private RequestScript _requestHistory;
//        private UserCmnScript _userCmn;

//        //------------------------------------------

//        [SerializeField]
//        private Image accountImage;
//        [SerializeField]
//        private RectTransform emptyProgressBar;
//        [SerializeField]
//        private RectTransform progressBar;
//        [SerializeField]
//        private RectTransform fullProgressBar;

//        [SerializeField]
//        private Text startDateTxt;
//        [SerializeField]
//        private Text idealEndDateTxt;

//        [SerializeField]
//        private Text timeLeftTxt;

//        #endregion

//        private OPPORTUNITY _opp;
//        private List<OPPORTUNITY_HISTORY> _oppHistories;

//        private DateTime _idealEndDate;

//        private TimeSpan _curTimeDiferenceToIdealTime;
//        private readonly TimeSpan oneMin = new TimeSpan(0, 1, 0);

//        private void OnEnable()
//        {
//            StartCoroutine(LookForManagersGo());
//            StartCoroutine(LookForDataGo());
//            StartCoroutine(LookForPlayerGo());

//            _curTimeDiferenceToIdealTime = TimeSpan.Zero;
//            _oppHistories = new List<OPPORTUNITY_HISTORY>();
//        }



//        private IEnumerator TickClock()
//        {
//            while (true)
//            {
//                RefreshClock();
//                yield return new WaitForSeconds(60f);
//            }
//        }





//        #region Setup & Refresh



//        public void Setup(OPPORTUNITY opp)
//        {
//            if (opp == null)
//            {
//                Debug.Log("::CRITICAL:: task sent to OppThumbnail.Setup() came null");
//                Destroy(gameObject);
//                return;
//            }

//            _opp = opp;

//            #region Opp History

//            _requestHistory = null;
//            _requestHistory = RequestScript.CreateInstance<RequestScript>();
//            _requestHistory.SetupRequest(RequestScript.RequestType.CONSULT_FIRST_PENULTIMATE_LAST_OPPORTUNITY_HISTORY, _opp.ToJson().Print());

//            StartCoroutine(RefreshThumbnail());

//            #endregion

//            if (accountImage.sprite == null)
//            {

//                WWW www = new WWW("file:///" + AccountManagerScript.AccountsFolder.Replace('\\', '/') + RepReader.GetAccount(_opp.account_id).account_name + "/logo.png");

//                if (www.texture.height == 100 && www.texture.width == 100)
//                {
//                    accountImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.height, www.texture.width), new Vector2(0.5f, 0.5f));
//                }

//                else
//                {
//                    accountImage.sprite = null;
//                }
//            }

//            //start date
//            startDateTxt.text = _opp.start_date.ToString("dd/MM/yyyy");
//            //end date
//            idealEndDateTxt.text = "";

//            StartCoroutine(TickClock());
//        }




//        private IEnumerator RefreshThumbnail()
//        {
//            yield return new WaitWhile(() => (_userCmn == null));
//            _userCmn.PerformRequest(ref _requestHistory);

//            yield return new WaitUntil(_requestHistory.IsClosed);

//            _oppHistories.Clear();
//            foreach (string json in _requestHistory.GetJsonArray())
//            {
//                OPPORTUNITY_HISTORY h = OPPORTUNITY_HISTORY.CreateInstance<OPPORTUNITY_HISTORY>();
//                h.FromJson(json);
//                _oppHistories.Add(h);
//            }

//            _oppHistories = _oppHistories.OrderBy(th => th.history_date).ToList();
//            _requestHistory = null;

//            //idealEndDate
//            _idealEndDate = _opp.start_date.AddHours(RepReader.SumOfMinimalIdealHours());
//            idealEndDateTxt.text = _idealEndDate.ToString("dd/MM/yyyy");

//            _curTimeDiferenceToIdealTime = (_oppHistories.LastOrDefault().history_date.AddHours(RepReader.GetProcess(_opp.current_process_id ?? 0).ideal_total_hours) - DateTime.Now);

//            float progressPercent = (float)(DateTime.Now - _oppHistories.FirstOrDefault().history_date).TotalHours * 100
//                                    / RepReader.SumOfMinimalIdealHours();

//            if (progressPercent > 100)
//            {
//                progressPercent = 100;
//            }

//            //progressBar
//            progressBar.offsetMax = new Vector2(emptyProgressBar.offsetMax.x - (progressPercent * (emptyProgressBar.offsetMax.x - fullProgressBar.offsetMax.x) / 100), progressBar.offsetMax.y); // Left
//            RefreshClock();

//            yield break;
//        }

//        private void RefreshClock()
//        {
//            _curTimeDiferenceToIdealTime = _curTimeDiferenceToIdealTime.Subtract(oneMin);
//            timeLeftTxt.text = string.Format("{0} day(s) {1}h {2}m", _curTimeDiferenceToIdealTime.Days, _curTimeDiferenceToIdealTime.Hours, _curTimeDiferenceToIdealTime.Minutes);

//            //If there is time left, and not lacking
//            if (_curTimeDiferenceToIdealTime > TimeSpan.Zero)
//            {
//                timeLeftTxt.color = Color.green;
//            }

//            else
//            {
//                timeLeftTxt.color = Color.red;
//            }
//        }

//        #endregion



//        #region Buttons

//        public void BtnActionDetailOpp()
//        {
//            if (_oppHistories != null && _oppHistories.Count() > 0)
//            {
//                GameMng._tempOpp = _opp;
//                GameMng._tempOppHistories = _oppHistories;
//                GameMng.ScheduleProcedure(GameManagerScript.Procedures.OppDetail);
//                GameMng.LoadNextWindow();
//            }
//        }

//        #endregion


//        #region LookFor

//        private IEnumerator LookForPlayerGo()
//        {
//            GameObject[] playerGOs;

//            //While the GameObject has not been Instantiated yet by the Scene
//            while (true)
//            {
//                playerGOs = GameObject.FindGameObjectsWithTag("Player");

//                //If found any playerGO
//                if (playerGOs != null && playerGOs.Count() > 0)
//                {
//                    foreach (GameObject playerGo in playerGOs)
//                    {
//                        try
//                        {
//                            //Look for the playerGo of this connection
//                            if (playerGo.GetComponent<NetworkIdentity>().isLocalPlayer)
//                            {
//                                //And set the references
//                                _userCmn = playerGo.GetComponent<UserCmnScript>();
//                                yield break;
//                            }
//                        }

//                        catch (UnityException)
//                        {
//                            continue;
//                        }

//                        yield return new WaitForEndOfFrame();
//                    }

//                    playerGOs = null;
//                }

//                yield return new WaitForEndOfFrame();
//            }

//        }

//        private IEnumerator LookForManagersGo()
//        {
//            GameObject homeGuiGo;

//            //While the GameObject has not been Instantiated yet by the Scene
//            while (true)
//            {
//                homeGuiGo = GameObject.Find("MngGO");

//                if (homeGuiGo != null)
//                {
//                    GameMng = homeGuiGo.GetComponent<GameManagerScript>();
//                    yield break;
//                }

//                yield return new WaitForEndOfFrame();
//            }
//        }

//        private IEnumerator LookForDataGo()
//        {
//            GameObject dataGo;

//            //While the GameObject has not been Instantiated yet by the Scene
//            while (true)
//            {
//                dataGo = GameObject.Find("DataGO");

//                if (dataGo != null)
//                {
//                    RepReader = dataGo.GetComponent<RepositoryReader>();
//                    yield break;
//                }

//                yield return new WaitForEndOfFrame();
//            }
//        }

//        #endregion

//    }
//}
