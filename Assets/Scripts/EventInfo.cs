using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

/// <summary>
/// イベントパネルを作成するクラス
/// </summary>
public class EventInfo : MonoBehaviour {
    // UI関連
    public Button btnSubmit;
    public Button btnField;
    public GameObject objEvent;
    public TMP_Text txtEventName;
    public TMP_Text txtSearchTargetName;
    public TMP_Text txtCurrentCount;
    public TMP_Text txtSucceseRate;
    public Image imgMain;
    public Image imgSearchTarget;
    public Image imgSearchTitle;
    public CanvasGroup canvasGroup;
    public CanvasGroup targetGroup;
    public CanvasGroup eventGroup;
    public CanvasGroup frameGroup;
    public Image imgHeader;
    public Image imgElementalType;   // 相性
    public Image imgAbilityType;     // 攻撃方法(武術、魔術、技術)

    public SEARCH_TARGET_TYPE searchType;

    // 地形ごとの対象リストと実際に使うデータ
    public PlayFabManager.EnemyData enemyData = new PlayFabManager.EnemyData();
    public SecretItemDataList.SecretItemData secretItemData = new SecretItemDataList.SecretItemData();
    public TrapDataList.TrapData trapData = new TrapDataList.TrapData();
    public LandscapeDataList.LandscapeData landscapeData = new LandscapeDataList.LandscapeData();

    [Header("最大探索回数")]
    public int maxCheckCount;
    private int currentCount;   // 現在の探索回数    
    private bool isClickable;   // 重複タップ防止用

    private int _cost;
    private float _progress;
    private int _fieldImageNo;
    private bool isClearEvent;
    private bool _isLucky;

    public float succeseRate = 50;
    public float encountRate = 20;
    QuestManager questManager;
    public EVENT_TYPE eventType;

    /// <summary>
    /// イベントの初期設定
    /// イベントタイプにより分岐
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    public void SetupEventInfo(EVENT_TYPE eventType, GameData.QuestData questData, FIELD_TYPE fieldType, int cost, float progress, int fieldImageNo, bool isLucky, QuestManager questManager, bool isSearchEvent) {
        isClickable = true;

        // イベントにかかわる値を取得      
        _cost = cost;
        _progress = progress;
        _fieldImageNo = fieldImageNo;
        _isLucky = isLucky;
        this.questManager = questManager;
        this.eventType = eventType;

        Sequence seq = DOTween.Sequence();
        seq.Append(gameObject.transform.DOScale(1.3f, 0.2f)).SetEase(Ease.Linear);
        seq.Append(gameObject.transform.DOScale(1.0f, 0.2f)).SetEase(Ease.Linear);

        Debug.Log(eventType);

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
        btnSubmit.onClick.AddListener(OnClickActionJudgment);
        btnSubmit.interactable = false;

        if (isSearchEvent) {
            // 探索イベント
            btnField.onClick.AddListener(() => StartCoroutine(CheckOpenEvent()));
            // 探索設定
            SetupSearchTarget(); 
        } else {
            // 確定しているイベント
            btnField.gameObject.SetActive(false);
            eventGroup.gameObject.SetActive(true);
            frameGroup.gameObject.SetActive(true);
            btnSubmit.interactable = true;
            Debug.Log("確定イベント");
        }
        // タップ可能
        isClickable = false;
    }

