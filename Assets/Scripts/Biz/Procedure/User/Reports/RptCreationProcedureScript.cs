using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Biz.Procedure.User.Reports
{
    public class RptCreationProcedureScript : Procedure, IProcedure
    {
        public override IEnumerator Proceed()
        {
            throw new NotImplementedException();
        }

        protected IEnumerator x()
        {
            //if (_tempOpp == null)
            //{
            //    Debug.LogError("::CRITICAL:: Impossible to report a null task. Aborting");
            //    yield break;
            //}

            //RequestScript taskHistoryRequest = RequestScript.CreateInstance<RequestScript>();
            //taskHistoryRequest.SetupRequest(RequestScript.RequestType.CONSULT_ENTIRE_OPPORTUNITY_HISTORY, _tempOpp.ToJson().Print());

            //if (!taskHistoryRequest.IsReady())
            //{
            //    Debug.LogError("::CRITICAL:: Entire Opp History Request was setup but was not ready. Aborting");
            //    yield break;
            //}

            //_userCmn.PerformRequest(ref taskHistoryRequest);
            //yield return new WaitUntil(taskHistoryRequest.IsClosed);

            //string[] replyArray = taskHistoryRequest.GetJsonArray();

            //if (replyArray == null || string.IsNullOrEmpty(replyArray[0]))
            //{
            //    Debug.LogError("::CRITICAL:: Request reply came null or empty. Aborting");
            //    yield break;
            //}

            //Debug.Log("Opp History request replied correctly.");
            //StringBuilder strBuilder = new StringBuilder();

            //foreach (string reply in replyArray)
            //{
            //    strBuilder.AppendLine(reply);
            //}

            //_repWriter.SafeWriteToDisc(strBuilder.ToString(), RepositoryReader.FilesDirectory.Client, "Opp_Report_" + _tempOpp.id.ToString() + ".json");
            yield break;
        }

    }
}
