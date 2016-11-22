using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
    public class PROCESS : JSONable
    {
        public int id { get; set; }
        public string process_name { get; set; }
        public string description { get; set; }
        public int ideal_total_hours { get; set; }
        public bool repetable { get; set; }
        public bool optional { get; set; }
        public int department_id { get; set; }
        public int[] next_processes_ids { get; set; }

        public void SetProcess(int id, string name, string description, int ideal_total_hours, bool repetable, bool optional, int department_id)
        {
            this.id = id;
            this.process_name = name;
            this.description = description;
            this.ideal_total_hours = ideal_total_hours;
            this.repetable = repetable;
            this.optional = optional;
            this.department_id = department_id;
        }

        public override JSONObject ToJson()
        {
            JSONObject jsonObj = new JSONObject(JSONObject.Type.OBJECT);
            jsonObj.AddField("id", id);
            jsonObj.AddField("process_name", process_name);
            jsonObj.AddField("description", description);
            jsonObj.AddField("ideal_total_hours", ideal_total_hours);
            jsonObj.AddField("repetable", repetable);
            jsonObj.AddField("optional", optional);
            jsonObj.AddField("department_id", department_id);

            JSONObject array = new JSONObject(JSONObject.Type.ARRAY);
            jsonObj.AddField("next_processes_ids", array);

            if (next_processes_ids == null || next_processes_ids.Length < 1)
            {
                return jsonObj;
            }

            foreach (int nextProcessId in next_processes_ids)
            {
                array.Add(nextProcessId);
            }

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
                    case "process_name":
                        process_name = jsonObj.list[i].str;
                        break;
                    case "description":
                        description = jsonObj.list[i].str;
                        break;
                    case "ideal_total_hours":
                        ideal_total_hours = (int)jsonObj.list[i].i;
                        break;
                    case "repetable":
                        repetable = jsonObj.list[i].b;
                        break;
                    case "optional":
                        optional = jsonObj.list[i].b;
                        break;
                    case "department_id":
                        department_id = (int)jsonObj.list[i].i;
                        break;
                    case "next_processes_ids":
                        List<int> nextProcessesIds = new List<int>();
                        foreach (JSONObject j in jsonObj.list[i].list)
                        {
                            nextProcessesIds.Add((int)j.i);
                        }
                        next_processes_ids = nextProcessesIds.ToArray();
                        break;
                }
            }
        }
    }
}
