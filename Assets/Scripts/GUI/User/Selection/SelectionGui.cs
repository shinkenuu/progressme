using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.GUI.User.Selection
{
    public enum SelectionGui
    {
        None,
        Account,
        Employee,
        Opp,
        Product,
        Process
    }

    public abstract class SelectionGui<TSelectable> : UserGui, IUserGui, ISelectionGui where TSelectable : class
    {
        public delegate void SelectionConfirmedHandler(TSelectable selected);
        public delegate void SelectionCanceledHandler();

        public event SelectionConfirmedHandler SelectionConfirmed;
        public event SelectionCanceledHandler SelectionCanceled;

        protected RepositoryReader RepReader;

        #region GUI Vars

        [SerializeField]
        protected Text SelectedAlertTxt;

        [SerializeField]
        protected Transform SelectionPanel;
        [SerializeField]
        protected Button SelectionBtnPrefab;

        #endregion

        protected IEnumerable<TSelectable> Selectables;

        public TSelectable Selected;


        protected override void OnEnable()
        {
            base.OnEnable();
            SelectedAlertTxt.text = string.Empty;
            Selectables = new List<TSelectable>();
            Selected = null;
            RepReader = Watcher.RepReader;
        }



        public void SetSelectables<T>(IEnumerable<T> selectables) where T : class
        {
            Selectables = selectables as IEnumerable<TSelectable>;
        }



        public void Subscribe<T>(Selection.SelectionGui<T>.SelectionConfirmedHandler func) where T : class
        {
            SelectionConfirmed += func as Selection.SelectionGui<TSelectable>.SelectionConfirmedHandler;
        }

        public void Subscribe<T>(Selection.SelectionGui<T>.SelectionCanceledHandler func) where T : class
        {
            SelectionCanceled += func as Selection.SelectionGui<TSelectable>.SelectionCanceledHandler;
        }



        protected void OnSelectionConfirmed(TSelectable param)
        {
            if(SelectionConfirmed == null)
            {
                return;
            }

            OnSelectionConfirmed(param);
        }

        protected void OnSelectionCanceled()
        {
            if (SelectionCanceled == null)
            {
                return;
            }

            OnSelectionCanceled();
        }



        #region Panels Control
        
        public void RefreshPanel()
        {
            if (SelectionPanel.childCount > 0)
            {
                ClearPanel(SelectionPanel);
            }

            GameObject panelElem = null;

            foreach (TSelectable selectable in Selectables)
            {
                panelElem = Instantiate(SelectionBtnPrefab).gameObject;
                panelElem.transform.SetParent(SelectionPanel, false);

                TSelectable selectableCopy = selectable;
                PresetPanelElement(panelElem, selectableCopy);
                panelElem.GetComponent<Button>().onClick.AddListener(() => { OnPanelButtonClick(selectableCopy); });
            }

            Debug.Log(this.ToString() + "'s panel was populated completely");
        }
                

        protected abstract void PresetPanelElement(GameObject panelElem, TSelectable elemSubject);

        #endregion




        #region Buttons


        public override void BtnConfirm_Click()
        {
            if (Selected == null)
            {
                InformUser("Please, select a " + Selected.ToString().ToLower() + " before confirm.");
                return;
            }

            OnSelectionConfirmed(Selected);
        }


        public override void BtnCancel_Click()
        {
            OnSelectionCanceled();
        }



        protected abstract void OnPanelButtonClick(TSelectable param);

        #endregion

    }
}
