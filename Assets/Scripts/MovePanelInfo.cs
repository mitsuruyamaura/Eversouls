using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System.Linq;

public class MovePanelInfo : MonoBehaviour
{
    // UI関連
    public Button btnSubmit;
    public TMP_Text lblFieldName;
    public TMP_Text txtCost;
    public TMP_Text txtFirstActionRate;
    public Image imgField;
    public Image imgEventIcon;
    public CanvasGroup canvasGroup;

    public QuestManager questManager;
    public FieldDataList.FieldData fieldData = new FieldDataList.FieldData();
    public LandscapeDataList.LandscapeData landscapeData = new LandscapeDataList.LandscapeData();

    private bool isClickable;      // 重複タップ防止フラグ(外部クラス経由でもこのクラス内で処理する場合はprivateでOK)
    public bool isSecretPlace = false;
    public int firstActionRate;    // 検出率。成功すると先制行動が可能
    bool isSearchEvent = false;    // 探索型イベントか確定イベントか

    public EVENT_TYPE eventType;

    // 配列かリストで選択可能なスキルを取得し、タップしたかどうかを監視する

    void Start() {
        btnSubmit.interactable = false;
        btnSubmit.onClick.AddListener(() => StartCoroutine(OnClickSubmit()));        
    }

    /// <summary>
    /// 移動パネル生成時、左から順番にアニメさせて生成
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public IEnumerator ChangePanelScale(float time) {
        yield return new WaitForSeconds(time);
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DORotate(new Vector3(0, -360, 0), 0.5f, RotateMode.FastBeyond360));
        seq.Join(transform.DOScale(1.0f, 0.5f));
        seq.Join(canvasGroup.DOFade(1.0f, 0.5f));

