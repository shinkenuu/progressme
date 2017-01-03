using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts.Biz.Procedure.User.Opportunity
{
    public class OppLooseProcedureScript : UserProcedure<OPPORTUNITY>, IProcedure
    {
        private OPPORTUNITY UserSelectedOpp;


        protected override IEnumerator UserProceed()
        {
            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(SelectionGui.Opp);
            CurrentSelectionGui.SetSelectables(RepReader.GetOppsAssignedTo(Watcher.EmployeeInFocus.id));
            CurrentSelectionGui.RefreshPanel();
            yield return null;

            if(!ValidateSelectedEntities())
            {
                throw new InvalidOperationException("Cannot proceed with invalid entities");
            }

            Command(UserSelectedOpp);
            yield break;
        }

        


        
        protected override void Command(OPPORTUNITY opp)
        {
            UserCmn.LoseOpp(opp, ReceiveCommandBack);
        }



        #region Validation

        protected override bool ValidateSelectedEntities()
        {
            return ValidateUserSelectedOpp();
        }


        private bool ValidateUserSelectedOpp()
        {
            if (UserSelectedOpp == null)
            {
                Debug.LogError("Invalid opp");
                return false;
            }

            return true;
        }

        #endregion


        #region Setters

        /// <summary>
        /// To be used by the GUIs selected button
        /// </summary>
        /// <param name="opp"></param>
        public void SetUserSelectedOpp(OPPORTUNITY opp)
        {
            UserSelectedOpp = opp;
        }

        #endregion

    }
}
