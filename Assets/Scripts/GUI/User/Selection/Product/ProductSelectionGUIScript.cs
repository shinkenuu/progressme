using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts.GUI.User.Selection.Product
{
    public class ProductSelectionGuiScript : SelectionGui<PRODUCT>, IUserGui
    {
        
        protected override void PresetPanelElement(GameObject panelElem, PRODUCT elemSubject)
        {
            panelElem.name = "btn_product_selection_option_" + elemSubject.product_name;
            panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
                "Sprites/Product/" + elemSubject.product_name + "/logo", 50, 50);
        }
        
                
        protected override void OnPanelButtonClick(PRODUCT param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.product_name.Trim();
        }
        
    }
}
