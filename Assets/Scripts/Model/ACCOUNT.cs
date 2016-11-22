using UnityEngine;

namespace Assets.Scripts.Model
{

    public class ACCOUNT : JSONable
    {
        public int id { get; set; }
        public string account_name { get; set; }
        public string account_type { get; set; }

        public void SetAccount(int id, string accountName, string accountType)
        {
            this.id = id;
            this.account_name = accountName;
            this.account_type = accountType;
        }
        
        public override JSONObject ToJson()
        {
            JSONObject jsonObj = new JSONObject(JSONObject.Type.OBJECT);
            jsonObj.AddField("id", id);
            jsonObj.AddField("account_name", account_name);
            jsonObj.AddField("account_type", account_type);

            return jsonObj;
        }

        public override void FromJson(string json)
        {
            JSONObject jsonObj = new JSONObject(json);

            for (byte i = 0; i < jsonObj.list.Count; i++)
            {
                switch (jsonObj.keys[i])
                {
                    case "id":
                        id = (int)jsonObj.list[i].i;
                        break;
                    case "account_name":
                        account_name = jsonObj.list[i].str;
                        break;
                    case "account_type":
                        account_type = jsonObj.list[i].str;
                        break;
                }
            }
        }
        
    }
}
