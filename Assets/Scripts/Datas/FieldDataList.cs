﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FieldData", menuName = "ScriptableObjects/CreateFieldData")]
public class FieldDataList : ScriptableObject {//31415

    public List<FieldData> fieldDataList = new List<FieldData>();

    [System.Serializable]
    public class FieldData {
        public FIELD_TYPE fieldType;   // 地形タイプ
        public int cost;               // 移動するためのコスト
        public float progress;         // 成功時にプラスされる進捗度。失敗時は半分
        public int imageNo;            // 地形のイメージ。Resourcesから読み込むので番号で管理
        public string info;            // 地形の情報
        public float criticalRate;     // クリティカル発生確率。クリティカルするとコスト0で進捗2倍
        public int searchRate;         // [?]の発生確率。探索に成功しないとイベントが発生しない 25%基準
        public string events;          // 敵/秘匿物/罠/景勝地の出現率。順番にチェックされて、入ったところで抜ける
        public string enemyEncount;    // レアリティごとの、出現する敵の種類と割合[0] => [4]の順に高くなる
        public string secretItem;      // 出現する秘匿物の種類の割合
        public string landscape;       // 出現する景勝地の種類と割合
        public string trap;            // 出現する罠の種類と割合
        public string firstActionRates;  // 敵/秘匿物/罠/景勝地の検出率。検出できると先制攻撃、罠解除などをできる。（反撃なしで１回多い行動）
    }
}
