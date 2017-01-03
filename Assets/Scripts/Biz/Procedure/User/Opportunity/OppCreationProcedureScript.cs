using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User.Selection;
using Assets.Scripts.Biz.Procedure;

namespace Assets.Scripts.Biz.Procedure.User
{
    public class OppCreationProcedureScript : UserProcedure<OPPORTUNITY>, IProcedure
    {
        private EMPLOYEE UserSelectedEmployee;
        private ACCOUNT UserSelectedAccount;
        private PRODUCT UserSelectedProduct;

        
        protected override IEnumerator UserProceed()
        {
            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(SelectionGui.Employee);
            CurrentSelectionGui.SetSelectables<EMPLOYEE>(
                RepReader.GetEmployeesIn(RepReader.GetDepartment("sales").id));
            CurrentSelectionGui.Subscribe<EMPLOYEE>(SetUserSelectedEmployee);
            yield return null;

            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(SelectionGui.Account);
            CurrentSelectionGui.SetSelectables<ACCOUNT>(
                RepReader.GetAllAccounts());
            CurrentSelectionGui.Subscribe<ACCOUNT>(SetUserSelectedAccount);
            yield return null;

            CurrentSelectionGui = UserGuiFactory.CreateSelectionGui(SelectionGui.Product);
            CurrentSelectionGui.SetSelectables<PRODUCT>(
                RepReader.GetAllProducts());
            CurrentSelectionGui.Subscribe<PRODUCT>(SetUserSelectedProduct);
            yield return null;

            if (!ValidateSelectedEntities())
            {
                throw new InvalidOperationException("Cannot proceed with invalid entities");
            }

            OPPORTUNITY newOpp = OPPORTUNITY.CreateInstance<OPPORTUNITY>();
            newOpp.SetOpportunity(0, "", "", DateTime.Now, UserSelectedEmployee.id, 0, 0, UserSelectedEmployee.id, UserSelectedEmployee.id);
            Command(newOpp);
            yield break;
        }
        


        protected override void Command(OPPORTUNITY opp)
        {
            UserCmn.InsertOpportunity(opp, ReceiveCommandBack);
        }



        #region Validation

        protected override bool ValidateSelectedEntities()
        {
            return ValidateSelectedEmployee() &&
                ValidateSelectedEmployee() &&
                ValidateSelectedProduct();
        }


        private bool ValidateSelectedEmployee()
        {
            if (UserSelectedEmployee == null || RepReader.GetDepartment(UserSelectedEmployee.department_id).department_name == "sales")
            {
                Debug.Log("::CRITICAL:: Attempt to create a opportunity with no selected responsable employee");
                return false;
            }

            return true;
        }


        private bool ValidateSelectedAccount()
        {
            if (UserSelectedEmployee == null)
            {
                Debug.Log("::CRITICAL:: Attempt to create a opportunity with no selected account");
                return false;
            }

            return true;
        }


        private bool ValidateSelectedProduct()
        {
            if (UserSelectedEmployee == null)
            {
                Debug.Log("::CRITICAL:: Attempt to create a opportunity with null product");
                return false;
            }

            return true;
        }

        #endregion
        
        #region Setters

        public void SetUserSelectedEmployee(EMPLOYEE emp)
        {
            UserSelectedEmployee = emp;
        }

        public void SetUserSelectedAccount(ACCOUNT acc)
        {
            UserSelectedAccount = acc;
        }

        public void SetUserSelectedProduct(PRODUCT prd)
        {
            UserSelectedProduct = prd;
        }

        #endregion

    }
}
