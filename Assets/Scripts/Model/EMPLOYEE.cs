using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public class EMPLOYEE : JSONable
    {
        public int id { get; set; }
        public string employee_name { get; set; }
        public string password { get; set; }
        public int department_id { get; set; }
        public int manager_id { get; set; }
        
        public void SetEmployee(int id, string employee_name, string password, int department_id, int manager_id)
        {
            this.id = id;
            this.employee_name = employee_name;
            this.password = password;
            this.department_id = department_id;
            this.manager_id = manager_id;
        }

        public override JSONObject ToJson()
        {
            JSONObject jsonObj = new JSONObject(JSONObject.Type.OBJECT);
            jsonObj.AddField("id", id);
            jsonObj.AddField("employee_name", employee_name);
            jsonObj.AddField("password", password);
            jsonObj.AddField("department_id", department_id);
            jsonObj.AddField("manager_id", manager_id);

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
                    case "employee_name":
                        employee_name = jsonObj.list[i].str;
                        break;
                    case "password":
                        password = jsonObj.list[i].str;
                        break;
                    case "department_id":
                        department_id = (int)jsonObj.list[i].i;
                        break;
                    case "manager_id":
                        manager_id = (int)jsonObj.list[i].i;
                        break;
                }
            }
        }
    }
}
