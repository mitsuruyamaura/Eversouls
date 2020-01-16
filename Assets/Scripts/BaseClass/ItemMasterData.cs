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
    }
}