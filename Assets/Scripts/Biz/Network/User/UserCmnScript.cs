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

namespace Assets.Scripts.Biz.Network.User
{
    public class UserCmnScript : Communication
    {        
        protected OpportunityManagerScript OppMng;
        protected AccountManagerScript AccMng;



        protected override void OnEnable()
        {
            switch(SceneManager.GetActiveScene().name)
            {
                case "User":
                case "Server":
                    base.OnEnable();
                    return;
            }

            //this.enabled = false;
        }





        #region Opp

        public void InsertOpportunity(OPPORTUNITY insertingOpportunity, Command.CmdClosed onClosedCall)
        {
            if(insertingOpportunity == null)
            {
                throw new ArgumentException("Cannot insert null opp");
            }

            Command.Command cmd = Command.Command.CreateInstance<Command.Command>();
            cmd.SubscribeForClosed(onClosedCall);
            cmd.SetupCmd(CmdIndexer, insertingOpportunity.ToJson().Print());
            Commands.Add(cmd);

            CmdCreateOpp(cmd.Id, cmd.Open());            
        }
        

        public void AdvanceOpp(OPPORTUNITY advancingOpp, Command.CmdClosed onClosedCall)
        {
            Command.Command cmd = Command.Command.CreateInstance<Command.Command>();
            cmd.SubscribeForClosed(onClosedCall);
            cmd.SetupCmd(CmdIndexer, advancingOpp.ToJson().Print());
            Commands.Add(cmd);

            CmdAdvanceOpp(cmd.Id, cmd.Open());
        }


        public void TransferOpp(OPPORTUNITY transferingOpp, Command.CmdClosed onClosedCall)
        {
            Command.Command cmd = Command.Command.CreateInstance<Command.Command>();
            cmd.SubscribeForClosed(onClosedCall);
            cmd.SetupCmd(CmdIndexer, transferingOpp.ToJson().Print());
            Commands.Add(cmd);

            CmdTransferOpp(cmd.Id, cmd.Open());
        }


        public void LoseOpp(OPPORTUNITY lostOpp, Command.CmdClosed onClosedCall)
        {
            Command.Command cmd = Command.Command.CreateInstance<Command.Command>();
            cmd.SubscribeForClosed(onClosedCall);
            cmd.SetupCmd(CmdIndexer, lostOpp.ToJson().Print());
            Commands.Add(cmd);

            CmdLoseOpp(cmd.Id, cmd.Open());
        }

        #endregion



        #region Acc

        public void InsertAccount(ACCOUNT insertingAcc, Command.CmdClosed onClosedCall)
        {
            if (insertingAcc == null)
            {
                throw new ArgumentException("Cannot insert null account");
            }            

            Command.Command cmd = Command.Command.CreateInstance<Command.Command>();
            cmd.SubscribeForClosed(onClosedCall);
            cmd.SetupCmd(CmdIndexer, insertingAcc.ToJson().Print());
            Commands.Add(cmd);

            CmdCreateAccount(cmd.Id, cmd.Open());
        }
        

        #endregion






        #region Commands


        #region Opp

        [Command]
        protected void CmdCreateOpp(int cmdId, string auxJson)
        {
            if (string.IsNullOrEmpty(auxJson))
            {
                RpcPopJsonArray(null, cmdId, true);
            }

            OPPORTUNITY opp = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            opp.FromJson(auxJson);

            OppMng.CreateOpportunity(ref opp);

            if(opp == null)
            {
                RpcPopJsonArray(null, cmdId, true);
                return;
            }

            RpcPopJsonArray(new string[] { opp.ToJson().Print() }, cmdId, true);        
        }


        [Command]
        protected void CmdAdvanceOpp(int cmdId, string auxJson)
        {
            if (string.IsNullOrEmpty(auxJson))
            {
                RpcPopJsonArray(null, cmdId, true);
            }

            OPPORTUNITY opp = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            opp.FromJson(auxJson);

            OppMng.AdvanceToNextProcess(ref opp);

            if (opp == null)
            {
                RpcPopJsonArray(null, cmdId, true);
                return;
            }

            RpcPopJsonArray(new string[] { opp.ToJson().Print() }, cmdId, true);
        }

        [Command]
        protected void CmdTransferOpp(int cmdId, string auxJson)
        {
            if (string.IsNullOrEmpty(auxJson))
            {
                RpcPopJsonArray(null, cmdId, true);
            }

            OPPORTUNITY opp = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            opp.FromJson(auxJson);

            OppMng.TransferOpportunity(ref opp);

            if (opp == null)
            {
                RpcPopJsonArray(null, cmdId, true);
                return;
            }

            RpcPopJsonArray(new string[] { opp.ToJson().Print() }, cmdId, true);
        }

        [Command]
        protected void CmdLoseOpp(int cmdId, string auxJson)
        {
            if (string.IsNullOrEmpty(auxJson))
            {
                RpcPopJsonArray(null, cmdId, true);
            }

            OPPORTUNITY opp = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            opp.FromJson(auxJson);

            OppMng.EndOpportunity(ref opp);

            if (opp == null)
            {
                RpcPopJsonArray(null, cmdId, true);
                return;
            }

            RpcPopJsonArray(new string[] { opp.ToJson().Print() }, cmdId, true);
        }


        #endregion





        #region Account

        [Command]
        protected void CmdCreateAccount(int cmdId, string auxJson)
        {
            if (string.IsNullOrEmpty(auxJson))
            {
                RpcPopJsonArray(null, cmdId, true);
            }

            ACCOUNT acc = ACCOUNT.CreateInstance<ACCOUNT>();
            acc.FromJson(auxJson);

            AccMng.CreateAccount(ref acc);

            if (acc == null)
            {
                RpcPopJsonArray(null, cmdId, true);
                return;
            }

            RpcPopJsonArray(new string[] { acc.ToJson().Print() }, cmdId, true);
        }

        #endregion


        #endregion



    }
}