    /// <summary>
    /// 探索対象を設定し表示する
    /// </summary>
    private void SetupSearchTarget() {
        Debug.Log("Search");
        targetGroup.gameObject.SetActive(true);
        objEvent.SetActive(false);
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
            quest.DestroyMovePanelsAndEventPanels(_fieldImageNo);
            return;
        }
        // TODO 行動パネル作成
        
    }

    /// <summary>
    /// ランダム探索の成否チェック
    /// </summary>
    public IEnumerator CheckOpenEvent(int successRate = 50) {
        if (isClickable) {
            yield break;
        }
        isClickable = true;
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

    /// <summary>
    /// 探索成功
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
        eventGroup.gameObject.SetActive(true);
        frameGroup.gameObject.SetActive(true);
        btnField.gameObject.SetActive(false);
        imgSearchTitle.gameObject.SetActive(false);
        imgHeader.gameObject.SetActive(false);
        targetGroup.gameObject.SetActive(false);

        // TODO イベントの種類にあわせてHeaderを表示
        btnSubmit.interactable = true;
        // タップ可能
        isClickable = false;
    }

    /// <summary>
    /// 探索失敗
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
            isClickable = false;
        }
    }

    private void OnClickActionJudgment() {
        if (isClickable) {
            return;
        }
        isClickable = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);

        HideEventInfo();

        // イベントに対しての判定を行う
        //StartCoroutine(questManager.ActionJudgment());
    }

    /// <summary>
    /// 終了したイベントを隠す(破壊はQuestManagerで行う)
    /// </summary>
    public void HideEventInfo() {
        canvasGroup.DOFade(0, 0.5f);
    }

    /// <summary>
    /// 敵のイベントを作成
    /// </summary>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    private void CreateEnemy(GameData.QuestData questData, FIELD_TYPE fieldType) {
        // この地域に出現する敵の出現割合を合計する
        int total = 0;
        for (int i = 0; i < questData.enemyEncountRates.Length; i++) {
            total += questData.enemyEncountRates[i];
        }
        Debug.Log(total);

        List<PlayFabManager.EnemyData> areaEnemyList = new List<PlayFabManager.EnemyData>();

        // この地域に出現する敵のデータリストを作成                   
        foreach (PlayFabManager.EnemyData enemyData in PlayFabManager.instance.enemyDataList) {
            for (int i = 0; i < enemyData.habitats.Length; i++) {
                if (enemyData.habitats[i] == fieldType) {
                    areaEnemyList.Add(enemyData);
                }
            }
        }
        Debug.Log(areaEnemyList.Count);

        // 重み付けしたレアリティの中からどのレアリティかを決定する
        int value = Random.Range(0, total + 1);
        Debug.Log(value);

        // まずレアリティごとに分けたリストを作る
        for (int x = 0; x < questData.enemyEncountRates.Length; x++) {
            if (value <= questData.enemyEncountRates[x]) {
                // 決定したレアリティ内から敵リストを作成し、出現する敵の重み付けを合計
                int appears = 0;
                List<PlayFabManager.EnemyData> enterEnemyDataList = new List<PlayFabManager.EnemyData>();
                for (int y = 0; y < areaEnemyList.Count; y++) {
                    Debug.Log(areaEnemyList[y].rarelity);
                    Debug.Log((RARE_TYPE)x);
                    if (areaEnemyList[y].rarelity == (RARE_TYPE)x) {
                        appears += areaEnemyList[y].appearance;
                        enterEnemyDataList.Add(areaEnemyList[y]);
                    }
                }
                Debug.Log(enterEnemyDataList.Count);
                Debug.Log(appears);

                // 作成したレアリティ別リスト内から敵をランダムに決定
                int randomAppear = Random.Range(0, appears + 1);
                Debug.Log(randomAppear);
                for (int count = 0; count < enterEnemyDataList.Count; count++) {
                    if (randomAppear <= enterEnemyDataList[count].appearance) {
                        // 敵のデータを登録
                        enemyData = enterEnemyDataList[count];

                        // 各データを入れ込む
                        enemyData.no = enterEnemyDataList[count].no;
                        enemyData.name = enterEnemyDataList[count].name;
                        txtEventName.text = enemyData.name;

                        imgMain.sprite = Resources.Load<Sprite>("Enemys/" + enemyData.no);

                        // TODO level補正
                        enemyData.level = GameData.instance.level;
                        for (int i = 0; i < enemyData.level; i++) {
                            // ランダムに選択された能力値をレベル分上昇させる
                            int bonusNum = Random.Range(0, enemyData.levelBonus.Length);
                            enemyData.skillAbilities[bonusNum] += enemyData.levelBonus[bonusNum];
                            if (i % 3 == 0) {
                                // Hpは3levelごとにその時に選択された能力値分上がる
                                enemyData.hp += enemyData.levelBonus[bonusNum];
                                Debug.Log(i);
                            }
                        }

                        // 能力値の合計値を攻撃方法の選択目標値にする(高い能力値ほど選択されやすくする)
                        int totalAttackWeight = 0;
                        for (int num = 0; num < enemyData.skillAbilities.Length; num++) {
                            totalAttackWeight += enemyData.skillAbilities[num];
                        }

                        // 重みづけした合計値から攻撃方法をランダムで選び、成功率を入れ込む
                        int attackRate = Random.Range(0, totalAttackWeight + 1);
                        for (int attackNo = 0; attackNo < enemyData.skillAbilities.Length; attackNo++) {
                            if (attackRate <= enemyData.skillAbilities[attackNo]) {
                                // 攻撃方法のイメージを表示
                                enemyData.weaponType = (WEAPON_TYPE)attackNo; 
                                imgAbilityType.sprite = Resources.Load<Sprite>("WeaponTypes/" + enemyData.weaponType.ToString());

                                // 成功率を入れて、画面に表示
                                succeseRate = enemyData.skillAbilities[attackNo];
                                txtSucceseRate.text = succeseRate.ToString();

                                // 相性のイメージを表示(毎回ランダム)
                                ELEMENT_TYPE elementalType = (ELEMENT_TYPE)Random.Range(0, (int)ELEMENT_TYPE.COUNT);
                                enemyData.elementType = elementalType;
                                imgElementalType.sprite = Resources.Load<Sprite>("ElementalTypes/" + elementalType.ToString());
                                break;
                            } else {
                                attackRate -= enemyData.skillAbilities[attackNo];
                            }
                        }                       
                        break;
                    } else {
                        randomAppear -= enterEnemyDataList[count].appearance;
                        Debug.Log(randomAppear);
                    }
                }
                break;
            } else {
                value -= questData.enemyEncountRates[x];
                Debug.Log(value);
            }           
        }
    }

    /// <summary>
    /// 秘匿物のイベントを作成
    /// </summary>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    private void CreateSecretItem(GameData.QuestData questData, FIELD_TYPE fieldType) {
        // この地域に出現する秘匿物の出現割合を合計する
        int total = 0;
        for (int i = 0; i < questData.secretItemRates.Length; i++) {
            total += questData.secretItemRates[i];
        }
        Debug.Log(total);

        List<SecretItemDataList.SecretItemData> secretList = new List<SecretItemDataList.SecretItemData>();

        // 重み付けしたレアリティの中からどのレアリティが排出されたか決定
        int value = Random.Range(0, total + 1);
        for (int x =0; x < questData.secretItemRates.Length; x++) {
            if (value <= questData.secretItemRates[x]) {
                int appears = 0;
                List<SecretItemDataList.SecretItemData> enterSecretDataList = new List<SecretItemDataList.SecretItemData>();
                for (int y = 0; y < GameData.instance.secretItemDataList.secretItemDatas.Count; y++) {
                    if (GameData.instance.secretItemDataList.secretItemDatas[y].rarelity == (RARE_TYPE)x) {
                        appears += GameData.instance.secretItemDataList.secretItemDatas[y].appearance;
                        enterSecretDataList.Add(GameData.instance.secretItemDataList.secretItemDatas[y]);
                    }
                }

                // レアリティ内でリストを作成し、その中から１つを排出
                int randomAppear = Random.Range(0, appears + 1);
                Debug.Log(randomAppear);
                for (int count = 0; count < enterSecretDataList.Count; count++) {
                    if (randomAppear <= enterSecretDataList[count].appearance) {
                        secretItemData = enterSecretDataList[count];
                        secretItemData.no = enterSecretDataList[count].no;
                        secretItemData.secretItemType = enterSecretDataList[count].secretItemType;
                        txtEventName.text = secretItemData.secretItemType.ToString();

                        imgMain.sprite = Resources.Load<Sprite>("SecretItems/" + secretItemData.no);
                        break;
                    } else {
                        randomAppear -= enterSecretDataList[count].appearance;
                        Debug.Log(randomAppear);
                    }
                }
                break;
            } else {
                value -= questData.secretItemRates[x];
                Debug.Log(questData.secretItemRates[x]);
            }
        }
    }

    /// <summary>
    /// 罠のイベントを作成
    /// </summary>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    private void CreateTrap(GameData.QuestData questData, FIELD_TYPE fieldType) {
        // この地域に出現する罠の出現割合を合計する
        int total = 0;
        for (int i = 0; i < questData.trapRates.Length; i++) {
            total += questData.trapRates[i];
        }
        Debug.Log(total);

        List<TrapDataList.TrapData> trapList = new List<TrapDataList.TrapData>();

        // 重み付けしたレアリティの中からどのレアリティが排出されたか決定
        int value = Random.Range(0, total + 1);
        for (int x = 0; x < questData.trapRates.Length; x++) {
            if (value <= questData.trapRates[x]) {
                int appears = 0;
                List<TrapDataList.TrapData> enterTrapDataList = new List<TrapDataList.TrapData>();
                for (int y = 0; y < GameData.instance.trapDataList.trapDatas.Count; y++) {
                    if (GameData.instance.trapDataList.trapDatas[y].rarelity == (RARE_TYPE)x) {
                        appears += GameData.instance.trapDataList.trapDatas[y].appearance;
                        enterTrapDataList.Add(GameData.instance.trapDataList.trapDatas[y]);
                    }
                }

                // レアリティ内でリストを作成し、その中から１つを排出
                int randomAppear = Random.Range(0, appears + 1);
                Debug.Log(randomAppear);
                for (int count = 0; count < enterTrapDataList.Count; count++) {
                    if (randomAppear <= enterTrapDataList[count].appearance) {
                        trapData = enterTrapDataList[count];
                        trapData.trapType = enterTrapDataList[count].trapType;
                        txtEventName.text = trapData.trapType.ToString();

                        imgMain.sprite = Resources.Load<Sprite>("Traps/" + (int)trapData.trapType);
                        break;
                    } else {
                        randomAppear -= enterTrapDataList[count].appearance;
                        Debug.Log(randomAppear);
                    }
                }
                break;
            } else {
                value -= questData.trapRates[x];
                Debug.Log(questData.trapRates[x]);
            }
        }
    }

    /// <summary>
    /// 景勝地のイベントを作成
    /// </summary>
    /// <param name="questData"></param>
    /// <param name="fieldType"></param>
    private void CreateLandscape(GameData.QuestData questData, FIELD_TYPE fieldType) {
        // この地域に出現する罠の出現割合を合計する
        int total = 0;
        for (int i = 0; i < questData.landscapeRates.Length; i++) {
            total += questData.landscapeRates[i];
        }
        Debug.Log(total);

        List<LandscapeDataList.LandscapeData> landscapeList = new List<LandscapeDataList.LandscapeData>();

        // 重み付けしたレアリティの中からどのレアリティが排出されたか決定
        int value = Random.Range(0, total + 1);
        for (int x = 0; x < questData.landscapeRates.Length; x++) {
            if (value <= questData.landscapeRates[x]) {
                int appears = 0;
                List<LandscapeDataList.LandscapeData> enterLandscapeDataList = new List<LandscapeDataList.LandscapeData>();
                for (int y = 0; y < GameData.instance.landscapeDataList.landscapeDatas.Count; y++) {
                    if (GameData.instance.landscapeDataList.landscapeDatas[y].rarelity == (RARE_TYPE)x) {
                        appears += GameData.instance.landscapeDataList.landscapeDatas[y].appearance;
                        enterLandscapeDataList.Add(GameData.instance.landscapeDataList.landscapeDatas[y]);
                    }
                }

                // レアリティ内でリストを作成し、その中から１つを排出
                int randomAppear = Random.Range(0, appears + 1);
                Debug.Log(randomAppear);
                for (int count = 0; count < enterLandscapeDataList.Count; count++) {
                    if (randomAppear <= enterLandscapeDataList[count].appearance) {
                        landscapeData = enterLandscapeDataList[count];
                        landscapeData.landscapeType = enterLandscapeDataList[count].landscapeType;
                        txtEventName.text = landscapeData.landscapeType.ToString();

                        imgMain.sprite = Resources.Load<Sprite>("Landscapes/" + (int)landscapeData.landscapeType);
                        break;
                    } else {
                        randomAppear -= enterLandscapeDataList[count].appearance;
                        Debug.Log(randomAppear);
                    }
                }
                break;
            } else {
                value -= questData.landscapeRates[x];
                Debug.Log(questData.landscapeRates[x]);
            }
        }
    }
}