        // すべての移動パネルが揃ってからタップできるように時間調整
        yield return new WaitForSeconds((2.0f - time));
        btnSubmit.interactable = true;
    }

    /// <summary>
    /// FieldDataから移動パネルを生成
    /// イベントの種類に合わせたアイコン、地形の名前などを受け取りセットする
    /// </summary>
    /// <param name="fieldData"></param>
    public void InitMovePanel(FieldDataList.FieldData fieldData) {
        isClickable = true;

        this.fieldData = fieldData;

        lblFieldName.text = this.fieldData.fieldType.ToString();
        txtCost.text = fieldData.cost.ToString();
        imgField.sprite = GameData.instance.spriteAtlas.GetSprite("Field_" + this.fieldData.imageNo.ToString());//Resources.Load<Sprite>("Fields/" + this.fieldData.imageNo);

        // イベントアイコンをイベント出現率から抽出して設定
        int[] eventTypesRate = GetFieldEventRates(this.fieldData.events);
        
        // イベントタイプに応じた検出率(先制/発見)を表示
        int[] tempFirstActionRates = GetFieldEventRates(this.fieldData.firstActionRates);
       
        // 各フィールドの持つ確率で探索型にする
        if (Random.Range(0, 100) < fieldData.searchRate) {
            isSearchEvent = true;
        }

        int totalRate = 0;
        for (int i = 0; i < eventTypesRate.Length; i++) {
            totalRate += eventTypesRate[i];
        }
        int randomValue = Random.Range(0, totalRate + 1);
        for (int x = 0; x < eventTypesRate.Length; x++) {
            if (randomValue <= eventTypesRate[x]) {
                // 探索型の場合【?】表示にするのでそのまま。それ以外はイベントに応じて差し替え
                if (!isSearchEvent) {
                    Debug.Log(x);
                    imgEventIcon.sprite = GameData.instance.spriteAtlas.GetSprite("Event_" + x.ToString());//Resources.Load<Sprite>("Events/" + x);
                }
                eventType = (EVENT_TYPE)x;
                firstActionRate = tempFirstActionRates[x];
                txtFirstActionRate.text = firstActionRate.ToString();
                break;
            } else {
                randomValue -= eventTypesRate[x];
            }
        }
        isClickable = false;
    }

    /// <summary>
    /// 出口パネルと交霊の祠パネルを生成
    /// </summary>
    /// <param name="landscapeData"></param>
    public void InitSacredPlacePanel(LandscapeDataList.LandscapeData landscapeData, int eventNo) {
        isClickable = true;

        isSecretPlace = true;
        this.landscapeData = landscapeData;

        lblFieldName.text = this.landscapeData.landscapeType.ToString();
        txtCost.text = 0.ToString();
        imgField.sprite = GameData.instance.spriteAtlas.GetSprite("Landscape_" + this.fieldData.imageNo.ToString());//Resources.Load<Sprite>("Landscapes/" + this.fieldData.imageNo);
        imgEventIcon.sprite = GameData.instance.spriteAtlas.GetSprite("Event_" + eventNo.ToString());//Resources.Load<Sprite>("Events/" + eventNo);
        txtFirstActionRate.text = 0.ToString();

        isClickable = false;
    }

    /// <summary>
    /// ボスパネルを生成
    /// </summary>
    public void InitBossPanel(ENEMY_LEVEL_TYPE bossType, int bossNo) {
        isClickable = true;

        Debug.Log("ボス 生成 : " + bossType);

        lblFieldName.text = bossType.ToString();
        txtCost.text = 0.ToString();
        imgField.sprite = GameData.instance.spriteAtlas.GetSprite("Boss_" + bossNo.ToString());//Resources.Load<Sprite>("BossImages/" + bossNo);
        imgEventIcon.sprite = GameData.instance.spriteAtlas.GetSprite("Event_" + 0.ToString());//Resources.Load<Sprite>("Events/" + 0);

        // ボスに対しての先制行動の成功率
        firstActionRate = Random.Range(0, 40);
        txtFirstActionRate.text = firstActionRate.ToString();

        isClickable = false;
    }

    /// <summary>
    /// 移動パネルをアニメさせて非表示にする
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnClickSubmit() {
        if (isClickable) {
            yield break;
        }
        isClickable = true;

        // 移動回数を加算
        questManager.moveCount++;
        questManager.scrollViewMoveSkillCanvasGroup.DOFade(0, 0.5f);

        // 他の移動パネルは見た目もタップできないようにする
        for (int i = 0; i < questManager.moveList.Count; i++) {
            if (!questManager.moveList[i].isClickable) {
                questManager.moveList[i].isClickable = true;
                questManager.moveList[i].btnSubmit.interactable = false;
            }
        }

        // ファーストアクションが可能かどうかの判定
        float waitTime = 0f;
        bool isLucky = false;
        Sequence seq = DOTween.Sequence();
        if (Random.Range(0, 100) <= firstActionRate) {
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_LUKCY);
            // ファーストアクション可能
            seq.Append(transform.DOScale(1.2f, 0.5f));
            seq.Append(transform.DORotate(new Vector3(0, 720, 0), 1.0f, RotateMode.FastBeyond360));
            seq.Append(transform.DORotate(new Vector3(0, 360, 0), 0.5f, RotateMode.FastBeyond360));
            seq.Join(transform.DOScale(0, 0.5f));
            waitTime = 2.0f;
            isLucky = true;
        } else {
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
            // 通常
            seq.Append(transform.DOScale(1.2f, 0.3f));
            seq.Append(transform.DORotate(new Vector3(0, 360, 0), 0.75f, RotateMode.FastBeyond360));
            seq.Join(transform.DOScale(0, 0.75f));
            waitTime = 1.05f;
        }
        yield return new WaitForSeconds(waitTime);
        questManager.scrollViewMoveSkillCanvasGroup.gameObject.SetActive(false);

        if (isSecretPlace) {
            if (landscapeData.landscapeType == LANDSCAPE_TYPE.出口) {
                // 脱出処理へ
                StartCoroutine(questManager.ExitQuest());
            } else {
                // ゲーム続行し、スピリットの生成処理へ
                StartCoroutine(questManager.CreateSpiritualityPlace());
            }
        } else {
            // 移動後の処理
            StartCoroutine(questManager.MoveJudgment(fieldData, eventType, isLucky, isSearchEvent));
        }
    }

    // Updateでスキルのオンオフを監視
    // タップされたスキルのリストをゲッターメソッドに渡し、この移動パネルに該当する要素がある場合には
    // その要素の数値や内容を変更する


    /// <summary>
    /// エリアのイベントの出現割合を算出
    /// </summary>
    /// <param name="fieldData"></param>
    /// <returns></returns>
    public int[] GetFieldEventRates(string fieldEventData) {
        int[] rates = fieldEventData.Split(',').Select(int.Parse).ToArray();
        return rates;
    }
}
