using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;

namespace Assets.Scripts.GUI.User.Creation
{
    public class OppCreationProcedureScript : UserProcedure<OPPORTUNITY>
    {
        private EMPLOYEE UserSeletectedEmployee;
        private ACCOUNT UserSeletectedAccount;
        private PRODUCT UserSeletectedProduct;





        protected override void SetupGuisToDisplayStack()
        {
            GuiStack = new Stack<Guis>();
            GuiStack.Push(Guis.ProductSelection);
            GuiStack.Push(Guis.AccountSelection);
            GuiStack.Push(Guis.EmployeeSelection);
        }







        public IEnumerator ProceedToNextGui()
        {
            UserGuiManager.Show(GuiStack.Pop());
        }





    }
}
