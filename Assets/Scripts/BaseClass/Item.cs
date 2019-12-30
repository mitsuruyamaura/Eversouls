using System;

/// <summary>
/// アイテムの基本クラス
/// </summary>
[Serializable]
public class Item {

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
    // 買値
    public int buyPrice;
    // 売値
    public int sellPrice;
    // 新規フラグ
    public bool newFlg;
    // お気に入りフラグ
    public bool favoriteFlg;
    // フレーバーテキスト
    public string fravorTxt;

    // 一括販売フラグ
    public bool sellBulkFlg;
    // アイテム選択時のフラグ
    public bool selectFlg;

}