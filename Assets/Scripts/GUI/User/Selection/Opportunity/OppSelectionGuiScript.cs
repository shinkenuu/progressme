using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.Biz.Manager;

namespace Assets.Scripts.GUI.User.Selection.Opportunity
{
    public class OppSelectionGuiScript : SelectionGui<OPPORTUNITY>, IUserGui
    {
        
        protected override void PresetPanelElement(GameObject panelElem, OPPORTUNITY elemSubject)
        {
            panelElem.name = "btn_opp_selection_option_" + elemSubject.opportunity_name;

            string accountOwnerName = RepReader.GetAccount(elemSubject.account_id).account_name;

            panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
                "file:///" + AccountManagerScript.AccountsFolder.Replace('\\', '/')
                    + accountOwnerName + "/logo.png",
                100,
                100);
        }
                

        protected override void OnPanelButtonClick(OPPORTUNITY param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.opportunity_name.Trim();
        }
        
    }
}
