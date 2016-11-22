using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts.GUI.User.Selection.Product
{
    public class ProductSelectionGUIScript : SelectionGuiScript<PRODUCT, ProductSelectionGUIScript.ActionDisplay>
    {

        public enum ActionDisplay
        {
            AllProducts,
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            GameMng._tempProduct = null;
        }






        #region Selection Panel

        protected override void FetchNextAction()
        {
            if (actionsQueue.Count < 1)
            {
                return;
            }

            switch (actionsQueue.First())
            {
                case ActionDisplay.AllProducts:
                    Handle_AllProducts();
                    break;
                default:
                    throw new System.InvalidOperationException("ScheduledActionId " +
                        actionsQueue.First().ToString() + " not identified in " + this.ToString());
            }

            RefreshPanel();
            actionsQueue.Dequeue();
        }


        protected override void PresetPanelElement(GameObject panelElem, PRODUCT elemSubject)
        {
            panelElem.name = "btn_product_selection_option_" + elemSubject.product_name;
            panelElem.GetComponent<ThumbnailScript>().SetImageComponent(
                "Sprites/Product/" + elemSubject.product_name + "/logo", 50, 50);
        }

        #endregion







        private void Handle_AllProducts()
        {
            Selectables = RepReader.GetAllProducts().ToList();
        }
        




        #region Buttons


        protected override void OnPanelButtonClick(PRODUCT param)
        {
            Selected = param;
            SelectedAlertTxt.text = Selected.product_name.Trim();
        }

        #endregion
    }
}
