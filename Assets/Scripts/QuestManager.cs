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

    [Header("探索しているクエストのリスト")]
    public List<QuestData> questList = new List<QuestData>();
    [Header("現在の行動リスト")]
    public List<ActionInfo> actionList = new List<ActionInfo>();

    [Header("クエスト用プレファブの生成位置")]
    public Transform questTran;
    [Header("行動用プレファブの生成位置")]
    public Transform actionTran;
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

    [Header("デバッグ表示用")]
    public TMP_Text txtDebug;

    public void Init() {
        txtDebug.text += "Init start\n";
        // スタートエリアを選択するためQuestDataオブジェクトをインスタンスする
        if (!GameData.instance.endTutorial) {
            // チュートリアルが終わっていなければ
            QuestData quest = Instantiate(questDataPrefab, questTran, false);
            quest.questManager = this;
            quest.InitQuestData(0);
            quest.no = 0;
            quest.clearCount = 10;           
            questList.Add(quest);
            txtDebug.text += "Init end\n";
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
    /// 行為判定
    /// </summary>
    /// <returns></returns>
    public IEnumerator ActionJudgment(int cost, float progress, bool isAction, int iconNo) {
        yield return new WaitForSeconds(0.5f);
        // 最初にクリティカルかどうか判定する

        // trueの場合、成功か失敗かを表示する

        // その後、進捗度を更新し、次の行動を作成

        UpdateHeaderInfo(cost, progress);
        UpDateActions(iconNo);
    }

    /// <summary>
    /// 移動開始前に敵が出現するかチェック
    /// </summary>
    public void CheckEnterEnemy() {

    }

    public void CheckTrap() {

    }

    public void CheckChest() {

    }

    /// <summary>
    /// 行動を作成
    /// </summary>
    /// <param name="iconNo"></param>
    public void CreateActions(int iconNo) {
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
                    action.InitField(questList[0].fieldDatas[j], GameData.instance.actionDataList.actionDataList[0]);
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
    public void UpDateActions(int iconNo) {
        for (int i = 0; i < actionList.Count; i++) {
            Destroy(actionList[i].gameObject);
        }
        CreateActions(iconNo);
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
