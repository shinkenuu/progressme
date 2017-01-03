//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Assets.Scripts.Biz.Procedure.User.Account
//{
//    public class AccCreationProcedureScript : Procedure, IProcedure
//    {


//        public IEnumerator CreateAccount()
//        {
//            if (string.IsNullOrEmpty(_tempAccount.account_name) || string.IsNullOrEmpty(_tempAccount.account_type))
//            {
//                _accountSelectionGui.InformUser("Please write the account's name and type before trying to create it");
//                yield break;
//            }

//            ACCOUNT newAccount = _tempAccount;
//            _tempAccount = null;

//            RequestScript createAccountRequest = RequestScript.CreateInstance<RequestScript>();
//            createAccountRequest.SetupRequest(RequestScript.RequestType.ADD_CUSTOMER, newAccount.ToJson().Print());

//            if (!createAccountRequest.IsReady())
//            {
//                Debug.LogError("Account creating request setup but wasn't ready. Aborting");
//                _accountSelectionGui.InformUser("Unable to create account");
//                yield break;
//            }

//            _userCmn.PerformRequest(ref createAccountRequest);
//            Debug.Log("Sent 'create account' command to server. Waiting for the reply...");
//            _accountSelectionGui.InformUser("Creating account...");

//            yield return new WaitUntil(createAccountRequest.IsClosed);

//            if (createAccountRequest.GetJsonArray() == null)
//            {
//                Debug.Log("::CRITICAL:: Attempt to create account failed");
//                _accountSelectionGui.InformUser("Unable to create account");
//                yield break;
//            }

//            Debug.Log("The account has been created");
//            newAccount.FromJson(createAccountRequest.GetJsonArray()[0]);
//            _repWriter.AddAccount(newAccount);

//            _accountSelectionGui.ScheduleAction(UserGUIScript.UserGuiAction.REFRESH_CUSTOMER_PANEL);

//            yield break;
//        }

//        public override IEnumerator Proceed
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }
//    }
//}
