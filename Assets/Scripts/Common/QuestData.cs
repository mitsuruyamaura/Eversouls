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

    public FIELD_TYPE[] fieldTypes;                // 出現する地形タイプ
    [Header("地形の出現割合")]
    public int[] feildRates;
    [Header("エリアのタイプ")]
    public AREA_TYPE areaType;
    [Header("このクエストにおいてエリア分岐の発生する回数")]
    public int branchCount;

    // UI関連
    [Header("エリアのタイプ名表示")]
    public TMP_Text txtAreaType;
    [Header("エリアの任意名称表示")]
    public TMP_Text txtUniqueAreaName;
    public Image imgArea;
    public Button btnSubmit;

    [HideInInspector]
    public QuestManager questManager;
    [HideInInspector]
    public bool isSubmit;　　　          // 重複タップ防止用フラグ
    [HideInInspector]
    public int iconNo;                   //地形のイメージ番号

    [System.Serializable]
    public class EventData {
        // stringを配列に入れるための用意
        [Header("イベント発生率(敵/秘匿物/罠/景勝地)")]
        public int[] eventsRates;
        [Header("敵の出現率")]
        public int[] enemyEncountRates;
        public int[] secretItemRates;
        public int[] landscapeRates;
        public int[] trapRates;
        public FIELD_TYPE field;
    }
    public List<EventData> eventDataList = new List<EventData>();

    [Header("昼夜")]
    public TIME_TYPE timeType;
    [Header("希少度")]
    public RARE_TYPE areaRarelity;
    [Header("温度")]
    public TEMPERATURE_TYPE tempratureType;

    [Header("エンカウント判定イベントデータ")]
    public EventData checkEventData;

    /// <summary>
    /// クエストデータを設定
    /// </summary>
    /// <param name="areaNo"></param>
    public void InitQuestData(int areaNo) {
        imgArea.DOFade(1, 0.5f);
        foreach (AreaDataList.AreaData data in GameData.instance.areaDatas.areaDataList) {
            if ((AREA_TYPE)areaNo == data.areaType) {
                // 生成されたクエストのデータを設定
                areaType = data.areaType;
                feildRates = GetFieldRates(data.fieldRate);
                fieldTypes = GetFieldTypes(data.fieldType);
                fieldDatas = GetFieldDatas();

                for (int i = 0; i < fieldDatas.Count; i++) {
                    EventData eventData = new EventData();
                    // 各出現率を文字列からIntに変更し、配列に入れる
                    eventData.eventsRates = new int[(int)EVENT_TYPE.COUNT];
                    eventData.enemyEncountRates = new int[5];
                    eventData.secretItemRates = new int[(int)SECRET_ITEM_TYPE.COUNT];                                       
                    eventData.landscapeRates = new int[(int)LANDSCAPE_TYPE.COUNT];
                    eventData.trapRates = new int[(int)TRAP_TYPE.COUNT];

                    eventData.eventsRates = fieldDatas[i].events.Split(',').Select(int.Parse).ToArray();
                    eventData.enemyEncountRates = fieldDatas[i].enemyEncount.Split(',').Select(int.Parse).ToArray();
                    eventData.secretItemRates = fieldDatas[i].secretItem.Split(',').Select(int.Parse).ToArray();
                    eventData.landscapeRates = fieldDatas[i].landscape.Split(',').Select(int.Parse).ToArray();
                    eventData.trapRates = fieldDatas[i].trap.Split(',').Select(int.Parse).ToArray();

                    eventData.field = fieldDatas[i].fieldType;

                    eventDataList.Add(eventData);
                }
                imgArea.sprite = Resources.Load<Sprite>("Areas/" + data.iconNo);
                iconNo = data.iconNo;
                branchCount = 0;
                timeType = data.timeType;
                areaRarelity = data.rareliry;
                tempratureType = data.tempType;
            }
        }
        btnSubmit.onClick.AddListener(() => StartCoroutine(OnClickSubmit()));
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
    public IEnumerator OnClickSubmit() {
        if (!isSubmit) {
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
            isSubmit = true;
            
            // アニメさせながら隠す
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOMove(questManager.centerTran.position, 1.0f));
            seq.Append(transform.DOScale(0f, 0.5f));
            yield return new WaitForSeconds(1.35f);
            
            // アクションを生成
            StartCoroutine(questManager.SetAreaImage(areaType));
            questManager.CreateMoveInfos(iconNo);

            yield return new WaitForSeconds(0.15f);
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// エリアに任意の名前を付ける
    /// </summary>
    public void GiveUniqueAreaName() {

    }

    /// <summary>
    /// 移動開始前に敵が出現するかチェック
    /// </summary>
    public bool CheckEncountEnemy(FIELD_TYPE fieldType, float amount) {  // 地形情報をもらって判断する
        bool isEncount = false;
        float value = UnityEngine.Random.Range(0, 100);
        Debug.Log(value);
        foreach (EventData data in eventDataList) {
            if (data.field == fieldType) {
                checkEventData = data;
                if (value <= data.eventsRates[0] + amount) {
                    isEncount = true;
                }
            }      
        }
        return isEncount;
    }

    public bool CheckEncoutSecret(FIELD_TYPE fieldType, float amount) {
        bool isEncount = false;
        float value = UnityEngine.Random.Range(0, 100);
        if (value <= checkEventData.eventsRates[1] + amount) {
            isEncount = true;
        }
        return isEncount;
    }

    public bool CheckEncountTrap(FIELD_TYPE fieldType, float amount) {
        bool isEncount = false;
        float value = UnityEngine.Random.Range(0, 100);
        if (value <= checkEventData.eventsRates[2] + amount) {
            isEncount = true;
        }
        return isEncount;
    }

    public bool CheckEncountLandscape(FIELD_TYPE fieldType, float amount) {
        bool isEncount = false;
        float value = UnityEngine.Random.Range(0, 100);
        if (value <= checkEventData.eventsRates[3] + amount) {
            isEncount = true;
        }
        return isEncount;
    }
}
