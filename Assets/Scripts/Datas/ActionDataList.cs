using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ActionData", menuName = "ScriptableObjects/CreateActionData")]
public class ActionDataList : ScriptableObject
{
    public List<ActionData> actionDataList = new List<ActionData>();

    [System.Serializable]
    public class ActionData {
        public ACTION_TYPE actionType; // 行動タイプ
        public int cost;               // 移動するためのコスト
        public int progress;           // 成功時にプラスされる進捗度。失敗時は半分
        public int imageNo;
        public string info;
        public float criticalRate;     // クリティカル発生確率。クリティカルするとコスト0で進捗2倍。
        public float value;
    }
}
