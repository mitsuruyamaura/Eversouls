using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestManager : MonoBehaviour
{
    // プレファブ関連
    [Header("クエスト用プレファブ")]
    public QuestData questDataPrefab;
    [Header("行動用プレファブ")]
    public ActionInfo actionInfoPrefab;
    [Header("イベント用プレファブ")]
    public EventInfo eventInfoPrefab;
    [Header("移動用プレファブ")]
    public MovePanelInfo moveInfoPrefab;
    [Header("スキル用プレファブ")]
    public SkillInfo skillInfoPrefab;

    [Header("探索しているクエストのリスト")]
    public List<QuestData> questList = new List<QuestData>();
    [Header("現在の行動用スキルのリスト")]
    public List<ActionInfo> eventActionList = new List<ActionInfo>();
    [Header("現在の移動用スキルのリスト")]
    public List<SkillInfo> moveSkillsList = new List<SkillInfo>();
    [Header("現在のイベント用スキルのリスト")]
    public List<SkillInfo> eventSkillsList = new List<SkillInfo>();
    [Header("現在の移動パネルのリスト")]
    public List<MovePanelInfo> moveList = new List<MovePanelInfo>();

    public List<PlayFabManager.SkillData> questSkillDatas = new List<PlayFabManager.SkillData>();

    [Header("クエスト用パネルの生成位置")]
    public Transform questTran;
    [Header("行動用スキルパネルの生成位置")]
    public Transform actionTran;
    [Header("移動用スキルパネルの生成位置")]
    public Transform moveSkillTran;
    [Header("イベント用パネルの生成位置")]
    public Transform eventTran;
    [Header("中央位置")]
    public Transform centerTran;
    [Header("移動パネルの生成位置")]
    public Transform moveInfoTran;
    [Header("行動用プレファブの基本生成回数")]
    public int actionBaseCount;

    // UI関連
    [Header("イベント用行動リストの表示/非表示切替用")]
    public CanvasGroup scrollViewEventActionCanvasGroup;
    [Header("移動用スキルリストの表示/非表示切替用")]
    public CanvasGroup scrollViewMoveSkillCanvasGroup;
    [Header("移動力Header関連の表示/非表示切替用")]
    public GameObject headerApObj;
    
    [Header("APの更新表示用")]
    public TMP_Text txtAp;
    [Header("進捗度の更新表示用")]
    public TMP_Text txtProgress;
    [Header("進捗度のバー表示用")]
    public Slider slider;
    [Header("進捗度のアニメ制御用")]
    public Image imgProgress;
    [Header("実行中のイベント(地形、敵、罠、宝箱など)イメージ表示用")]
    public Image imgEvent;
    [Header("エリアイメージ表示フェイドイン用")]
    public Image imgAreaBack;
    [Header("エリアイメージ表示フェイドアウト用")]
    public Image imgAreaFront;

    [Header("クエストの進捗度")]
    public float progressPoint;
    [Header("宝箱の天井値")]
    public int ceilChestPoint;
    [Header("出現したイベント")]
    public List<EventInfo> eventList = new List<EventInfo>();

    public bool isEvent;
    public int ap;
    public bool isUpdateSkill;   // イベントなどでスキルに変更があった場合

    private void Start() {
        SoundManager.Instance.PlayBGM(SoundManager.ENUM_BGM.QUEST);
        StartCoroutine(TransitionManager.instance.EnterScene());
        CreateStartPanel();
        ap = 100;
    }

    /// <summary>
    /// 初期移動可能エリアの選択用オブジェクトの生成
    /// </summary>
    public void CreateStartPanel() {
        // スタートエリアを選択するためQuestDataオブジェクトをインスタンスする
        if (!GameData.instance.endTutorial) {
            // チュートリアルが終わっていなければチュートリアル用エリアの生成
            QuestData quest = Instantiate(questDataPrefab, questTran, false);
            quest.questManager = this;
            quest.InitQuestData(0);
            quest.no = 0;
            quest.clearCount = 10;           
            questList.Add(quest);
        } else {
            for (int i = 0; 0 < GameData.instance.questClearCountsByArea[0]; i++) {
                QuestData quest = Instantiate(questDataPrefab, questTran, false);
                int areaType = Random.Range(0, GameData.instance.areaDatas.areaDataList.Count);
                quest.InitQuestData(areaType);
                quest.no = GameData.instance.totalCount++;
                quest.questManager = this;
                questList.Add(quest);
            }
        }
        // プレイヤーの所持しているスキルデータを取得
        SetupSkillDatas();

        // 移動用のスキルリストの作成
        CreateSkillListOfEventType(EVENT_TYPE.移動, moveSkillsList, moveSkillTran);
        scrollViewMoveSkillCanvasGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// Playerの所持しているスキルのデータを取得
    /// </summary>
    private void SetupSkillDatas() {
        foreach (PlayFabManager.SkillData skillData in GameData.instance.haveSkillDatas) {
            skillData.amountCount = GameData.instance.GetSkillAmountCount(skillData.cost, skillData.skillAbilityNo);
            questSkillDatas.Add(skillData);          
        }
    }

    /// <summary>
    /// 指定されたタイプのスキルパネルを生成し、リストを作成
    /// </summary>
    private void CreateSkillListOfEventType(EVENT_TYPE eventType, List<SkillInfo> skillList, Transform tran) {
        // 取得したスキルのリストから移動用のスキルを抽出してパネルを作る
        foreach (PlayFabManager.SkillData skillData in questSkillDatas) {
            for (int i =0; i < skillData.eventTypes.Length; i++) {
                if (skillData.eventTypes[i] == eventType) {
                    SkillInfo skillInfo = Instantiate(skillInfoPrefab, tran, false);
                    skillInfo.questManager = this;
                    skillInfo.InitSkillPanelInfo(skillData);
                    skillList.Add(skillInfo);
                }
            }
        }
    }

    /// <summary>
    /// 対象のスキルリストを破棄
    /// </summary>
    public void DestroySkillListOfEventType(List<SkillInfo> destroyList) {
        for (int i = 0; i < destroyList.Count; i++) {
            Destroy(destroyList[i].gameObject);
        }
        destroyList.Clear();
    }

    /// <summary>
    /// Header項目を更新
    /// </summary>
    public void UpdateHeaderInfo(int cost, float progress) {
        // APをアニメ更新
        int currentAp = ap;
        int updateAp = currentAp - cost;
        DOTween.To(
            () => currentAp,
            (x) => {
                currentAp = x;
                txtAp.text = x.ToString();
            },
            updateAp,
            1.0f);
        ap = updateAp;

        // Progressをアニメ更新
        float currentProgress = progressPoint;
        float updataProgress = currentProgress + progress;
        if (updataProgress > 100) {
            updataProgress = 100;
        }
        DOTween.To(
            () => currentProgress,
            (x) => {
                currentProgress = x;
                txtProgress.text = x.ToString("F0");
            },
            updataProgress,
            1.0f);
        progressPoint = updataProgress;

        //txtAp.text = GameData.instance.ap.ToString();
        //txtProgress.text = progressPoint.ToString();
        //slider.value = (progressPoint / 100);
        // 進捗バーをアニメ表示
        imgProgress.fillAmount = (progressPoint / 100);
        slider.DOValue(imgProgress.fillAmount, 0.5f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// 使用したスキルの使用回数を更新(クエスト内で共用化)
    /// </summary>
    public void UpdateActiveSkillsAmountCount(List<SkillInfo> list) {      
        for (int i = 0; i < list.Count; i++) {
            foreach (PlayFabManager.SkillData skillData in questSkillDatas) {
                if (skillData.skillNo == list[i].skillData.skillNo) {
                    skillData.amountCount = list[i].skillData.amountCount;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 移動時のイベント発生判定
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveJudgment(FieldDataList.FieldData fieldData, EVENT_TYPE eventType, bool isLucky) {
        // スキルの使用回数を更新して移動スキルのパネルを非表示にする       
        UpdateActiveSkillsAmountCount(moveSkillsList);
        scrollViewMoveSkillCanvasGroup.gameObject.SetActive(false);
        scrollViewMoveSkillCanvasGroup.DOFade(0f, 0.5f);
        yield return new WaitForSeconds(0.5f);

        //EVENT_TYPE eventType = new EVENT_TYPE();
        //// クリティカル以外はイベント判定を順番にチェックする
        //float[] amount = new float[4];
        //foreach (ActionDataList.ActionData data in GameData.instance.actionDataList.actionDataList) {
        //    if (data.actionType == action) {
        //        switch (action) {
        //            case ACTION_TYPE.警戒移動:
        //                // 敵の出現率と罠の出現率ダウン
        //                amount[0] = data.value;
        //                amount[2] = data.value;
        //                break;
        //            case ACTION_TYPE.探索:
        //                // 秘匿物と景勝地の出現率アップ
        //                amount[1] = data.value;
        //                amount[3] = data.value;
        //                break;
        //        }
        //    }
        //}

        // チェックする地形の番号を渡す
        //if (questList[0].CheckEncountEnemy(field, amount[0])) {
        //    Debug.Log("敵が出現");
        //    eventType = EVENT_TYPE.敵;
        //} else if (questList[0].CheckEncoutSecret(field, amount[1])) {
        //    Debug.Log("秘匿物を発見");
        //    eventType = EVENT_TYPE.秘匿物;
        //} else if (questList[0].CheckEncountTrap(field, amount[2])) {
        //    Debug.Log("罠を発見");
        //    eventType = EVENT_TYPE.罠;
        //} else if (questList[0].CheckEncountLandscape(field, amount[3])) {
        //    Debug.Log("景勝地を発見");
        //    eventType = EVENT_TYPE.景勝地;
        //} else {
        //    Debug.Log("移動");
        //    eventType = EVENT_TYPE.移動;
        //}
        //Debug.Log(eventType);
        // もしもイベントが開いていたらそれを破壊
        if (eventList.Count > 0) {
            for (int i = 0; i < eventList.Count; i++) {
                Destroy(eventList[i].gameObject);
            }
            eventList.Clear();           
        }
        isEvent = false;
        int cost = fieldData.cost;
        float progress = fieldData.progress;

        Debug.Log(eventType);
        // 上記の行動に合わせて分岐し、その中で行為判定を行う
        if (eventType == EVENT_TYPE.移動) {
            // 移動の場合
            // 最初にクリティカルかどうか判定
            if (CheckActionCritical(fieldData.criticalRate)) {
                Debug.Log("Critical!");
                // クリティカルならボーナス授与
                cost = 0;
                progress *= 2;
            }
            // 移動用パネルを破棄し、新しい移動用パネルを生成
            DestroyMovePanelsAndEventPanels(fieldData.imageNo);

            // 進捗度を更新
            UpdateHeaderInfo(cost, progress);
        } else {
            isEvent = true;
            // 移動以外ならイベントを作成する
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.FIND);
            EventInfo eventInfo = Instantiate(eventInfoPrefab, eventTran, false);
            eventInfo.Init(eventType, questList[0], fieldData.fieldType, cost, progress, fieldData.imageNo, isLucky);
            eventList.Add(eventInfo);

            // 移動用パネルを破棄(イベント解決まで移動パネルは作らない)
            DestroyMovePanelsAndEventPanels(fieldData.imageNo, false);

            // イベントの種類に応じた行動パネルを生成
            //eventInfo.ChooseActions();
            CreateSkillListOfEventType(eventType, eventSkillsList, actionTran);

            UpdateHeaderInfo(cost, progress);
            CreateEventActions(eventType);
        }     
    }

    /// <summary>
    /// 選択した行動の成否判定
    /// </summary>
    /// <returns></returns>
    public IEnumerator ActionJudgment(int cost, float progress, int fieldImageNo, ACTION_TYPE actionType) {
        yield return new WaitForSeconds(0.5f);

        if (actionType == ACTION_TYPE.先を急ぐ) {
            // もしもイベントが開いていたらそれを破壊
            if (eventList.Count > 0) {
                for (int i = 0; i < eventList.Count; i++) {
                    eventList[i].HideEventInfo();
                    Destroy(eventList[i].gameObject, 0.5f);
                }
                eventList.Clear();
                isEvent = false;
            }
            //UpdateHeaderInfo(cost, progress);
            DestroyMovePanelsAndEventPanels(fieldImageNo);            
        }
        scrollViewEventActionCanvasGroup.DOFade(0f, 0.5f);
        scrollViewEventActionCanvasGroup.gameObject.SetActive(false);        
    }

    /// <summary>
    /// イベントに応じた行動パネルを生成
    /// </summary>
    /// <param name="eventType"></param>
    public void CreateEventActions(EVENT_TYPE eventType) {
        scrollViewEventActionCanvasGroup.gameObject.SetActive(true);
        scrollViewEventActionCanvasGroup.DOFade(1.0f, 0.5f);

        eventActionList = new List<ActionInfo>();

        // 必ず「先を急ぐ(諦める)」パネルを表示する
        ActionInfo action = Instantiate(actionInfoPrefab, actionTran, false);
        action.questManager = this;
        //action.InitField(questList[0].fieldDatas[j], GameData.instance.actionDataList.actionDataList[Random.Range(0, GameData.instance.actionDataList.actionDataList.Count)]);

        action.InitAction(GameData.instance.actionDataList.actionDataList[0]);
        
        eventActionList.Add(action);
    }

    /// <summary>
    /// クリティカル判定
    /// </summary>
    /// <param name="criticalRate"></param>
    /// <returns></returns>
    public bool CheckActionCritical(float criticalRate) {
        bool isCritical = false;
        float value = Random.Range(0, 100);
        if (value <= criticalRate) {
            isCritical = true;
        }
        return isCritical; 
    }

    /// <summary>
    /// 移動先パネルを規定数だけ生成
    /// </summary>
    /// <param name="iconNo"></param>
    public void CreateMovePanelInfos(int fieldNo) {
        // 移動用背景イメージをアニメ表示
        StartCoroutine(SetFieldImage(fieldNo));

        // 所持スキルのリストと移動力を表示
        scrollViewMoveSkillCanvasGroup.gameObject.SetActive(true);
        scrollViewMoveSkillCanvasGroup.DOFade(1.0f, 0.5f);
        // 所持スキルに変更があった場合には移動用スキルを再度作成しなおす
        if (isUpdateSkill) {
            DestroySkillListOfEventType(moveSkillsList);
            CreateSkillListOfEventType(EVENT_TYPE.移動, moveSkillsList, moveSkillTran);
        }

        for (int i = 0; i < moveSkillsList.Count; i++) {
            moveSkillsList[i].UpdateActiveSkill();
        }
        // 移動力を表示する
        if (!headerApObj.activeSelf) {
            headerApObj.SetActive(true);
        }       
        UpdateHeaderInfo(0, 0);

        eventActionList = new List<ActionInfo>();
        moveList = new List<MovePanelInfo>();

        // エリアごとに生成可能な地形の出現割合を合計
        int total = 0;
        for (int i = 0; i < questList[0].feildRates.Length; i++) {
            total += questList[0].feildRates[i];
        }

        // 移動可能な地形をランダムに生成する
        for (int i = 0; i < actionBaseCount; i++) {
            int value = Random.Range(0, total + 1);
            for (int j = 0; j < questList[0].feildRates.Length; j++) {
                if (value <= questList[0].feildRates[j]) {
                    MovePanelInfo moveInfo = Instantiate(moveInfoPrefab, moveInfoTran, false);
                    StartCoroutine(moveInfo.ChangePanelScale(0.5f + (i * 0.5f)));
                    moveInfo.InitMovePanel(questList[0].fieldDatas[j]);
                    moveInfo.questManager = this;
                    moveList.Add(moveInfo);
                    break;
                } else {
                    value -= questList[0].feildRates[j];
                }
            }
        }
    }

    /// <summary>
    /// 移動パネルと各イベント用行動パネルを破棄し、新しい移動パネルの生成を呼び出す
    /// </summary>
    public void DestroyMovePanelsAndEventPanels(int fieldImageNo, bool isCreate = true) {
        if (moveList.Count > 0) {
            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < moveList.Count; i++) {
                if (moveList[i].transform.localScale.x > 0) {
                    seq.Append(moveList[i].canvasGroup.DOFade(0, 0.25f));
                    seq.Join(moveList[i].transform.DOScale(0, 0.25f));
                }
                Destroy(moveList[i].gameObject, 0.8f);
            }
            moveList.Clear();
        }
        if (eventActionList.Count > 0) {
            for (int i = 0; i < eventActionList.Count; i++) {
                Destroy(eventActionList[i].gameObject);
            }
            eventActionList.Clear();
        }
        if (eventSkillsList.Count > 0) {
            for (int i = 0; i < eventSkillsList.Count; i++) {
                Destroy(eventSkillsList[i].gameObject);
            }
            eventSkillsList.Clear();
        }
        if (isCreate) {
            // イベント発生の場合は始めはfalseなので移動パネルは生成しない
            CreateMovePanelInfos(fieldImageNo);
        }
    }

    /// <summary>
    /// 背景イメージを選択したクエストのエリアのイメージに切り替える
    /// </summary>
    /// <param name="areaType"></param>
    public IEnumerator SetAreaImage(AREA_TYPE areaType) {
        // 背景用の後ろのイメージを変更しておく
        imgAreaBack.sprite = Resources.Load<Sprite>("Areas/" + (int)areaType);

        // 手前側のイメージをフェイドアウトさせる
        Sequence seq = DOTween.Sequence();
        seq.Append(imgAreaFront.DOFade(0f, 1.0f));
        seq.Join(imgAreaBack.DOFade(0.75f, 1.0f));

        yield return new WaitForSeconds(1.0f);

        // 次のために手前側のイメージも変更しておく
        imgAreaFront.sprite = Resources.Load<Sprite>("Areas/" + (int)areaType);
        seq.Append(imgAreaFront.DOFade(0.75f, 1.0f));
        seq.Join(imgAreaBack.DOFade(0f, 1.0f));
    }

    public IEnumerator SetFieldImage(int fieldNo) {
        // 背景用の後ろのイメージを変更しておく
        imgAreaBack.sprite = Resources.Load<Sprite>("Fields/" + fieldNo);

        // 手前側のイメージをフェイドアウトさせる
        Sequence seq = DOTween.Sequence();
        seq.Append(imgAreaFront.DOFade(0f, 1.0f));
        seq.Join(imgAreaBack.DOFade(0.75f, 1.0f));

        yield return new WaitForSeconds(1.0f);

        // 次のために手前側のイメージも変更しておく
        imgAreaFront.sprite = Resources.Load<Sprite>("Fields/" + fieldNo);
        seq.Append(imgAreaBack.DOFade(0f, 0.01f));       
        seq.Join(imgAreaFront.DOFade(0.75f, 0.01f));
    }
}
