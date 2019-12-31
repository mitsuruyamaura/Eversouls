using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AreaData", menuName = "ScritablaObjects/CreateAreaData")]
public class AreaDataList : ScriptableObject {

    public List<AreaData> areaDataList = new List<AreaData>();

    [System.Serializable]
    public class AreaData {
        public AREA_TYPE areaType;
        public string branchRate;    // 他のエリアへの分岐の割合。カンマ区切りで作る
        public string fieldRate;     // 出現する地形の割合。カンマ区切りで作る
        public string fieldType;     // 出現する地形のタイプ
        public int iconNo;
    }
}
