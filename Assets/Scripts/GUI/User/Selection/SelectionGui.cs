using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.GUI.User.Selection
{
    public abstract class SelectionGui<TSelectable, VAction> : UserGui where TSelectable : class
    {
        public delegate void OnSelectionConfirm(TSelectable selected);
        public OnSelectionConfirm SelectionConfirmEvent
        {
            protected get;
            set;
        }

        #region GUI Vars

        [SerializeField]
        protected Text SelectedAlertTxt;

        [SerializeField]
        protected Transform SelectionPanel;
        [SerializeField]
        protected Button SelectionBtnPrefab;

        #endregion
        
        /*
                FILL_USER_INFO = 1,
                REFRESH_OPPORTUNITY_PANEL = 2,
                SETUP_HOME = 3
        */


        protected Queue<VAction> actionsQueue;
        protected List<TSelectable> Selectables;

        public TSelectable Selected
        {
            protected set;
            get;
        }


        protected override void OnEnable()
        {
            base.OnEnable();
            actionsQueue = new Queue<VAction>();
            SelectedAlertTxt.text = string.Empty;
            Selectables = new List<TSelectable>();
        }


        public void QueueAction(VAction action)
        {
            if (action == null)
            {
                throw new System.ArgumentNullException("selectionGuiAction cannot be null");
            }

            actionsQueue.Enqueue(action);

            if (actionsQueue.Count == 1)
            {
                FetchNextAction();
            }
        }

        protected abstract void FetchNextAction();



        #region Panels Control

        protected void ClearPanel(Transform panelTransform)
        {
            List<Transform> childrenTransform = new List<Transform>();
            foreach (Transform child in panelTransform.GetComponentInChildren<Transform>())
            {
                childrenTransform.Add(child);
            }

            foreach (Transform child in childrenTransform)
            {
                Destroy(child.gameObject);
            }
        }

        protected void RefreshPanel()
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
                InformUser("Please, select a " + Selected.ToString().ToLower() + " before advancing to the next step.");
                return;
            }

            SelectionConfirmEvent.Invoke(Selected);
        }

        protected abstract void OnPanelButtonClick(TSelectable param);

        #endregion

    }
}
