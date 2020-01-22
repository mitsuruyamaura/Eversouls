using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ActionInfo : MonoBehaviour
{
    [Header("移動/行動するためのコスト")]
    public int cost;
    [Header("成功時に獲得できる進捗度")]
    public float progress;
    [Header("クリティカル確率")]
    public float critical;

    // UI系
    [Header("地形/行動名")]
    public TMP_Text txtActionName;
    [Header("インフォ")]
    public TMP_Text txtActionInfo;
    [Header("コスト表示用")]
    public TMP_Text txtCost;
    [Header("キャラスキルの残り回数表示")]
    public TMP_Text txtAmountCount;
    [Header("キャラのイメージ")]
    public Image imgCharaIcon;
    [Header("移動/行動のイメージ")]
    public Image imgMainAction;
    [Header("移動/行動の対象となる地形のイメージ")]
    public Image imgField;
    [Header("ボタン用")]
    public Button btnActionInfo;

    // 設定系
    public FIELD_TYPE fieldType;
    public ACTION_TYPE actionType;    
    public QuestManager questManager;

    [Header("重複タップ防止フラグ")]
    public bool isSubmit;
    [Header("行為判定が必要か不要かのフラグ")]
    private bool isAction = false;
    [Header("地形/行動の表示用番号")]
    private int imageNo;

    private void Start() {
        if (cost > GameData.instance.ap) {
            btnActionInfo.interactable = false;
        } else {
            btnActionInfo.onClick.AddListener(OnClickCheckMove);
        }
    }

    public void InitField(FieldDataList.FieldData fieldData, ActionDataList.ActionData actionData) {
        // 行動を設定
        fieldType = fieldData.fieldType;
        cost = fieldData.cost + actionData.cost;
        progress = fieldData.progress;
        critical = fieldData.criticalRate + actionData.criticalRate;

        actionType = actionData.actionType;

        // 行動のイメージ設定
        imgMainAction.sprite = Resources.Load<Sprite>("Actions/" + (int)actionData.actionType);

        // 行動対象となる地形のイメージ設定
        imgField.sprite = Resources.Load<Sprite>("Fields/" + fieldData.imageNo);
        imageNo = fieldData.imageNo;

        // テキスト関連表示
        txtActionName.text = fieldType.ToString();
        txtActionInfo.text = fieldData.info;
        txtCost.text = fieldData.cost.ToString();
    }

    public void InitAction(ActionDataList.ActionData data) {
        btnActionInfo.onClick.AddListener(OnClickCheckChooseAction);
        isAction = true;

        // 行動を設定
        actionType = data.actionType;
        cost = data.cost;
        progress = data.progress;
        imgMainAction.sprite = Resources.Load<Sprite>("Actions/" + data.imageNo);
        imageNo = data.imageNo;

        //imgCharaIcon.gameObject.SetActive(true);
        //imgCharaIcon.sprite = Resources.Load<Sprite>("Charas/" + GameData.instance.charas[0].imageNo);

        txtActionName.text = actionType.ToString();
        txtActionInfo.text = data.info;
        txtCost.text = data.cost.ToString();

        //txtAmountCount.text
    }

    /// <summary>
    /// 移動時のイベント発生を確認
    /// </summary>
    public void OnClickCheckMove() {
        if (!isSubmit) {
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
            isSubmit = true;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(1.2f, 0.2f));
            seq.Append(transform.DOScale(1.0f, 0.2f));
            questManager.InactieActionInfo();
            StartCoroutine(questManager.MoveJudgment(cost, progress, imageNo, critical, fieldType, actionType));
        }
    }

    /// <summary>
    /// イベント時の行動の成否チェック TODO　使わないかも
    /// </summary>
    public void OnClickCheckChooseAction() {
        if (!isSubmit) {
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
            isSubmit = true;
            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(1.2f, 0.2f));
            seq.Append(transform.DOScale(1.0f, 0.2f));
            questManager.InactieActionInfo();
                       
            StartCoroutine(questManager.ActionJudgment(cost, progress, imageNo, actionType));
        }
    }
}
