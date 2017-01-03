using System;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Biz.Network.Command
{
    public enum Status
    {
        Setup = 0,
        Ready = 1,
        Open = 2,
        Closed = 3
    }
    

    public delegate void CmdClosed(Command closedCmd);

    public class Command : ScriptableObject
    {
        protected CmdClosed CmdClosed;

        protected List<string> _jsonList;
        public string[] JsonArray
        {
            set
            {
                if (value == null)
                {
                    return;
                }

                else if (Status != Status.Open)
                {
                    throw new InvalidOperationException("Attempt to set JsonArray to a unopened command");
                }

                _jsonList.AddRange(value);
            }
            get
            {
                return _jsonList.ToArray();
            }
        }

        protected string AuxJson;

        public int Id
        {
            protected set;
            get;
        }

        public Status Status
        {
            protected set;
            get;
        }
        
                
        /// <summary>
        /// Prepare the Command to be sent
        /// </summary>
        /// <param name="action">Select, Update, Insert or Delete</param>
        /// <param name="auxJson">Conditional</param>
        /// <param name="func">Observer method</param>
        public void SetupCmd(int id, string auxJson)
        {
            if(id < 0)
            {
                throw new ArgumentException("id must be greater than -1");
            }

            Id = id;
            AuxJson = auxJson;
            Status = Status.Ready;
        }

        /// <summary>
        /// Opens the Cmd for setup of JsonArray
        /// </summary>
        /// <returns>The auxiliar json for any condition needed</returns>
        public string Open()
        {
            Status = Status.Open;
            return AuxJson;
        }

        /// <summary>
        /// Closes the Cmd and call all the observers
        /// </summary>
        public void Close()
        {
            Status = Status.Closed;
            OnCmdClosed();
        }
        
        protected void OnCmdClosed()
        {
            if(CmdClosed == null)
            {
                return;
            }

            CmdClosed(this);
        }

        public void SubscribeForClosed(CmdClosed func)
        {
            CmdClosed += func;
        }
    }
}
