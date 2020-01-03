using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/CreateEnemyData")]
public class EnemyDataList : ScriptableObject {
    public List<EnemyData> enemyDatas = new List<EnemyData>();

    [System.Serializable]
    public class EnemyData {
        public int no;                             // 管理番号
        public string name;                        // 名前
        public FIELD_TYPE[] habitats;              // 生息エリア
        public RARE_TYPE rarelity;                 // 希少度
        public bool isTalk;                        // 会話可能かどうか
        public TIME_TYPE timeType;                 // 出現時間帯。ALLなら常時出現
        public int appearance;                     // 出現割合

        public int hp;
        public int speed;
        public WEAPON_TYPE weaponType;             // そのまま弱点属性になる
        public MAGIC_TYPE magicType;
        public string skills;
        public string chestType;
        public int exp;
    }
}
