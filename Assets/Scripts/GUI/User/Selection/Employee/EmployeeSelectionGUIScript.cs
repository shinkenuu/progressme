using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts.GUI.User.Selection.Employee
{
    public class EmployeeSelectionGuiScript : SelectionGui<EMPLOYEE>, IUserGui
    {
    
        protected override void PresetPanelElement(GameObject panelElem, EMPLOYEE elemSubject)
        {
            panelElem.name = "btn_employee_selection_option_" + elemSubject.employee_name;
            panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
                "Sprites/Employees/" + elemSubject.employee_name + "/profile", 50, 50);
        }
        

        protected override void OnPanelButtonClick(EMPLOYEE param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.employee_name.Trim().Replace('.', ' ');
        }
           
    }
}
