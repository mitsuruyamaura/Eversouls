using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemMasterData {

    public List<ItemData> item = new List<ItemData>();

    /// <summary>
    /// アイテムの基本クラス
    /// </summary>
    [Serializable]
    public class ItemData {
        // アイテム固有番号
        public long itemNo;
        // アイテムコード
        public int itemCd;
        // アイテム名
        public string itemName;
        // アイテムアイコン名
        public string itemIcon;
        // アイテムタイプ
        public ITEM_TYPE itemType;
        // 売値
        public int sellPrice;
        // フレーバーテキスト
        public string fravorTxt;
        // 探索成功率
        public float successFix;
        // 敵とのエンカウント率
        public float encountFix;
        // クリティカル率
        public float criticalFix;
        // 命中率
        public float hittingFix;
        // 回避率
        public float evadeFix;
        // 攻撃力
        public int damageFix;
        // 防御力
        public int diffenceFix;
        // 回復力
        public int recoveryFix;
        // コスト
        public int cost;
        // 行動の場合、行動のタイプ
        public ACTION_TYPE actionType;
        // イメージ
        public int imageNo;

        public EVENT_TYPE eventType;
    }
}