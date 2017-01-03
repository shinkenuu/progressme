using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

namespace Assets.Scripts.Biz.Procedure.User.Opportunity
{
    public class OppSelectionProcedureScript : UserProcedure<OPPORTUNITY>
    {
        public  OPPORTUNITY UserSelectedOpp
        {
            private set;
            get;
        }
        
        protected override void Command(OPPORTUNITY result)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerator UserProceed()
        {
            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(GUI.User.Selection.SelectionGui.Opp);
            CurrentSelectionGui.SetSelectables<OPPORTUNITY>(
                RepReader.GetOppsAssignedTo(Watcher.EmployeeInFocus.id));
            CurrentSelectionGui.RefreshPanel();
            yield return null;

            if(ValidateSelectedEntities())
            {
                ProcedureResult = UserSelectedOpp;
                OnUserProcedureComplete();
            }

            yield break;
        }


        #region Validation

        protected override bool ValidateSelectedEntities()
        {
            return ValidateUserSelectedOpp();
        }

        private bool ValidateUserSelectedOpp()
        {
            if (UserSelectedOpp == null || UserSelectedOpp.id < 1)
            {
                Debug.Log("::CRITICAL:: Attempt to select a null opportunity");
                return false;
            }

            return true;
        }

        #endregion



        #region Setters

        public void SetUserSelectedOpp(OPPORTUNITY opp)
        {
            UserSelectedOpp = opp;
        }

        #endregion




    }
}
