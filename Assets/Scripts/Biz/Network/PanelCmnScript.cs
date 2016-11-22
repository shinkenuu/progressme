using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;


public class PanelCmnScript : NetworkBehaviour {

    #region References

    private ServerManagerScript _srvMng;
    private RepositoryWriter _repWriter;

    #endregion

    private List<RequestScript> _requestsList;
    private int _nextRequestId;

    

    private void OnEnable()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Server":
                StartCoroutine(LookForServerManagerGo());
                StartCoroutine(LookForDataGo());
                break;
            case "Panel":
                StartCoroutine(LookForDataGo());
                break;
        }

        _requestsList = new List<RequestScript>();
        _nextRequestId = 0;
    }





    #region Commands


    [Command]
    public void CmdRefresh(int requestTypeId, int requestId)
    {
        List<string> jsonList = new List<string>();

        switch (requestTypeId)
        {
            case (int)RequestScript.RequestType.REFRESH_DEPARTMENTS:
                foreach (JSONable jsonable in _repWriter.GetAllDepartments())
                {
                    jsonList.Add(jsonable.ToJson().Print());
                }
                break;
            case (int)RequestScript.RequestType.REFRESH_CUSTOMER:
                foreach (JSONable jsonable in _repWriter.GetAllAccounts())
                {
                    jsonList.Add(jsonable.ToJson().Print());
                }
                break;
            case (int)RequestScript.RequestType.REFRESH_PROCESS:
                foreach (JSONable jsonable in _repWriter.GetAllProcesses())
                {
                    jsonList.Add(jsonable.ToJson().Print());
                }
                break;
            case (int)RequestScript.RequestType.REFRESH_PRODUCT:
                foreach (JSONable jsonable in _repWriter.GetAllProducts())
                {
                    jsonList.Add(jsonable.ToJson().Print());
                }
                break;
            case (int)RequestScript.RequestType.REFRESH_WORKER_PROFILE:
                foreach (JSONable jsonable in _repWriter.GetAllEmployees())
                {
                    jsonList.Add(jsonable.ToJson().Print());
                }
                break;
        }

        SendByPieces(jsonList, requestId);
    }
    
    [Command]
    public void CmdConsult(int requestTypeId, int requestId, string auxJson)
    {
        List<string> jsonList = new List<string>();

        OPPORTUNITY auxOpp = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
        EMPLOYEE auxEmployee = EMPLOYEE.CreateInstance<EMPLOYEE>();

        switch (requestTypeId)
        {
            case (int)RequestScript.RequestType.CONSULT_FIRST_LAST_OPPORTUNITY_HISTORY:
                auxOpp.FromJson(auxJson);
                foreach (OPPORTUNITY_HISTORY tHistory in _repWriter.GetFirstAndLastOppHistoryOf(auxOpp.id))
                {
                    jsonList.Add(tHistory.ToJson().Print());
                }
                break;
            case (int)RequestScript.RequestType.CONSULT_FIRST_PENULTIMATE_LAST_OPPORTUNITY_HISTORY:
                auxOpp.FromJson(auxJson);
                foreach (OPPORTUNITY_HISTORY tHistory in _repWriter.GetFirstAndPenultimateAndLastOppHistoryOf(auxOpp.id))
                {
                    jsonList.Add(tHistory.ToJson().Print());
                }
                break;
            case (int)RequestScript.RequestType.CONSULT_ASSIGNED_OPPORTUNITIES:
                auxEmployee.FromJson(auxJson);
                foreach (OPPORTUNITY assignedOpp in _repWriter.GetOppsAssignedTo(auxEmployee.id))
                {
                    jsonList.Add(assignedOpp.ToJson().Print());
                }
                SendByPieces(jsonList, requestId);
                return;
            default:
                Debug.Log("RequestType not implemented for Panel: " + requestTypeId.ToString());
                break;
        }

        RpcPopJsonArray(jsonList.ToArray(), requestId, true);
    }



    private void SendByPieces(List<string> jsonList, int requestId)
    {
        string[] jsonArray = jsonList.ToArray();

        StringBuilder strBldr = new StringBuilder();

        foreach (string json in jsonArray)
        {
            if (ASCIIEncoding.Unicode.GetByteCount(strBldr.ToString() + json) > 1024)
            {
                jsonList = strBldr.ToString().Split(new char[] { '|' }).ToList();
                jsonList.RemoveAt(jsonList.Count() - 1);

                RpcPopJsonArray(jsonList.ToArray(), requestId, false);

                strBldr.Remove(0, strBldr.Length);
            }

            strBldr.Append(json + '|');
        }

        jsonList = strBldr.ToString().Split(new char[] { '|' }).ToList();
        jsonList.RemoveAt(jsonList.Count() - 1);

        RpcPopJsonArray(jsonList.ToArray(), requestId, true);
    }

    #endregion


    #region RPC


    [ClientRpc]
    public void RpcPopJsonArray(string[] jsonArray, int requestId, bool closeRequest)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (jsonArray == null || jsonArray.Count() == 0 || string.IsNullOrEmpty(jsonArray[0]))
        {
            Debug.Log("::CRITICAL:: jsonArray provided by the server came null!");
            ReplyRequest(null, requestId, true);
            return;
        }

        ReplyRequest(jsonArray, requestId, closeRequest);
    }



    #endregion





    #region Request

    public void PerformRequest(ref RequestScript newRequest)
    {
        if (!newRequest.IsReady())
        {
            Debug.LogError("Request denied by PanelCmn for not beeing READY");
            newRequest = null;
            return;
        }

        newRequest.Id = FetchRequestId();
        _requestsList.Add(newRequest);

        //REFRESH
        if ((int)newRequest.GetRequestType() < 20)
        {
            newRequest.Open();
            CmdRefresh((int)newRequest.GetRequestType(), newRequest.Id);
        }
        else if ((int)newRequest.GetRequestType() > 79 && (int)newRequest.GetRequestType() < 100)
        {
            CmdConsult((int)newRequest.GetRequestType(), newRequest.Id, newRequest.Open());
        }

        return;
    }

    private void ReplyRequest(string[] jsonArray, int requestId, bool closeRequest)
    {
        RequestScript requestScript = _requestsList.FirstOrDefault(r => r.Id == requestId);

        if (requestScript == null)
        {
            Debug.Log("Attempt to reply null request with id: " + requestId.ToString());
            return;
        }

        if (jsonArray != null)
        {
            requestScript.AddToJsonList(jsonArray);
        }

        if (closeRequest)
        {
            requestScript.Close();
            _requestsList.Remove(requestScript);
        }
    }


    private int FetchRequestId()
    {
        _nextRequestId = (_nextRequestId + 1) % int.MaxValue;
        return _nextRequestId;
    }

    #endregion








    #region LookFor

    private IEnumerator LookForDataGo()
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

    private IEnumerator LookForServerManagerGo()
    {
        GameObject srvMngGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            srvMngGo = GameObject.Find("SrvMngGO");

            if (srvMngGo != null)
            {
                _srvMng = srvMngGo.GetComponent<ServerManagerScript>();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    #endregion







}
