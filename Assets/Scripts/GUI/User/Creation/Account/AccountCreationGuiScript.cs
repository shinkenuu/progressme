using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;

namespace Assets.Scripts.GUI.User.Creation
{
    public class AccountCreationGuiScript : CreationGui<ACCOUNT>, ICreationGui
    {
        #region GUI Vars
        
        [SerializeField]
        private GameObject AccountNameIfGo;
        [SerializeField]
        private Text AccountNameText;

        #endregion


        #region GUI Setup

        protected override void OnEnable()
        {
            DisplayInputFields();
        }
        
        private void DisplayInputFields()
        {
            AccountNameText.text = string.Empty;
            AccountNameIfGo.SetActive(true);
        }


        #endregion


    }
}
