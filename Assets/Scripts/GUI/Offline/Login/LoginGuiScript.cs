using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;

namespace Assets.Scripts.GUI.Offline.Login
{
    public class LoginGuiScript : OfflineGui, ILoginGui, IOfflineGui
    {
        [SerializeField]
        private InputField UsernameInputField;
        [SerializeField]
        private InputField PasswordInputField;


        public void SetupInputFields(UnityEngine.Events.UnityAction<string> usernameCall, UnityEngine.Events.UnityAction<string> passwordCall)
        {
            if (UsernameInputField == null || PasswordInputField == null)
            {
                throw new InvalidOperationException("UsernameInputField or PasswordInputField is null");
            }

            UsernameInputField.onEndEdit.AddListener(delegate { usernameCall(UsernameInputField.text); });
            PasswordInputField.onEndEdit.AddListener(delegate { passwordCall(PasswordInputField.text); });
        }


        public void SetInputFieldsActive(bool active)
        {
            UsernameInputField.gameObject.SetActive(active);
            PasswordInputField.gameObject.SetActive(active);

            if (active)
            {
                PasswordInputField.text = string.Empty;
            }
        }

    }
}
