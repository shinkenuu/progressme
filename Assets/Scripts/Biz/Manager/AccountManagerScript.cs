using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using Assets.Scripts.Model;
using System.Collections.Generic;

namespace Assets.Scripts.Biz.Manager
{
    public class AccountManagerScript : EntityManager<ACCOUNT>
    {

        public static readonly string AccountsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ProgressMe\Accounts\";
        

        protected override void OnEnable()
        {
            base.OnEnable();

        }


        public void CreateAccount(ref ACCOUNT newAccount)
        {
            //If acc is invalid or already exists
            if (!Validate(newAccount) || RepWriter.GetAccount(newAccount.account_name) != null)
            {
                //Debug.Log("Account " + newAccount.account_name + " already exists. Aborting account cration");
                newAccount = null;
            }

            newAccount = RepWriter.AddNewAccount(newAccount);
        }


        public bool Validate(ACCOUNT acc)
        {
            if (acc == null)
            {
                return false;
            }

            //Check the account type
            switch (acc.account_type)
            {
                case "oem":
                case "portal":
                case "leasing":
                case "components or consultancy":
                    break;
                default:
                    Debug.Log("Account type " + acc.account_type + " is invalid.");
                    return false;
            }

            return true;
        }
        
    }
}
