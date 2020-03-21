using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SkillInfo : MonoBehaviour
{
    // UI系
    [Header("スキル名")]
    public TMP_Text txtSkillName;
    [Header("スキルの使用回数表示")]
    public TMP_Text txtAmountCount;
    [Header("スピリットのイメージ")]
    public Image imgCharaIcon;
    [Header("スキルのイメージ")]
    public Image imgMainAction;
    [Header("ボタン用")]
    public Button btnSkillInfo;
    [Header("スキル使用中のボタンの色")]
    public Color btnOnColor;
    [Header("使用可能スキルの未使用時のボタンの色")]
    public Color btnOffColor;

    public PlayFabManager.SkillData skillData;
    public QuestManager questManager;
    public EVENT_TYPE eventType;

    [Header("重複タップ防止フラグ")]
    public bool isClickable;

    [Header("スキル適用状態フラグ")]
    public bool isActive;

    // レリックによる修正があり/なし
    private bool isRelicFix;

    /// <summary>
    /// スキルの情報でパネルを設定
    /// </summary>
    /// <param name="data"></param>
    public void InitSkillPanelInfo(PlayFabManager.SkillData data, EVENT_TYPE eventType) {
        skillData = data;
        this.eventType = eventType;

        // 設定
        imgMainAction.sprite = Resources.Load<Sprite>("Actions/" + skillData.imageNo);
        txtSkillName.text = skillData.skillName;

        // スキルの使用回数を設定。使用回数0の場合はタップできないようにする
        txtAmountCount.text = skillData.amountCount.ToString();
        UpdateActiveSkill();

        btnSkillInfo.onClick.AddListener(() => OnClickChooseSkill());
    }

    /// <summary>
    /// スキルの選択／解除
    /// </summary>
    private void OnClickChooseSkill() {
        if (isClickable) {
            return;
        }
        isClickable = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);

        // アクティブ/非アクティブの切り替え
        isActive = !isActive;
        // ボタンの色をアクティブ/非アクティブで変更
        btnSkillInfo.image.color = isActive ? btnOnColor : btnOffColor;

        if (isActive) {
            Debug.Log("スキル 使用");
            skillData.amountCount--;
            // コストを減らす
            questManager.UpdateHeaderInfo(skillData.cost, 0);
        } else {
            Debug.Log("スキル 解除");
            skillData.amountCount++;
            // コストを戻す
            questManager.UpdateHeaderInfo(-skillData.cost, 0);
        }
        txtAmountCount.text = skillData.amountCount.ToString();

        // スキルリスト全体の使用可否をチェックしてボタンのアクティブ状態を更新
        questManager.UpdateSkillListByCost(eventType == EVENT_TYPE.移動 ? questManager.moveSkillsList : questManager.eventSkillsList);
        if (isActive) {
            // 使用している(アクティブ状態)スキルのみ、もう一度押すとキャンセルできるようにする            
            btnSkillInfo.interactable = true;
        }
        isClickable = false;
    }

    /// <summary>
    /// 移動後にスキルの残り回数を確認して使用の有無を更新
    /// </summary>
    public void UpdateActiveSkill() {
        btnSkillInfo.interactable = true;
        if (skillData.amountCount <= 0) {
            btnSkillInfo.interactable = false;
        }
    }
}
