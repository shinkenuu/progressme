using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Biz.Procedure.User
{
    public enum UserProcedures
    {
        CreateOpp,
        CreateAccount,
        TransferOpp,
        AdvanceOpp
    }

    public class UserProcedureFactory : MonoBehaviour
    {




        public IProcedure CreateUserProcedure(UserProcedures proc)
        {
            throw new NotImplementedException();
        }


    }
}
