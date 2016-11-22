using UnityEngine;
using System.Collections.Generic;

public class RequestScript : ScriptableObject {


    private List<string> JsonList;
    private string AuxJson;
    public int Id;

    public enum RequestStatus
    {
        Setup   = 0,
        Ready   = 1,
        Open    = 2,
        Closed  = 3
    }
    private RequestStatus Status;
    
    public enum RequestType
    {
        REFRESH_DEPARTMENTS                                 = 1,
        REFRESH_CUSTOMER                                    = 2,
        REFRESH_PROCESS                                     = 3,
        REFRESH_PRODUCT                                     = 5,
        REFRESH_WORKER_PROFILE                              = 7,
        // ---------------------------------                -----
        ADD_OPPORTUNITY                                     = 20,
        ADD_OPPORTUNITY_HISTORY                             = 21,
        ADD_CUSTOMER                                        = 22,
        // ---------------------------------                -----
        UPDATE_ADVANCING_OPPORTUNITY                        = 40,
        UPDATE_RETREATING_OPPORTUNITY                       = 41,
        UPDATE_TRANSFERING_OPPORTUNITY                      = 42,
        // ---------------------------------                -----
        REMOVE_OPPORTUNITY                                  = 60,
        // ---------------------------------                -----
        CONSULT_FIRST_OPPORTUNITY_HISTORY                   = 80,
        CONSULT_PENULTIMATE_OPPORTUNITY_HISTORY             = 81,
        CONSULT_LAST_OPPORTUNITY_HISTORY                    = 82,
        CONSULT_FIRST_LAST_OPPORTUNITY_HISTORY              = 83,
        CONSULT_FIRST_PENULTIMATE_LAST_OPPORTUNITY_HISTORY  = 84,
        CONSULT_ENTIRE_OPPORTUNITY_HISTORY                  = 85,
        CONSULT_ASSIGNED_OPPORTUNITIES                      = 90
    }
    private RequestType Type;


    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------
    //------------------------------------------------------------------------------------------------


    public void AddToJsonList(string[] jsonArray)
    {
        if(Status != RequestStatus.Open || jsonArray == null)
        {
            Debug.LogError("::CRITICAL:: Attempt to add null jsonArray");
            return;
        }

        if(JsonList == null)
        {
            JsonList = new List<string>();
        }

        JsonList.AddRange(jsonArray);
    }

    public void SetupRequest(RequestType type, string auxJson)
    {
        //If request demands a conditional json
        if((int)type > 19 && auxJson == null)
        {
            Status = RequestStatus.Setup;
            return;
        }

        Type = type;
        AuxJson = auxJson;

        Status = RequestStatus.Ready;
    }
    
    /// <summary>
    /// Opens the Request for acceptance of data
    /// </summary>
    /// <returns>The auxiliar json for any condition needed</returns>
    public string Open()
    {
        Status = RequestStatus.Open;
        return AuxJson;
    }

    public void Close()
    {
        Status = RequestStatus.Closed;
    }

    public string[] GetJsonArray()
    {
        if(JsonList == null)
        {
            return null;
        }

        return JsonList.ToArray();
    }
    
    public bool IsReady()
    {
        return Status == RequestStatus.Ready;
    }

    public bool IsClosed()
    {
        return Status == RequestStatus.Closed;
    }
    
    public RequestType GetRequestType()
    {
        return Type;
    }

}
