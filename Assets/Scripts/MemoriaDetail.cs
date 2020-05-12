using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// Memoria管理用クラス
/// QuestSelectPopupにアタッチ
/// </summary>
public class MemoriaDetail : MonoBehaviour
{
    public Text txtMemoriaName;
    public Image imgMemoriaChara;
    public Button btnMemoria;
    public Text txtMemoriaLevel;
    public Text txtHaveMagicWord;
    public Text txtMemoriaExp;
    public Text txtClearCount;

    public MemoriaStatusViewInfo memoriaStatusViewInfoPrefab;

    public PlayFabManager.MemoriaData memoriaData;
    public QuestSelectPopup questSelectPopup;

    public bool isClickable;
    private int memoriaViewNo = 500;

    /// <summary>
    /// MemoriaDataを取得して設定(ポップアップの背景画像と名前)
    /// </summary>
    public void Setup() {
        // 使用するMemoriaDataを取得
        memoriaData = PlayFabManager.instance.memoriaDataList.Find(a => a.no == GameData.instance.activeMomoriaNo);

        // 名前とタイトルと背景用アイコン表示
        txtMemoriaName.text = memoriaData.title + "\n" + memoriaData.name;
        imgMemoriaChara.sprite = GameData.instance.spriteAtlas.GetSprite("Memoria_" + memoriaData.iconNo);

        txtMemoriaLevel.text = memoriaData.level.ToString();
        txtHaveMagicWord.text = memoriaData.haveMagicWord;      // カンマ区切りのまま使う。ゲームブックのフラグをイメージさせる
        txtMemoriaExp.text = memoriaData.exp.ToString();
        txtClearCount.text = memoriaData.clearCount.ToString();

        btnMemoria.onClick.AddListener(OnClickDetail);

        // MemoriaStatsuViewInfoを生成
        CreateMemoriaStatusViewInfo();
    }

    /// <summary>
    /// MemoriaStatusViewInfoを生成
    /// </summary>
    private void CreateMemoriaStatusViewInfo() {
        // SkillViewNoと被らないように500からスタート(TODO GameDataで連番を採番してもいい)
        memoriaViewNo += memoriaData.no;
        MemoriaStatusViewInfo memoriaStatusViewInfo = Instantiate(memoriaStatusViewInfoPrefab, questSelectPopup.viewInfoTran, false);
        memoriaStatusViewInfo.SetupMemoriaStatusView(memoriaData, memoriaViewNo);
        questSelectPopup.viewInfoList.Add(memoriaStatusViewInfo);
    }

    /// <summary>
    /// 他のViewInfoを非表示にして、MemoriaStatusViewInfoを表示する
    /// </summary>
    private void OnClickDetail() {
        if (isClickable) {
            return;
        }
        isClickable = true;

        // すべてのViewInfoを一旦非表示
        for (int i = 0; i < questSelectPopup.viewInfoList.Count; i++) {
            questSelectPopup.viewInfoList[i].SwitchActiveView(0f);
        }

        // MemoriaStatusViewDetailを再表示
        foreach (ViewInfoDetail viewInfo in questSelectPopup.viewInfoList.Where(x => x.viewNo == memoriaViewNo)) {
            // 再表示
            viewInfo.SwitchActiveView(1.0f);
            break;
        }
        isClickable = false;
    }
}
