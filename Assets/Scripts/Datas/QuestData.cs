﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class QuestData : MonoBehaviour
{
    public long no;
    public int clearCount;                         // クリアまでの必要なポイント
    public List<FieldDataList.FieldData> fieldDatas;
    public FIELD_TYPE[] fieldTypes;                // 出現する地形タイプ
    public int[] feildRates;                       // 地形の出現割合
    public AREA_TYPE areaType;                     // エリアのタイプ
    public int branchCount;                        // エリア分岐の発生する回数
    public TMP_Text txtAreaName;
    public Image imgArea;
    public Button btnSubmit;
    public QuestManager questManager;
    public bool isSubmit;
    public int iconNo;
    public AreaDataList areaDataList;

    public void InitQuestData(int areaNo) {
        imgArea.DOFade(1, 0.5f);
        questManager.txtDebug.text += "InitQuest\n";
        foreach (AreaDataList.AreaData data in areaDataList.areaDataList) {
            questManager.txtDebug.text = data.areaType.ToString() + "\n";
            if ((AREA_TYPE)areaNo == data.areaType) {
                areaType = data.areaType;
                feildRates = GetFieldRates(data.fieldRate);
                fieldTypes = GetFieldTypes(data.fieldType);
                fieldDatas = GetFieldDatas();
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

    public void OnClickSubmit() {
        Debug.Log("Submit");
        if (!isSubmit) {
            isSubmit = true;
            GameData.instance.questData = this;
            StartCoroutine(questManager.SetAreaImage(areaType));
            questManager.CreateActions(iconNo);
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(1.2f, 0.15f));
            seq.Append(transform.DOScale(1.0f, 0.15f));
            seq.Join(imgArea.DOFade(0, 0.15f));
            Destroy(gameObject, 0.3f);
        }
    }
}
