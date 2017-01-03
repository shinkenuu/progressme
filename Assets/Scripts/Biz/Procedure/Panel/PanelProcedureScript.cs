using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Biz.Network;
using Assets.Scripts.Model;

namespace Assets.Scripts.Biz.Procedure.Panel
{
    public class PanelProcedureScript : Procedure, IProcedure
    { 
        private AnimationScript Animation;

        private EMPLOYEE[] _employeesArray;
        private int _employeeArrayIdx = 0;

        private EMPLOYEE FetchEmployee
        {
            get
            {
                _employeeArrayIdx = ++_employeeArrayIdx % _employeesArray.Length;
                return _employeesArray[_employeeArrayIdx];
            }
        }




        public void RefreshAvatarStatus(TimeSpan timeSpan)
        {
            if (timeSpan.Days < -2)
            {
                Animation.SetAction(AnimationScript.Action.prone);
                Animation.SetExpression(AnimationScript.Expression.despair);
            }

            else if (timeSpan.Days < -1)
            {
                Animation.SetAction(AnimationScript.Action.alert);
                Animation.SetExpression(AnimationScript.Expression.troubled);
            }

            else if (timeSpan.Days < 1)
            {
                Animation.SetAction(AnimationScript.Action.stand1);
                Animation.SetExpression(AnimationScript.Expression.blink);
            }

            else if (timeSpan.TotalDays < 2)
            {
                Animation.SetAction(AnimationScript.Action.alert);
                Animation.SetExpression(AnimationScript.Expression.glitter);
            }

            else
            {
                Animation.SetAction(AnimationScript.Action.shootF);
                Animation.SetExpression(AnimationScript.Expression.dam);
            }
        }




        public override IEnumerator Proceed()
        {
            throw new NotImplementedException();
        }
    }
}
