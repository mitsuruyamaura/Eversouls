using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TrapData", menuName = "ScriptableObjects/CreateTrapData")]
public class TrapDataList : ScriptableObject {

    public List<TrapData> trapDatas = new List<TrapData>();

    [System.Serializable]
    public class TrapData {
        public TRAP_TYPE trapType;
        public RARE_TYPE rarelity;
        public int appearance;
        public string info;

    }
}
