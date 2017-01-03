using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI.User;
using Assets.Scripts.GUI.User.Creation;
using Assets.Scripts.GUI.User.Selection;

namespace Assets.Scripts.GUI.User
{

    public enum Guis
    {
        None,
        OppDetails
    };
    

    public class UserGuiFactory : MonoBehaviour
    {
        
        private IUserGui CurrentGui;        


        public ISelectionGui CreateSelectionGui(SelectionGui gui)
        {
            if(CurrentGui != null)
            {
                CurrentGui.Close();
            }

            GameObject selectionGuiGo;

            switch(gui)
            {
                case SelectionGui.None:
                    CurrentGui = null;
                    return CurrentGui as ISelectionGui;
                case SelectionGui.Account:
                    selectionGuiGo = Instantiate(Resources.Load<GameObject>("Prefabs/GUI/AccSelectionGui"), transform) as GameObject;
                    selectionGuiGo.transform.SetParent(transform);
                    CurrentGui = selectionGuiGo.GetComponent<GUI.User.Selection.Account.AccountSelectionGuiScript>();
                    return CurrentGui as ISelectionGui;
                case SelectionGui.Employee:
                    selectionGuiGo = Instantiate(Resources.Load<GameObject>("Prefabs/GUI/EmpSelectionGui"), transform) as GameObject;
                    selectionGuiGo.transform.SetParent(transform);
                    CurrentGui = selectionGuiGo.GetComponent<GUI.User.Selection.Employee.EmployeeSelectionGuiScript>();
                    return CurrentGui as ISelectionGui;
                case SelectionGui.Opp:
                    selectionGuiGo = Instantiate(Resources.Load<GameObject>("Prefabs/GUI/OppSelectionGui"), transform) as GameObject;
                    selectionGuiGo.transform.SetParent(transform);
                    CurrentGui = selectionGuiGo.GetComponent<GUI.User.Selection.Opportunity.OppSelectionGuiScript>();
                    return CurrentGui as ISelectionGui;
                case SelectionGui.Product:
                    selectionGuiGo = Instantiate(Resources.Load<GameObject>("Prefabs/GUI/PrdSelectionGui"), transform) as GameObject;
                    selectionGuiGo.transform.SetParent(transform);
                    CurrentGui = selectionGuiGo.GetComponent<GUI.User.Selection.Product.ProductSelectionGuiScript>();
                    return CurrentGui as ISelectionGui;
                default:
                    throw new NotImplementedException(gui.ToString());
            }
        }                
    }
}
