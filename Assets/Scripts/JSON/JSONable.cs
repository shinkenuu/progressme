using UnityEngine;
using System.Collections;

[System.Serializable]
abstract public class JSONable : ScriptableObject
{
    abstract public JSONObject ToJson();
    abstract public void FromJson(string json);
}
