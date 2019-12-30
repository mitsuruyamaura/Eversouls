using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharaData", menuName = "ScritablaObjects/CreateCharaData")]
public class CharaDataList : ScriptableObject
{
    public List<CharaData> charaDataList = new List<CharaData>();

    [System.Serializable]
    public class CharaData {
        public string name;
        public int no;
        public RARE_TYPE rare;
        public ROLL_TYPE charaType;
        public WEAPON_TYPE weaponType;
        public MAGIC_TYPE magicType;
        public int imageNo;
    }
}
