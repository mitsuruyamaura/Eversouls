using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EventInfo : MonoBehaviour
{
    // UI関連
    public Button btnSubmit;
    public Button btnField;
    public GameObject objEvent;
    public TMP_Text txtEventName;
    public TMP_Text txtSearchTargetName;
    public TMP_Text txtCurrentCount;
    public Image imgMain;
    public Image imgSearchTarget;
    public Image imgSearchTitle;
    public CanvasGroup canvasGroup;
    public Image imgHeader;

    public SEARCH_TARGET_TYPE searchType;

    // 地形ごとの対象リストと実際に使うデータ
    public List<EnemyDataList.EnemyData> enemyList = new List<EnemyDataList.EnemyData>();
    public EnemyDataList.EnemyData enemyData = new EnemyDataList.EnemyData();
    public List<SecretItemDataList.SecretItemData> secretList = new List<SecretItemDataList.SecretItemData>();
    public SecretItemDataList.SecretItemData secretItemData = new SecretItemDataList.SecretItemData();
    public List<TrapDataList.TrapData> trapList = new List<TrapDataList.TrapData>();
    public TrapDataList.TrapData trapData = new TrapDataList.TrapData();
    public List<LandscapeDataList.LandscapeData> landscapeList = new List<LandscapeDataList.LandscapeData>();
    public LandscapeDataList.LandscapeData landscapeData = new LandscapeDataList.LandscapeData();

    [Header("最大探索回数")]
    public int maxCheckCount;
    private int currentCount;   // 現在の探索回数    
    private bool isCheckable;   // 重複タップ防止用

    private int _cost;
    private float _progress;
    private int _iconNo;
    private bool isClearEvent;

    public float succeseRate = 50;
    public float encountRate = 20;


    /// <summary>
    /// イベントの初期設定
    /// イベントタイプにより分岐
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    public void Init(EVENT_TYPE eventType, QuestData questData, FIELD_TYPE fieldType, int cost, float progress, int iconNo) {
        // イベントにかかわる値を取得
        _cost = cost;
        _progress = progress;
        _iconNo = iconNo;

        Sequence seq = DOTween.Sequence();
        seq.Append(gameObject.transform.DOScale(1.3f, 0.2f)).SetEase(Ease.Linear);
        seq.Append(gameObject.transform.DOScale(1.0f, 0.2f)).SetEase(Ease.Linear);

        switch (eventType) {
            case EVENT_TYPE.敵:
                CreateEnemy(questData, fieldType);
                break;
            case EVENT_TYPE.秘匿物:
                CreateSecretItem(questData, fieldType);
                break;
            case EVENT_TYPE.罠:
                CreateTrap(questData, fieldType);
                break;
            case EVENT_TYPE.景勝地:
                CreateLandscape(questData, fieldType);
                break;
        }
        // 各ボタンの登録
        btnField.onClick.AddListener(() => StartCoroutine(CheckOpenEvent()));
        btnSubmit.onClick.AddListener(HideEventInfo);
        btnSubmit.interactable = false;

        // 探索対象の設定
        SetupSearchTarget();
    }

    /// <summary>
    /// 探索対象を設定し表示する
    /// </summary>
    private void SetupSearchTarget() {
        int value = Random.Range(0, (int)SEARCH_TARGET_TYPE.COUNT);
        searchType = (SEARCH_TARGET_TYPE)value;
        txtSearchTargetName.text = searchType.ToString();
        imgSearchTarget.sprite = Resources.Load<Sprite>("SearchTargets/" + value);
        currentCount = maxCheckCount;
        txtCurrentCount.text = currentCount.ToString();
    }

    /// <summary>
    /// イベントの種類に応じて選択可能な行動パネルを生成
    /// </summary>
    public void ChooseActions() {
        if (isClearEvent) {
            QuestManager quest = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
            quest.UpdateHeaderInfo(_cost, _progress);
            quest.UpdateActions(_iconNo);
            return;
        }
        // TODO 行動パネル作成
        
    }

    /// <summary>
    /// イベントの成否チェック
    /// </summary>
    public IEnumerator CheckOpenEvent(int successRate = 50) {
        if (!isCheckable) {
            isCheckable = true;
            currentCount--;
            txtCurrentCount.text = currentCount.ToString();

            // 判定待ち時間
            Sequence seq = DOTween.Sequence();
            seq.Append(objEvent.transform.DOScale(0.75f, 0.75f)).SetEase(Ease.Linear);
            seq.Join(btnField.gameObject.transform.DOScale(0.75f, 0.75f)).SetEase(Ease.Linear);
            //gameObject.transform.DOScale(0.75f, 0.75f);
            yield return new WaitForSeconds(0.75f);

            // TODO 成功判定を行う。
            // 今は50％で判定。クリティカルなし
            int value = Random.Range(0, 100);
            if (value < successRate) {
                // 成功した場合
                StartCoroutine(SuccessEvent());
            } else {
                // 失敗した場合               
                StartCoroutine(FailureEvent());
            }
        }
    }

    /// <summary>
    /// 成功
    /// </summary>
    /// <returns></returns>
    private IEnumerator SuccessEvent() {
        // 画像を拡大
        Sequence seq = DOTween.Sequence();
        seq.Append(objEvent.transform.DOScale(1.3f, 0.25f)).SetEase(Ease.Linear);
        seq.Join(btnField.gameObject.transform.DOScale(1.3f, 0.25f)).SetEase(Ease.Linear);
        yield return new WaitForSeconds(0.25f);

        // 画像を元の大きさに戻す
        seq.Append(objEvent.transform.DOScale(1.0f, 0.25f)).SetEase(Ease.Linear);
        seq.Join(btnField.gameObject.transform.DOScale(1.0f, 0.25f)).SetEase(Ease.Linear);

        // 下に隠れていたイベントを表示し、他のイメージを隠す
        objEvent.SetActive(true);
        btnField.gameObject.SetActive(false);
        imgSearchTitle.gameObject.SetActive(false);
        imgHeader.gameObject.SetActive(false);

        // TODO イベントの種類にあわせてHeaderを表示
        btnSubmit.interactable = true;
    }

    /// <summary>
    /// 失敗
    /// </summary>
    /// <returns></returns>
    private IEnumerator FailureEvent() {       
        if (currentCount <= 0) {
            // 探索最大回数を超えたらイベント終了
            // そのままイベントを縮小する
            gameObject.transform.DOScale(0f, 0.25f);
            yield return new WaitForSeconds(0.25f);
            HideEventInfo();
        } else {
            // 画像を元の大きさに戻す
            Sequence seq = DOTween.Sequence();
            seq.Append(objEvent.transform.DOScale(1.0f, 0.25f)).SetEase(Ease.Linear);
            seq.Join(btnField.gameObject.transform.DOScale(1.0f, 0.25f)).SetEase(Ease.Linear);
            yield return new WaitForSeconds(0.25f);
            // 再度タップできるようにする
            isCheckable = false;
        }
    }

    /// <summary>
    /// イベントを隠す(破壊はQuestManagerで行う)
    /// </summary>
    public void HideEventInfo() {
        canvasGroup.DOFade(0, 0.5f);
    }

    /// <summary>
    /// 敵のイベントを作成
    /// </summary>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    private void CreateEnemy(QuestData questData, FIELD_TYPE fieldType) {
        // この地域に出現する敵の出現割合を合計する
        int total = 0;
        for (int i = 0; i < questData.checkEventData.enemyEncountRates.Length; i++) {
            total += questData.checkEventData.enemyEncountRates[i];
        }
        Debug.Log(total);

        // この地域に出現する敵のデータリストを作成                   
        foreach (EnemyDataList.EnemyData enemy in GameData.instance.enemyDataList.enemyDatas) {
            for (int i = 0; i < enemy.habitats.Length; i++) {
                if (enemy.habitats[i] == fieldType) {
                    enemyList.Add(enemy);
                }
            }
        }

        // 重み付けしたレアリティの中からどのレアリティかを決定
        int value = Random.Range(0, total + 1);
        Debug.Log(value);
        for (int x = 0; x < questData.checkEventData.enemyEncountRates.Length; x++) {
            if (value <= questData.checkEventData.enemyEncountRates[x]) {
                // 決定したレアリティ内から敵リストを作成し、出現する敵の重み付けを合計
                int appears = 0;
                List<EnemyDataList.EnemyData> enterEnemyList = new List<EnemyDataList.EnemyData>();
                for (int y = 0; y < enemyList.Count; y++) {
                    Debug.Log(enemyList[y].rarelity);
                    Debug.Log((RARE_TYPE)x);
                    if (enemyList[y].rarelity == (RARE_TYPE)x) {
                        appears += enemyList[y].appearance;
                        enterEnemyList.Add(enemyList[y]);
                    }
                }
                Debug.Log(enterEnemyList.Count);
                Debug.Log(appears);

                // 作成したリスト内から敵をランダムに決定
                int randomAppear = Random.Range(0, appears + 1);
                Debug.Log(randomAppear);
                for (int count = 0; count < enterEnemyList.Count; count++) {
                    if (randomAppear <= enterEnemyList[count].appearance) {
                        // 敵のデータを入れ込む
                        enemyData.no = enterEnemyList[count].no;
                        enemyData.name = enterEnemyList[count].name;
                        txtEventName.text = enemyData.name;

                        imgMain.sprite = Resources.Load<Sprite>("Enemys/" + enemyData.no);

                        break;
                    } else {
                        randomAppear -= enterEnemyList[count].appearance;
                        Debug.Log(randomAppear);
                    }
                }
                break;
            } else {
                value -= questData.checkEventData.enemyEncountRates[x];
                Debug.Log(value);
            }           
        }
    }

    /// <summary>
    /// 秘匿物のイベントを作成
    /// </summary>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    private void CreateSecretItem(QuestData questData, FIELD_TYPE fieldType) {
        // この地域に出現する秘匿物の出現割合を合計する
        int total = 0;
        for (int i = 0; i < questData.checkEventData.secretItemRates.Length; i++) {
            total += questData.checkEventData.secretItemRates[i];
        }
        Debug.Log(total);

        // 重み付けしたレアリティの中からどのレアリティが排出されたか決定
        int value = Random.Range(0, total + 1);
        for (int x =0; x < questData.checkEventData.secretItemRates.Length; x++) {
            if (value <= questData.checkEventData.secretItemRates[x]) {
                int appears = 0;
                List<SecretItemDataList.SecretItemData> enterSecretList = new List<SecretItemDataList.SecretItemData>();
                for (int y = 0; y < GameData.instance.secretItemDataList.secretItemDatas.Count; y++) {
                    if (GameData.instance.secretItemDataList.secretItemDatas[y].rarelity == (RARE_TYPE)x) {
                        appears += GameData.instance.secretItemDataList.secretItemDatas[y].appearance;
                        enterSecretList.Add(GameData.instance.secretItemDataList.secretItemDatas[y]);
                    }
                }

                // レアリティ内でリストを作成し、その中から１つを排出
                int randomAppear = Random.Range(0, appears + 1);
                Debug.Log(randomAppear);
                for (int count = 0; count < enterSecretList.Count; count++) {
                    if (randomAppear <= enterSecretList[count].appearance) {
                        secretItemData.no = enterSecretList[count].no;
                        secretItemData.secretItemType = enterSecretList[count].secretItemType;
                        txtEventName.text = secretItemData.secretItemType.ToString();

                        imgMain.sprite = Resources.Load<Sprite>("SecretItems/" + secretItemData.no);
                        break;
                    } else {
                        randomAppear -= enterSecretList[count].appearance;
                        Debug.Log(randomAppear);
                    }
                }
                break;
            } else {
                value -= questData.checkEventData.secretItemRates[x];
                Debug.Log(questData.checkEventData.secretItemRates[x]);
            }
        }
    }

    /// <summary>
    /// 罠のイベントを作成
    /// </summary>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    private void CreateTrap(QuestData questData, FIELD_TYPE fieldType) {
        // この地域に出現する罠の出現割合を合計する
        int total = 0;
        for (int i = 0; i < questData.checkEventData.trapRates.Length; i++) {
            total += questData.checkEventData.trapRates[i];
        }
        Debug.Log(total);

        // 重み付けしたレアリティの中からどのレアリティが排出されたか決定
        int value = Random.Range(0, total + 1);
        for (int x = 0; x < questData.checkEventData.trapRates.Length; x++) {
            if (value <= questData.checkEventData.trapRates[x]) {
                int appears = 0;
                List<TrapDataList.TrapData> enterTrapList = new List<TrapDataList.TrapData>();
                for (int y = 0; y < GameData.instance.trapDataList.trapDatas.Count; y++) {
                    if (GameData.instance.trapDataList.trapDatas[y].rarelity == (RARE_TYPE)x) {
                        appears += GameData.instance.trapDataList.trapDatas[y].appearance;
                        enterTrapList.Add(GameData.instance.trapDataList.trapDatas[y]);
                    }
                }

                // レアリティ内でリストを作成し、その中から１つを排出
                int randomAppear = Random.Range(0, appears + 1);
                Debug.Log(randomAppear);
                for (int count = 0; count < enterTrapList.Count; count++) {
                    if (randomAppear <= enterTrapList[count].appearance) {
                        trapData.trapType = enterTrapList[count].trapType;
                        txtEventName.text = trapData.trapType.ToString();

                        imgMain.sprite = Resources.Load<Sprite>("Traps/" + (int)trapData.trapType);
                        break;
                    } else {
                        randomAppear -= enterTrapList[count].appearance;
                        Debug.Log(randomAppear);
                    }
                }
                break;
            } else {
                value -= questData.checkEventData.trapRates[x];
                Debug.Log(questData.checkEventData.trapRates[x]);
            }
        }
    }

    /// <summary>
    /// 景勝地のイベントを作成
    /// </summary>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    private void CreateLandscape(QuestData questData, FIELD_TYPE fieldType) {
        // この地域に出現する罠の出現割合を合計する
        int total = 0;
        for (int i = 0; i < questData.checkEventData.landscapeRates.Length; i++) {
            total += questData.checkEventData.landscapeRates[i];
        }
        Debug.Log(total);

        // 重み付けしたレアリティの中からどのレアリティが排出されたか決定
        int value = Random.Range(0, total + 1);
        for (int x = 0; x < questData.checkEventData.landscapeRates.Length; x++) {
            if (value <= questData.checkEventData.landscapeRates[x]) {
                int appears = 0;
                List<LandscapeDataList.LandscapeData> enterLandscapeList = new List<LandscapeDataList.LandscapeData>();
                for (int y = 0; y < GameData.instance.landscapeDataList.landscapeDatas.Count; y++) {
                    if (GameData.instance.landscapeDataList.landscapeDatas[y].rarelity == (RARE_TYPE)x) {
                        appears += GameData.instance.landscapeDataList.landscapeDatas[y].appearance;
                        enterLandscapeList.Add(GameData.instance.landscapeDataList.landscapeDatas[y]);
                    }
                }

                // レアリティ内でリストを作成し、その中から１つを排出
                int randomAppear = Random.Range(0, appears + 1);
                Debug.Log(randomAppear);
                for (int count = 0; count < enterLandscapeList.Count; count++) {
                    if (randomAppear <= enterLandscapeList[count].appearance) {
                        landscapeData.landscapeType = enterLandscapeList[count].landscapeType;
                        txtEventName.text = landscapeData.landscapeType.ToString();

                        imgMain.sprite = Resources.Load<Sprite>("Landscapes/" + (int)landscapeData.landscapeType);
                        break;
                    } else {
                        randomAppear -= enterLandscapeList[count].appearance;
                        Debug.Log(randomAppear);
                    }
                }
                break;
            } else {
                value -= questData.checkEventData.landscapeRates[x];
                Debug.Log(questData.checkEventData.landscapeRates[x]);
            }
        }
    }
}
