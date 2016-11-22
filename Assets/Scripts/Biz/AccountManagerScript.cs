using UnityEngine;
using System;
using System.Collections;
using Assets.Scripts.Model;

public class AccountManagerScript : MonoBehaviour {

    public static readonly string AccountsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\ProgressMe\Accounts\";

    private RepositoryWriter _repWriter;

    private void OnEnable()
    {
        StartCoroutine(LookForRepositoryReader());
    }





    public void CreateAccount(ref ACCOUNT newAccount)
    {
        if(newAccount == null)
        {
            newAccount = null;
            return;
        }

        //iF account already exists
        if(_repWriter.GetAccount(newAccount.account_name) != null)
        {
            Debug.Log("Account " + newAccount.account_name + " already exists. Aborting account cration");
            newAccount = null;
            return;
        }

        //Check the account type
        switch (newAccount.account_type)
        {
            case "oem":
            case "portal":
            case "leasing":
            case "components or consultancy":
                break;
            default:
                Debug.Log("Account type " + newAccount.account_type + " is invalid. Aborting account cration");
                newAccount = null;
                break;
        }
                
        //Validation complete

        newAccount = _repWriter.AddNewAccount(newAccount);
    }













    #region LookFor

    private IEnumerator LookForRepositoryReader()
    {
        GameObject dataGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            dataGo = GameObject.FindWithTag("Storage");

            if (dataGo != null)
            { 
                _repWriter = dataGo.GetComponent<RepositoryWriter>();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    #endregion


}

