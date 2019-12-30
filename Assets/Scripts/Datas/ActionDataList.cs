using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionData", menuName = "ScritablaObjects/CreateActionData")]
public class ActionDataList : ScriptableObject
{
    public List<ActionData> actionDataList = new List<ActionData>();

    [System.Serializable]
    public class ActionData {
        public ACTION_TYPE actionType;
        public int cost;
        public int imageNo;
        public string info;
    }
}
