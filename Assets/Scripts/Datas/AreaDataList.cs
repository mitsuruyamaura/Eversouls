using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaData", menuName = "ScriptableObjects/CreateAreaData")]
public class AreaDataList : ScriptableObject {

    public List<AreaData> areaDataList = new List<AreaData>();

    [System.Serializable]
    public class AreaData {
        public AREA_TYPE areaType;             // エリア名
        public TEMPERATURE_TYPE tempType;      // このエリアの温度設定
        public string fieldRate;               // 出現する地形の割合。カンマ区切りで作る
        public string fieldType;               // 出現する地形のタイプ
        public string areaName;                // 初期設定の名称
        public int iconNo;
        public RARE_TYPE rareliry;
        public FIELD_TYPE[] fieldTypeAllay;
        public string areaInfo;
    }
}
