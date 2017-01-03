using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Biz.Procedure.Offline
{
    public abstract class OfflineProcedure : Procedure, IProcedure
    {
        protected GUI.Offline.OfflineGuiFactory GuiFactory;
        protected Network.Offline.OfflineCmnScript OffCmn;

        protected override void OnEnable()
        {
            base.OnEnable();
            GuiFactory = Watcher.OfflineGuiFactory;

            try
            {
                OffCmn = Watcher.OfflineCmn;
            }

            catch(Exception)
            {

            }
        }
    }
}
