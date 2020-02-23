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
    public Image imgField;
    public Image imgEventIcon;
    public CanvasGroup canvasGroup;

    public QuestManager questManager;
    public FieldDataList.FieldData fieldData = new FieldDataList.FieldData();

    public bool isSubmit = false;  // 重複タップ防止フラグ

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
        this.fieldData = fieldData;

        lblFieldName.text = this.fieldData.fieldType.ToString();
        txtCost.text = fieldData.cost.ToString();
        imgField.sprite = Resources.Load<Sprite>("Fields/" + this.fieldData.imageNo);

        // イベントアイコンをイベント出現率から抽出して設定
        int[] eventTypesRate = new int[(int)EVENT_TYPE.COUNT];
        eventTypesRate = GetFieldRates(this.fieldData.events);
        int totalRate = 0;
        for (int i = 0; i < eventTypesRate.Length; i++) {
            totalRate += eventTypesRate[i];
        }
        int randomValue = Random.Range(0, totalRate + 1);
        for (int x = 0; x < eventTypesRate.Length; x++) {
            if (randomValue <= eventTypesRate[x]) {
                imgEventIcon.sprite = Resources.Load<Sprite>("Events/" + x);
                break;
            } else {
                randomValue -= eventTypesRate[x];
            }
        }
    }

    /// <summary>
    /// 移動パネルをアニメさせて非表示にする
    /// </summary>
    /// <returns></returns>
    private IEnumerator OnClickSubmit() {
        if (!isSubmit) {
            isSubmit = true;
                
            // 他の移動パネルは見た目もタップできないようにする
            for (int i = 0; i < questManager.moveList.Count; i++) {
                if (!questManager.moveList[i].isSubmit) {
                    questManager.moveList[i].isSubmit = true;
                    questManager.moveList[i].btnSubmit.interactable = false;
                }
            }

            // 抽選
            float waitTime = 0f;
            bool isLucky = false;
            Sequence seq = DOTween.Sequence();
            if (Random.Range(0, 100) <= 50) {
                SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_LUKCY);
                // 当たり
                seq.Append(transform.DOScale(1.5f, 0.5f));
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

            // 移動後の処理
            StartCoroutine(questManager.MoveJudgment(fieldData.cost, fieldData.progress, fieldData.imageNo, fieldData.criticalRate, fieldData.fieldType, ACTION_TYPE.通常移動, isLucky));

            // Debug用 TODO メソッドにLucky引数をつけて分岐を消す？
            //if (isLucky) {
            //    questManager.UpdateMoveInfo(fieldData.imageNo);
            //} else {
            //    questManager.UpdateMoveInfo(fieldData.imageNo);
            //}
        }
    }

    // Updateでスキルのオンオフを監視
    // タップされたスキルのリストをゲッターメソッドに渡し、この移動パネルに該当する要素がある場合には
    // その要素の数値や内容を変更する


    /// <summary>
    /// エリアの地形の出現割合を算出
    /// </summary>
    /// <param name="fieldData"></param>
    /// <returns></returns>
    public int[] GetFieldRates(string fieldEventData) {
        int[] rates = fieldEventData.Split(',').Select(int.Parse).ToArray();
        return rates;
    }
}
