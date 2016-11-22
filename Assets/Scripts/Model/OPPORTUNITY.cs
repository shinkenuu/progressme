using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.Model
{
    public class OPPORTUNITY : JSONable
    {
        public long id { get; set; }
        public string opportunity_name { get; set; }
        public string note { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public int? responsable_employee_id { get; set; }
        public int? current_process_id { get; set; }
        public int[] unrepetable_past_processes { get; set; }
        public int final_process_id { get; set; }
        public int product_id { get; set; }
        public int account_id { get; set; }

        public void SetOpportunity(OPPORTUNITY opportunity)
        {
            id = opportunity.id;
            opportunity_name = opportunity.opportunity_name;
            note = opportunity.note;
            start_date = opportunity.start_date;
            end_date = opportunity.end_date;
            responsable_employee_id = opportunity.responsable_employee_id;
            current_process_id = opportunity.current_process_id;
            product_id = opportunity.product_id;
            account_id = opportunity.account_id;
        }

        public void SetOpportunity(long id, string opportunity_name, string note, DateTime start_date, int responsable_employee_id, int current_process_id, int final_process_id, int product_id, int account_id)
        {
            this.id = id;
            this.opportunity_name = opportunity_name;
            this.note = note;
            this.start_date = start_date;
            this.end_date = null;
            this.responsable_employee_id = responsable_employee_id;
            this.current_process_id = current_process_id;
            this.unrepetable_past_processes = new int[] { };
            this.product_id = product_id;
            this.account_id = account_id;
        }

        public override JSONObject ToJson()
        {
            JSONObject jsonObj = new JSONObject(JSONObject.Type.OBJECT);
            jsonObj.AddField("id", id);
            jsonObj.AddField("opportunity_name", opportunity_name);
            jsonObj.AddField("note", note);
            jsonObj.AddField("start_date", start_date.ToString());
            jsonObj.AddField("end_date", end_date == null ? "null" : end_date.ToString());
            jsonObj.AddField("responsable_employee_id", responsable_employee_id ?? 0);
            jsonObj.AddField("current_process_id", current_process_id ?? 0);
            jsonObj.AddField("final_process_id", final_process_id);
            jsonObj.AddField("product_id", product_id);
            jsonObj.AddField("account_id", account_id);

            JSONObject array = new JSONObject(JSONObject.Type.ARRAY);
            jsonObj.AddField("unrepetable_past_processes", array);

            if (unrepetable_past_processes == null || unrepetable_past_processes.Length < 1)
            {
                return jsonObj;
            }

            foreach (int nextProcessId in unrepetable_past_processes)
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
                    case "opportunity_name":
                        opportunity_name = jsonObj.list[i].str;
                        break;
                    case "note":
                        note = jsonObj.list[i].str;
                        break;
                    case "start_date":
                        start_date = DateTime.Parse(jsonObj.list[i].str);
                        break;
                    case "end_date":
                        if(jsonObj.list[i].str == "null")
                        {
                            end_date = null;
                        }
                        else
                        {
                            end_date = DateTime.Parse(jsonObj.list[i].str);
                        }
                        break;
                    case "responsable_employee_id":
                        if(jsonObj.list[i].i == 0)
                        {
                            responsable_employee_id = null;
                        }
                        else
                        {
                            responsable_employee_id = (int)jsonObj.list[i].i;
                        }
                        break;
                    case "current_process_id":
                        if (jsonObj.list[i].i == 0)
                        {
                            current_process_id = null;
                        }
                        else
                        {
                            current_process_id = (int)jsonObj.list[i].i;
                        }
                        break;
                    case "final_process_id":
                            final_process_id = (int)jsonObj.list[i].i;
                        break;
                    case "unrepetable_past_processes":
                        List<int> nextProcessesIds = new List<int>();
                        foreach (JSONObject j in jsonObj.list[i].list)
                        {
                            nextProcessesIds.Add((int)j.i);
                        }
                        unrepetable_past_processes = nextProcessesIds.ToArray();
                        break;
                    case "product_id":
                        product_id = (int)jsonObj.list[i].i;
                        break;
                    case "account_id":
                        account_id = (int)jsonObj.list[i].i;
                        break;
                }
            }
        }
        
    }

}
