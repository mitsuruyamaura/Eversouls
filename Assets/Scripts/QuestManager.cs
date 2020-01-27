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
    [Header("行動用プレファブ")]
    public EventInfo eventInfoPrefab;

    [Header("探索しているクエストのリスト")]
    public List<QuestData> questList = new List<QuestData>();
    [Header("現在の行動リスト")]
    public List<ActionInfo> actionList = new List<ActionInfo>();

    [Header("クエスト用プレファブの生成位置")]
    public Transform questTran;
    [Header("行動用プレファブの生成位置")]
    public Transform actionTran;
    [Header("イベント用プレファブの生成位置")]
    public Transform eventTran;
    [Header("中央位置")]
    public Transform centerTran;
    [Header("行動用プレファブの基本生成回数")]
    public int actionBaseCount;

    // UI関連
    [Header("行動リストの表示/非表示切替用")]
    public GameObject scrollViewAction;
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

    private void Start() {
        SoundManager.Instance.PlayBGM(SoundManager.ENUM_BGM.QUEST);
        StartCoroutine(TransitionManager.instance.EnterScene());
        Init();
    }

    /// <summary>
    /// 初期移動可能エリアの選択用オブジェクトの生成
    /// </summary>
    public void Init() {
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
            for (int i = 0; 0 < GameData.instance.clearQuestCount; i++) {
                QuestData quest = Instantiate(questDataPrefab, questTran, false);
                int areaType = Random.Range(0, GameData.instance.areaDatas.areaDataList.Count);
                quest.InitQuestData(areaType);
                quest.no = GameData.instance.totalCount++;
                quest.questManager = this;
                questList.Add(quest);
            }
        }
        UpdateHeaderInfo(0, 0);       
    }

    /// <summary>
    /// Header項目を更新
    /// </summary>
    public void UpdateHeaderInfo(int cost, float progress) {
        // APをアニメ更新
        int currentAp = GameData.instance.ap;
        int updateAp = currentAp - cost;
        DOTween.To(
            () => currentAp,
            (x) => {
                currentAp = x;
                txtAp.text = x.ToString();
            },
            updateAp,
            1.0f);
        GameData.instance.ap = updateAp;

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
    /// 移動時のイベント発生判定
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveJudgment(int cost, float progress, int iconNo, float criticalRate, FIELD_TYPE field, ACTION_TYPE action) {
        yield return new WaitForSeconds(0.5f);

        EVENT_TYPE eventType = new EVENT_TYPE();
        // クリティカル以外はイベント判定を順番にチェックする
        float[] amount = new float[4];
        foreach (ActionDataList.ActionData data in GameData.instance.actionDataList.actionDataList) {
            if (data.actionType == action) {
                switch (action) {
                    case ACTION_TYPE.警戒移動:
                        // 敵の出現率と罠の出現率ダウン
                        amount[0] = data.value;
                        amount[2] = data.value;
                        break;
                    case ACTION_TYPE.探索:
                        // 秘匿物と景勝地の出現率アップ
                        amount[1] = data.value;
                        amount[3] = data.value;
                        break;
                }
            }
        }

        // チェックする地形の番号を渡す
        if (questList[0].CheckEncountEnemy(field, amount[0])) {
            Debug.Log("敵が出現");
            eventType = EVENT_TYPE.敵;
        } else if (questList[0].CheckEncoutSecret(field, amount[1])) {
            Debug.Log("秘匿物を発見");
            eventType = EVENT_TYPE.秘匿物;
        } else if (questList[0].CheckEncountTrap(field, amount[2])) {
            Debug.Log("罠を発見");
            eventType = EVENT_TYPE.罠;
        } else if (questList[0].CheckEncountLandscape(field, amount[3])) {
            Debug.Log("景勝地を発見");
            eventType = EVENT_TYPE.景勝地;
        } else {
            Debug.Log("移動");
            eventType = EVENT_TYPE.移動;
        }
        Debug.Log(eventType);
        // もしもイベントが開いていたらそれを破壊
        if (eventList.Count > 0) {
            for (int i = 0; i < eventList.Count; i++) {
                Destroy(eventList[i].gameObject);
            }
            eventList.Clear();
            isEvent = false;
        }
        // 上記の行動に合わせて分岐し、その中で行為判定を行う
        if (eventType == EVENT_TYPE.移動) {
            // 移動の場合
            // 最初にクリティカルかどうか判定
            if (CheckActionCritical(criticalRate)) {
                Debug.Log("Critical!");
                // クリティカルならボーナス授与
                cost = 0;
                progress *= 2;
            }
            // その後、進捗度を更新し、次の行動を作成
            UpdateHeaderInfo(cost, progress);
            UpdateActions(iconNo);
        } else {
            isEvent = true;
            // 移動以外ならイベントを作成する
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.FIND);
            EventInfo eventInfo = Instantiate(eventInfoPrefab, eventTran, false);
            eventInfo.Init(eventType, questList[0], field, cost, progress, iconNo);
            eventList.Add(eventInfo);

            // 前の移動用パネルを破棄
            UpdateActions(iconNo, false);
            // イベントの種類に応じた行動パネルを生成
            //eventInfo.ChooseActions();

            CreateEventActions(eventType);
        }     
    }

    /// <summary>
    /// 選択した行動の成否判定
    /// </summary>
    /// <returns></returns>
    public IEnumerator ActionJudgment(int cost, float progress, int iconNo, ACTION_TYPE actionType) {
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
            UpdateHeaderInfo(cost, progress);
            UpdateActions(iconNo);            
        }
    }

    /// <summary>
    /// イベントに応じた行動パネルを生成
    /// </summary>
    /// <param name="eventType"></param>
    public void CreateEventActions(EVENT_TYPE eventType) {
        actionList = new List<ActionInfo>();

        // 必ず「先を急ぐ(諦める)」パネルを表示する
        ActionInfo action = Instantiate(actionInfoPrefab, actionTran, false);
        action.questManager = this;
        //action.InitField(questList[0].fieldDatas[j], GameData.instance.actionDataList.actionDataList[Random.Range(0, GameData.instance.actionDataList.actionDataList.Count)]);

        action.InitAction(GameData.instance.actionDataList.actionDataList[6]);
        
        actionList.Add(action);

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
    /// 地形と行動を作成
    /// </summary>
    /// <param name="iconNo"></param>
    public void CreateField(int iconNo) {
        // Eventイメージ表示(現在いる地形、敵、ワナなどのイベントなど)
        //if (!imgEvent.gameObject.activeSelf) {
        //    imgEvent.gameObject.SetActive(true);
        //}
        //imgEvent.sprite = Resources.Load<Sprite>("Fields/" + iconNo);
        StartCoroutine(SetFieldImage(iconNo));


        scrollViewAction.SetActive(true);

        actionList = new List<ActionInfo>();

        // エリアごとに生成可能な地形の出現割合を合計
        int total = 0;
        for (int i = 0; i < questList[0].feildRates.Length; i++) {
            total += questList[0].feildRates[i];
        }

        // 移動可能な地形をランダムに生成する
        for (int i = 0; i < actionBaseCount; i++) {
            int value = Random.Range(0, total + 1);
            for (int j = 0; j < questList[0].feildRates.Length; j++) {
                if (value < questList[0].feildRates[j]) {
                    ActionInfo action = Instantiate(actionInfoPrefab, actionTran, false);
                    action.questManager = this;
                    action.InitField(questList[0].fieldDatas[j], GameData.instance.actionDataList.actionDataList[Random.Range(0, GameData.instance.actionDataList.actionDataList.Count)]);
                    actionList.Add(action);
                    break;
                } else {
                    value -= questList[0].feildRates[j];

                }
            }
        }

        // 実行可能な行動を作成

    }

    /// <summary>
    /// 今までの行動を破棄して新規に行動を作成
    /// </summary>
    public void UpdateActions(int iconNo, bool isCreate = true) {
        for (int i = 0; i < actionList.Count; i++) {
            Destroy(actionList[i].gameObject);
        }
        if (isCreate) {
            // 移動以外の場合は始めはfalseなので移動先は生成しない
            CreateField(iconNo);
        }
    }

    /// <summary>
    /// １つの行動を選択したら、他の行動をタップできないように制御
    /// </summary>
    public void InactieActionInfo() {
        for (int i = 0; i < actionList.Count; i++) {
            actionList[i].btnActionInfo.interactable = false;
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
