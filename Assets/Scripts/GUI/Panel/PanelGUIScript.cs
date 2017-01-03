using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Biz.Network;
using Assets.Scripts.Model;

namespace Assets.Scripts.GUI.Panel
{

    [RequireComponent(typeof(AnimationScript))]
    public class PanelGuiScript : GenericGUI
    {        
        [SerializeField]
        private Image EmployeeInFocusImage;
        [SerializeField]
        private Transform PanelTransform;

        [SerializeField]
        private Transform PanelOppThumbnailPrefab;


        private List<OPPORTUNITY> _oppsInFocus;

        public OPPORTUNITY[] OppsInFocus
        {
            private get
            {
                return _oppsInFocus.ToArray();
            }

            set
            {
                if(_oppsInFocus == null || value == null)
                {
                    _oppsInFocus = new List<OPPORTUNITY>(); 
                }

                _oppsInFocus = value.ToList();
            }
        }

        




        #region Panel Manipulation

        private void RefreshPanel()
        {
            if (PanelTransform.childCount > 0)
            {
                ClearPanel(PanelTransform);
            }
            
            foreach (OPPORTUNITY opp in OppsInFocus)
            {
                AddToPanel(opp);
            }
        }


        private void AddToPanel(OPPORTUNITY opp)
        {
            GameObject panelOppThumbnailGo = Instantiate(PanelOppThumbnailPrefab).gameObject;
            panelOppThumbnailGo.name = panelOppThumbnailGo.name.Replace("(Clone)", "") + opp.id.ToString();
            panelOppThumbnailGo.transform.SetParent(PanelTransform, false);
        }
        
        #endregion


        


    }
}
