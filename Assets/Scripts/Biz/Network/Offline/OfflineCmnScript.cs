using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;
using Assets.Scripts.Biz.Manager;
using Assets.Scripts.Model;

namespace Assets.Scripts.Biz.Network.Offline
{

    [RequireComponent(typeof(Communication))]
    public class OfflineCmnScript : NetworkBehaviour
    {
        protected Communication Cmn;

        protected void OnEnable()
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "Offline":
                case "Server":
                    //base.OnEnable();
                    Cmn = gameObject.GetComponent<Communication>();
                    return;
                default:
                    Cmn.Commands.Clear();
                    Cmn.Commands = null;
                    break;
            }

            //this.enabled = false;
        }

        

        public void Login(string username, string password, Command.CmdClosed onClosedCall)
        {
            EMPLOYEE emp = EMPLOYEE.CreateInstance<EMPLOYEE>();
            emp.employee_name = username;
            emp.password = password;

            Command.Command cmd = Command.Command.CreateInstance<Command.Command>();
            cmd.SubscribeForClosed(onClosedCall);
            cmd.SetupCmd(Cmn.CmdIndexer, emp.ToJson().Print());

            if(cmd.Status != Command.Status.Ready)
            {
                throw new InvalidOperationException("Command status should be READY. Actual status:" + cmd.Status.ToString());
            }

            Cmn.Commands.Add(cmd);
            CmdLogin(cmd.Id, cmd.Open());
        }


        [Command]
        protected void CmdLogin(int cmdId, string auxJson)
        {
            if(string.IsNullOrEmpty(auxJson))
            {
                Cmn.RpcPopJsonArray(null, cmdId, true);
            }

            EMPLOYEE emp = EMPLOYEE.CreateInstance<EMPLOYEE>();
            emp.FromJson(auxJson);

            emp = Cmn.Watcher.EmpMng.ValidateLogIn(emp.employee_name, emp.password);

            if(emp == null)
            {
                Cmn.RpcPopJsonArray(null, cmdId, true);
            }

            Cmn.RpcPopJsonArray(new string[] { emp.ToJson().Print() }, cmdId, true);
        }



        //[ClientRpc]
        //protected override void RpcPopJsonArray(string[] jsonArray, int cmdId, bool closeRequest)
        //{
        //    if (!isLocalPlayer)
        //    {
        //        return;
        //    }

        //    if (jsonArray == null || jsonArray.Count() == 0 || string.IsNullOrEmpty(jsonArray[0]))
        //    {
        //        Debug.Log("jsonArray provided by the server came null!");
        //        ReplyCmn(null, cmdId, true);
        //        return;
        //    }

        //    ReplyCmn(jsonArray, cmdId, closeRequest);
        //}




        //protected override void ReplyCmn(string[] jsonArray, int requestId, bool closeRequest)
        //{
        //    Command.Command cmd = Commands.FirstOrDefault(r => r.Id == requestId);

        //    if (cmd == null)
        //    {
        //        Debug.Log("Attempt to reply null request with id: " + requestId.ToString());
        //        return;
        //    }

        //    if (jsonArray != null)
        //    {
        //        cmd.JsonArray = jsonArray;
        //    }

        //    if (closeRequest)
        //    {
        //        cmd.Close();
        //        Commands.Remove(cmd);
        //    }
        //}

    }
}
