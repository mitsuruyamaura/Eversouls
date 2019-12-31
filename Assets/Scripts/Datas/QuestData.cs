using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// クエストのデータ管理クラス
/// </summary>
public class QuestData : MonoBehaviour
{
    [Header("クエストの通し番号")]
    public long no;
    [Header("クリアまでの必要なポイント(未使用)")]
    public int clearCount;
    [Header("クエストに出現する地形リスト")]
    public List<FieldDataList.FieldData> fieldDatas;
    [HideInInspector]
    public FIELD_TYPE[] fieldTypes;                // 出現する地形タイプ
    [Header("地形の出現割合")]
    public int[] feildRates;
    [Header("エリアのタイプ")]
    public AREA_TYPE areaType;
    [Header("このクエストにおいてエリア分岐の発生する回数")]
    public int branchCount;

    // UI関連
    [Header("エリアのタイプ")]
    public TMP_Text txtAreaName;
    public Image imgArea;
    public Button btnSubmit;

    [HideInInspector]
    public QuestManager questManager;
    [HideInInspector]
    public bool isSubmit;　　　          // 重複タップ防止用フラグ
    [HideInInspector]
    public int iconNo;                   //地形のイメージ番号

    public void InitQuestData(int areaNo) {
        imgArea.DOFade(1, 0.5f);
        questManager.txtDebug.text += "InitQuest\n";
        foreach (AreaDataList.AreaData data in GameData.instance.areaDatas.areaDataList) {
            questManager.txtDebug.text = data.areaType.ToString() + "\n";
            if ((AREA_TYPE)areaNo == data.areaType) {
                // 生成されたクエストのデータを設定
                areaType = data.areaType;
                feildRates = GetFieldRates(data.fieldRate);
                fieldTypes = GetFieldTypes(data.fieldType);
                fieldDatas = GetFieldDatas();

                for (int i = 0; i < fieldDatas.Count; i++) {
                    // 敵の出現率と宝箱の出現率を配列に入れる
                    fieldDatas[i].encountRates = new int[4];
                    fieldDatas[i].chestRates = new int[4];
                    fieldDatas[i].encountRates = fieldDatas[i].encount.Split(',').Select(int.Parse).ToArray();
                    fieldDatas[i].chestRates = fieldDatas[i].chest.Split(',').Select(int.Parse).ToArray();
                }
                imgArea.sprite = Resources.Load<Sprite>("Areas/" + data.iconNo);
                iconNo = data.iconNo;
                branchCount = 0;
            }
        }
        btnSubmit.onClick.AddListener(OnClickSubmit);
    }

    /// <summary>
    /// エリアの地形の出現割合を算出
    /// </summary>
    /// <param name="fieldData"></param>
    /// <returns></returns>
    public int[] GetFieldRates(string fieldData) {
        int[] rates = fieldData.Split(',').Select(int.Parse).ToArray();
        return rates;
    }

    /// <summary>
    /// 地形のタイプを取得
    /// </summary>
    /// <param name="fieldTypes"></param>
    /// <returns></returns>
    public FIELD_TYPE[] GetFieldTypes(string fieldTypes) {
        int[] types = fieldTypes.Split(',').Select(int.Parse).ToArray();
        FIELD_TYPE[] feilds = new FIELD_TYPE[types.Length];
        for (int i = 0; i < types.Length; i++) {
            feilds[i] = (FIELD_TYPE)types[i];
        }
        return feilds;
    }

    /// <summary>
    /// 地形のデータを取得
    /// </summary>
    /// <returns></returns>
    public List<FieldDataList.FieldData> GetFieldDatas() {
        List<FieldDataList.FieldData> fieldList = new List<FieldDataList.FieldData>();
        foreach (FieldDataList.FieldData data in GameData.instance.fieldDataList.fieldDataList) {
            for (int i = 0; i < fieldTypes.Length; i++) {
                if (data.fieldType == fieldTypes[i]) {
                    fieldList.Add(data);
                }
            }
        }
        return fieldList;
    }

    /// <summary>
    /// 選択したクエストのデータをQuestManagerに渡す
    /// </summary>
    public void OnClickSubmit() {
        if (!isSubmit) {
            isSubmit = true;
            GameData.instance.questData = this;
            StartCoroutine(questManager.SetAreaImage(areaType));
            questManager.CreateActions(iconNo);

            // アニメさせながら破棄する
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(1.2f, 0.15f));
            seq.Append(transform.DOScale(1.0f, 0.15f));
            seq.Join(imgArea.DOFade(0, 0.15f));
            Destroy(gameObject, 0.3f);
        }
    }
}
