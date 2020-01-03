using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SecretItemData", menuName = "ScriptableObjects/CreateSecretItemData")]
public class SecretItemDataList : ScriptableObject{

    public List<SecretItemData> secretItemDatas = new List<SecretItemData>();

    [System.Serializable]
    public class SecretItemData {
        public int no;
        public SECRET_ITEM_TYPE secretItemType;
        public RARE_TYPE rarelity;
        public int appearance;
        public string info;
    }
}
