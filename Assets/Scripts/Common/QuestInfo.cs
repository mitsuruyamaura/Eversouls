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
public class QuestInfo : MonoBehaviour
{
    [Header("クエストの通し番号")]
    public long no;
    [Header("クリアまでの必要なポイント(未使用)")]
    public int clearCount;   
    [Header("このクエストにおいてエリア分岐の発生する回数(未使用)")]
    public int branchCount;
    public string  areaInfo;

    // UI関連
    [Header("エリアのタイプ名表示")]
    public TMP_Text txtAreaType;
    public Image imgArea;
    public Button btnSubmit;

    [HideInInspector]
    public QuestSelectPopup questSelectPopup;
    [HideInInspector]
    public bool isClickable;　　　          // 重複タップ防止用フラグ

    [Header("エンカウント判定イベントデータ")]
    public GameData.QuestData checkEventData;
    public List<GameData.QuestData> questDataList = new List<GameData.QuestData>();

    /// <summary>
    /// クエストデータを設定
    /// </summary>
    /// <param name="areaNo"></param>
    public void InitQuestData(int areaNo, QuestSelectPopup questSelectPopup) {
        isClickable = true;
        this.questSelectPopup = questSelectPopup;
        imgArea.DOFade(1, 0.5f);
        foreach (AreaDataList.AreaData data in GameData.instance.areaDatas.areaDataList) {
            if ((AREA_TYPE)areaNo == data.areaType) {
                // 生成されたクエストのデータをエリアから設定
                FIELD_TYPE[] fieldTypes = GetFieldTypes(data.fieldType);
                List<FieldDataList.FieldData> fieldDatas = GetFieldDatas(fieldTypes);
                Debug.Log(fieldTypes.Length);
                Debug.Log(fieldDatas.Count);

                // エリアに含まれるすべての地形データをQuestDataに入れてリスト化
                for (int i = 0; i < fieldDatas.Count; i++) {
                    GameData.QuestData questData = new GameData.QuestData();
                    // 各出現率を文字列からIntに変更し、配列に入れる
                    questData.eventsRates = fieldDatas[i].events.Split(',').Select(int.Parse).ToArray();
                    questData.enemyEncountRates = fieldDatas[i].enemyEncount.Split(',').Select(int.Parse).ToArray();
                    questData.secretItemRates = fieldDatas[i].secretItem.Split(',').Select(int.Parse).ToArray();
                    questData.landscapeRates = fieldDatas[i].landscape.Split(',').Select(int.Parse).ToArray();
                    questData.trapRates = fieldDatas[i].trap.Split(',').Select(int.Parse).ToArray();

                    // 他のデータを設定
                    questData.field = fieldDatas[i].fieldType;
                    questData.fieldDatas = fieldDatas;
                    questData.areaRarelity = data.rareliry;
                    questData.tempratureType = data.tempType;               
                    questData.feildRates = GetFieldRates(data.fieldRate);
                    questData.areaType = data.areaType;
                    questData.areaInfo = data.areaInfo;
                    questData.iconNo = data.iconNo;

                    questDataList.Add(questData);                 
                }
                // UI設定
                imgArea.sprite = Resources.Load<Sprite>("Areas/" + data.iconNo);
                areaInfo = data.areaInfo;
                branchCount = 0;
            }
        }
        btnSubmit.onClick.AddListener(() => StartCoroutine(OnClickSubmit()));
        isClickable = false;
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
    public List<FieldDataList.FieldData> GetFieldDatas(FIELD_TYPE[] fieldTypes) {
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
    /// 選択したQuestDataをGameDataに渡し、ポップアップを閉じる
    /// </summary>
    public IEnumerator OnClickSubmit() {
        if (isClickable) {
            yield break;
        }
        isClickable = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);

        // GameDataにQuestDataの参照を保存
        GameData.instance.questDataList = new List<GameData.QuestData>();
        for (int i = 0; i < questDataList.Count; i++) {
            GameData.instance.questDataList.Add(questDataList[i]);
        }
        yield return new WaitForSeconds(0.15f);
        // ポップアップを閉じる準備
        questSelectPopup.PreparateQuestScene();
    }

    /***************************未使用******************************/

    /// <summary>
    /// エリアに任意の名前を付ける
    /// </summary>
    //public void GiveUniqueAreaName() {

    //}

    /// <summary>
    /// 移動開始前に敵が出現するかチェック
    /// </summary>
    //public bool CheckEncountEnemy(FIELD_TYPE fieldType, float amount) {  // 地形情報をもらって判断する
    //    bool isEncount = false;
    //    float value = UnityEngine.Random.Range(0, 100);
    //    Debug.Log(value);
    //    foreach (GameData.QuestData data in GameData.instance.questDataList) {
    //        if (data.field == fieldType) {
    //            checkEventData = data;
    //            if (value <= data.eventsRates[0] + amount) {
    //                isEncount = true;
    //            }
    //        }      
    //    }
    //    return isEncount;
    //}

    //public bool CheckEncoutSecret(FIELD_TYPE fieldType, float amount) {
    //    bool isEncount = false;
    //    float value = UnityEngine.Random.Range(0, 100);
    //    if (value <= checkEventData.eventsRates[1] + amount) {
    //        isEncount = true;
    //    }
    //    return isEncount;
    //}

    //public bool CheckEncountTrap(FIELD_TYPE fieldType, float amount) {
    //    bool isEncount = false;
    //    float value = UnityEngine.Random.Range(0, 100);
    //    if (value <= checkEventData.eventsRates[2] + amount) {
    //        isEncount = true;
    //    }
    //    return isEncount;
    //}

    //public bool CheckEncountLandscape(FIELD_TYPE fieldType, float amount) {
    //    bool isEncount = false;
    //    float value = UnityEngine.Random.Range(0, 100);
    //    if (value <= checkEventData.eventsRates[3] + amount) {
    //        isEncount = true;
    //    }
    //    return isEncount;
    //}
}
