using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GUI.User
{
    public abstract class UserGui : GenericGUI, IUserGui
    {        

        [SerializeField]
        protected Text MsgTxt;



        public abstract void BtnConfirm_Click();

        public abstract void BtnCancel_Click();




        protected void InformUser(string msg)
        {
            MsgTxt.text = msg;
        }


    }
}
