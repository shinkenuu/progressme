using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.Biz.Network;
using Assets.Scripts.GUI.User;

namespace Assets.Scripts.Biz.Procedure.User
{
    public delegate void UserProcedureComplete();

    public abstract class UserProcedure<TResult> : Procedure, IProcedure where TResult : JSONable
    {
        protected RepositoryReader RepReader;
        protected Network.User.UserCmnScript UserCmn;
        protected UserGuiFactory UserGuiFactory;

        protected GUI.User.Selection.ISelectionGui CurrentSelectionGui;

        protected event UserProcedureComplete _userProcedureComplete;

        protected override void OnEnable()
        {
            base.OnEnable();
            RepReader = Watcher.RepReader;
        }

        public TResult ProcedureResult
        {
            protected set;
            get;
        }

        protected abstract IEnumerator UserProceed();

        public override IEnumerator Proceed()
        {
            yield return UserProceed();
        }
        
        
        /// <summary>
        /// Validate all the selected enities before making the command
        /// </summary>
        /// <returns>True if OK, False otherwise</returns>
        protected abstract bool ValidateSelectedEntities();
    
        /// <summary>
        /// Calls the proper UserCmn's method as per the final purpose of the User Procedure
        /// </summary>
        /// <param name="result"></param>
        protected abstract void Command(TResult result);

        protected void ReceiveCommandBack(Network.Command.Command cmd)
        {
            if(cmd.JsonArray == null)
            {
                return;
            }
            
            ProcedureResult.FromJson(cmd.JsonArray[0]);
        }


        public void Subscribe(UserProcedureComplete call)
        {
            _userProcedureComplete += call;
        }

        public void Unsubscribe(UserProcedureComplete call)
        {
            _userProcedureComplete -= call;
        }

        protected void OnUserProcedureComplete()
        {
            if(_userProcedureComplete != null)
            {
                _userProcedureComplete.Invoke();
            }
        }

    }
}
