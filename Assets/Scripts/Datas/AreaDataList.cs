using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaData", menuName = "ScritablaObjects/CreateAreaData")]
public class AreaDataList : ScriptableObject {

    public List<AreaData> areaDataList = new List<AreaData>();

    [System.Serializable]
    public enum TIME_TYPE {
        DAY,
        NIGHT,
    }

    [System.Serializable]
    public enum RARELITY {
        COMMON,
        UN_COMMON,
        RARE,
        S_RARE,
        MISTIC
    }

    [System.Serializable]
    public class AreaData {
        public AREA_TYPE areaType;             // エリア名
        public TIME_TYPE timeType;             // 日中か夜か
        public TEMPERATURE_TYPE tempType;      // このエリアの温度設定       
        public string branchRate;              // 他のエリアへの分岐の割合。カンマ区切りで作る
        public string fieldRate;               // 出現する地形の割合。カンマ区切りで作る
        public string fieldType;               // 出現する地形のタイプ
        public string defaultName;             // 初期設定の名称
        public int iconNo;
        public RARELITY rareliry;
    }
}
