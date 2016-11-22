using UnityEngine;

namespace Assets.Scripts.Model
{
    public class PRODUCT : JSONable
    {
        public int id { get; set; }
        public string product_name { get; set; }
        public int first_process_id { get; set; }
        public int final_process_id { get; set; }

        public void SetProduct(int id, string product_name, int first_process_id, int final_process_id)
        {
            this.id = id;
            this.product_name = product_name;
            this.first_process_id = first_process_id;
            this.final_process_id = final_process_id;
        }

        public override JSONObject ToJson()
        {
            JSONObject jsonObj = new JSONObject(JSONObject.Type.OBJECT);
            jsonObj.AddField("id", id);
            jsonObj.AddField("product_name", product_name);
            jsonObj.AddField("first_process_id", first_process_id);
            jsonObj.AddField("final_process_id", final_process_id);

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
                    case "product_name":
                        product_name = jsonObj.list[i].str;
                        break;
                    case "first_process_id":
                        first_process_id = (int)jsonObj.list[i].i;
                        break;
                    case "final_process_id":
                        final_process_id = (int)jsonObj.list[i].i;
                        break;
                }
            }
        }
    }
}
