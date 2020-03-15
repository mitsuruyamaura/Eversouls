using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharaData", menuName = "ScriptableObjects/CreateCharaData")]
public class CharaDataList : ScriptableObject
{
    public List<CharaData> charaDataList = new List<CharaData>();

    [System.Serializable]
    public class CharaData {
        public string name;
        public int no;
        public RARE_TYPE rarelity;
        public ROLL_TYPE rollType;
        public WEAPON_TYPE weaponType;
        public ELEMENT_TYPE magicType;
        public int imageNo;
    }
}
