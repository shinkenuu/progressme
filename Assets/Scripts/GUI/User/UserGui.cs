using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GUI.User
{
    public abstract class UserGui : GenericGUI, IUserGui
    {       
        [SerializeField]
        protected Text MsgTxt;

        protected RepositoryReader RepReader;

        protected override void OnEnable()
        {
            base.OnEnable();
            RepReader = Watcher.RepReader;
        }



        public virtual void Initialize()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }


        public abstract void BtnConfirm_Click();

        public abstract void BtnCancel_Click();




        protected void InformUser(string msg)
        {
            MsgTxt.text = msg;
        }


    }
}
