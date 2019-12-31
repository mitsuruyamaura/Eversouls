using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldData", menuName = "ScritablaObjects/CreateFieldData")]
public class FieldDataList : ScriptableObject {

    public List<FieldData> fieldDataList = new List<FieldData>();

    [System.Serializable]
    public class FieldData {
        public FIELD_TYPE fieldType;   // 地形タイプ
        public int cost;               // 移動するためのコスト
        public float progress;         // 成功時にプラスされる進捗度。失敗時は半分
        public int imageNo;            // 地形のイメージ。Resourcesから読み込むので番号で管理
        public string info;            // 地形の情報
        public float criticalRate;     // クリティカル発生確率。クリティカルするとコスト0で進捗2倍。
        public string encount;         // 敵の出現率{出現しない/ノーマル/アンコモン/レア}
        public string chest;           // 宝箱の出現率{出現しない/ノーマル/アンコモン/レア}
        public int[] encountRates;     
        public int[] chestRates;
    }
}
