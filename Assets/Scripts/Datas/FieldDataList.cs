using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldData", menuName = "ScritablaObjects/CreateFieldData")]
public class FieldDataList : ScriptableObject {

    public List<FieldData> fieldDataList = new List<FieldData>();

    [System.Serializable]
    public class FieldData {
        public FIELD_TYPE fieldType;
        public int cost;
        public int imageNo;
        public string info;
    }
}
