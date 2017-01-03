using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.GUI.User.Selection
{
    public interface ISelectionGui : IUserGui
    {
        void RefreshPanel();

        void SetSelectables<T>(IEnumerable<T> selectables) where T : class;

        void Subscribe<T>(Selection.SelectionGui<T>.SelectionConfirmedHandler func) where T : class;

        void Subscribe<T>(Selection.SelectionGui<T>.SelectionCanceledHandler func) where T : class;
    }
}
