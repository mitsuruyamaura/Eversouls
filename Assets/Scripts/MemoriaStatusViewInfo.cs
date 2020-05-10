using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Memoriaの詳細説明用クラス
/// MemoriaDetailから生成される
/// </summary>
public class MemoriaStatusViewInfo : ViewInfoDetail
{
    // ステータス表示用UI
    public Text txtInfo;

    PlayFabManager.MemoriaData memoriaData;

    /// <summary>
    /// MemoriaStatusViewInfoを設定
    /// </summary>
    public override void SetupMemoriaStatusView(PlayFabManager.MemoriaData memoriaData, int memoriaViewNo) {       
        this.memoriaData = memoriaData;

        viewNo = memoriaViewNo;

        // UI設定(TODO 追加する)
        txtInfo.text = this.memoriaData.info;
    }
}
