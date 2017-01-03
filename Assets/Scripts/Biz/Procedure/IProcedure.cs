using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Model;
using Assets.Scripts.GUI;

namespace Assets.Scripts.Biz.Procedure
{
    public interface IProcedure
    {
        IEnumerator Proceed();
    }
}
