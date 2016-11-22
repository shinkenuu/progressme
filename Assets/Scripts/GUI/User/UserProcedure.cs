using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;

namespace Assets.Scripts.GUI.User
{
    public enum Procedures
    {
        CreateOpp,
        CreateCustomer,
        TransferOpp,
        AdvanceOpp
    }

    public abstract class UserProcedure<TResult> : MonoBehaviour
    {
        protected UserGuiManager UserGuiManager;
        protected Stack<Guis> GuiStack;

        public TResult ProcedureResult
        {
            protected set;
            get;
        }

        protected virtual void OnEnable()
        {
            SetupGuisToDisplayStack();
        }



        protected abstract void SetupGuisToDisplayStack();

    }
}
