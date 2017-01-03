using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using System;

namespace Assets.Scripts.GUI.User.Selection.Process
{
    public class ProcessSelectionGuiScript : SelectionGui<PROCESS>, IUserGui
    {

        protected override void PresetPanelElement(GameObject panelElem, PROCESS elemSubject)
        {
            panelElem.name = "btn_pcs_selection_option_" + elemSubject.process_name;

            throw new NotImplementedException();

            //string processDescription = RepReader.GetProcess(elemSubject.id).description;

            //panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
            //    "file:///" + AccountManagerScript.AccountsFolder.Replace('\\', '/')
            //        + accountOwnerName + "/logo.png",
            //    100,
            //    100);
        }


        protected override void OnPanelButtonClick(PROCESS param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.process_name.Trim();
        }

    }
}
