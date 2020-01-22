using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LandscapeData", menuName = "ScriptableObjects/CreateLandscapeData")]
public class LandscapeDataList : ScriptableObject {

    public List<LandscapeData> landscapeDatas = new List<LandscapeData>();

    [System.Serializable]
    public class LandscapeData {
        public LANDSCAPE_TYPE landscapeType;
        public RARE_TYPE rarelity;
        [Header("出現率")]
        public int appearance;
        public string info;
    }
}
