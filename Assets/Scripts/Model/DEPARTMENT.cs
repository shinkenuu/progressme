using UnityEngine;

namespace Assets.Scripts.Model
{
    public class DEPARTMENT : JSONable
    {
        public int id { get; set; }
        public string department_name { get; set; }

        public void SetBusinessLayer(int id, string business_layer_name)
        {
            this.id = id;
            this.department_name = business_layer_name;
        }

        public override JSONObject ToJson()
        {
            JSONObject jsonObj = new JSONObject(JSONObject.Type.OBJECT);
            jsonObj.AddField("id", id);
            jsonObj.AddField("department_name", department_name);

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
                    case "department_name":
                        department_name = jsonObj.list[i].str;
                        break;
                }
            }
        }
    }
}
