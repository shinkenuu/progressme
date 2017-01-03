using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.Biz.Manager;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts.GUI.User.Selection.Account
{
    public class AccountSelectionGuiScript : SelectionGui<ACCOUNT>, IUserGui
    {
        
        protected override void PresetPanelElement(GameObject panelElem, ACCOUNT elemSubject)
        {
            panelElem.name = "btn_account_selection_option_" + elemSubject.account_name;
            panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
                "file:///" + AccountManagerScript.AccountsFolder.Replace('\\', '/')
                    + elemSubject.account_name + "/logo.png",
                100,
                100);
        }

        
        protected override void OnPanelButtonClick(ACCOUNT param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.account_name.Trim();
        }
        
    }
}