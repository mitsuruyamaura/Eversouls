using UnityEngine;

public class LoadMasterDataFromJson : MonoBehaviour
{
    public ItemSO itemSO;

    private void Awake() {
        if (!itemSO) {
            itemSO = Resources.Load<ItemSO>("MasterData/ItemMasterData");
        }
    }

    /// <summary>
    /// Jsonファイルを読み込んで、それをItemMasterDataに書き込む
    /// </summary>
    public void LoadFromJson() {
        itemSO.itemMasterData = new ItemMasterData();
        itemSO.itemMasterData = JsonUtility.FromJson<ItemMasterData>(JsonHelper.GetJsonFile("/", "item.json"));
    }
}
