using UnityEngine;
using System;

namespace Assets.Scripts.Model
{
    public class OPPORTUNITY_HISTORY : JSONable
    {
        public long id { get; set; }
        public long opportunity_id { get; set; }
        public int process_id { get; set; }
        public int responsable_employee_id { get; set; }
        public DateTime history_date { get; set; }
        public long? next_history_id { get; set; }

        public void SetOppHistory(OPPORTUNITY_HISTORY taskHistory)
        {
            id = taskHistory.id;
            opportunity_id = taskHistory.opportunity_id;
            process_id = taskHistory.process_id;
            responsable_employee_id = taskHistory.responsable_employee_id;
            history_date = taskHistory.history_date;
            next_history_id = taskHistory.next_history_id;
        }

        public void SetOppHistory(long id, long opportunity_id, int process_id, int responsable_employee_id, DateTime history_date)
        {
            this.id = id;
            this.opportunity_id = opportunity_id;
            this.process_id = process_id;
            this.responsable_employee_id = responsable_employee_id;
            this.history_date = history_date;
        }

        public void SetNextOppHistoryId(long next_history_id)
        {
            this.next_history_id = next_history_id;
        }
        
        public override JSONObject ToJson()
        {
            JSONObject jsonObj = new JSONObject(JSONObject.Type.OBJECT);
            jsonObj.AddField("id", id);
            jsonObj.AddField("opportunity_id", opportunity_id);
            jsonObj.AddField("process_id", process_id);
            jsonObj.AddField("responsable_employee_id", responsable_employee_id);
            jsonObj.AddField("history_date", history_date.ToString());
            jsonObj.AddField("next_history_id", next_history_id ?? 0);

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
                    case "opportunity_id":
                        opportunity_id = jsonObj.list[i].i;
                        break;
                    case "process_id":
                        process_id = (int)jsonObj.list[i].i;
                        break;
                    case "responsable_employee_id":
                        responsable_employee_id = (int)jsonObj.list[i].i;
                        break;
                    case "history_date":
                        history_date = DateTime.Parse(jsonObj.list[i].str);
                        break;
                    case "next_history_id":
                        next_history_id = (int)jsonObj.list[i].i;
                        break;
                }
            }
        }

    }
}
