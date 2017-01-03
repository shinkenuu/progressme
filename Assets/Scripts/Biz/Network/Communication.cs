using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts;
using Assets.Scripts.Biz;
using Assets.Scripts.Model;

namespace Assets.Scripts.Biz.Network
{
    public class Communication : NetworkBehaviour
    {
        public WatcherOfAll Watcher;
                
        public List<Command.Command> Commands;

        protected int _cmdIdx;
        public int CmdIndexer
        {
            get
            {
                return ++_cmdIdx % int.MaxValue; 
            }
        }

        protected virtual void OnEnable()
        {
            Watcher = GameObject.FindGameObjectWithTag("Watcher").GetComponent<WatcherOfAll>();
            Commands = new List<Command.Command>();
            _cmdIdx = 0;
        }
        

        protected virtual void ReplyCmn(string[] jsonArray, int requestId, bool closeRequest)
        {
            Command.Command cmd = Commands.FirstOrDefault(r => r.Id == requestId);

            if (cmd == null)
            {
                Debug.Log("Attempt to reply null request with id: " + requestId.ToString());
                return;
            }

            if (jsonArray != null)
            {
                cmd.JsonArray = jsonArray;
            }

            if (closeRequest)
            {
                cmd.Close();
                Commands.Remove(cmd);
            }
        }


        public void SendByPieces(List<string> jsonList, int cmdId)
        {
            string[] jsonArray = jsonList.ToArray();

            StringBuilder strBldr = new StringBuilder();

            foreach (string json in jsonArray)
            {
                if (ASCIIEncoding.Unicode.GetByteCount(strBldr.ToString() + json) > 1024)
                {
                    jsonList = strBldr.ToString().Split(new char[] { '|' }).ToList();
                    jsonList.RemoveAt(jsonList.Count() - 1);

                    RpcPopJsonArray(jsonList.ToArray(), cmdId, false);

                    strBldr.Remove(0, strBldr.Length);
                }

                strBldr.Append(json + '|');
            }

            jsonList = strBldr.ToString().Split(new char[] { '|' }).ToList();
            jsonList.RemoveAt(jsonList.Count() - 1);

            RpcPopJsonArray(jsonList.ToArray(), cmdId, true);
        }
        

        [ClientRpc]
        public void RpcPopJsonArray(string[] jsonArray, int cmdId, bool closeRequest)
        {
            if (!isLocalPlayer)
            {
                return;
            }

            if (jsonArray == null || jsonArray.Count() == 0 || string.IsNullOrEmpty(jsonArray[0]))
            {
                Debug.Log("jsonArray provided by the server came null!");
                ReplyCmn(null, cmdId, true);
                return;
            }

            ReplyCmn(jsonArray, cmdId, closeRequest);
        }
                
    }
}
