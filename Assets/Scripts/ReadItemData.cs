using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ReadItemData : MonoBehaviour
{
    public string fileName;
    public ItemMasterData itemDataList;
    
    void Start()
    {
        // infoにStreamingAsset下にあるファイルを代入
        //FileInfo info = new FileInfo(Application.streamingAssetsPath + "/" + fileName);

        // readerに読み込ませる
        //StreamReader reader = new StreamReader(info.OpenRead());

        //string json = reader.ReadToEnd();

        // Jsonデータをパース
        //ItemDataList.ItemData itemData = JsonUtility.FromJson<ItemDataList.ItemData>(json);

        //foreach (ItemDataList.ItemData item in itemData) {

        //}

    }
}
