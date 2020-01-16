using UnityEngine;

/// <summary>
/// Itemのマスターデータのスクリプタブル・オブジェクト
/// </summary>
[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObject/CreateItemSO")]
public class ItemSO : ScriptableObject {
    public ItemMasterData itemMasterData;
}
