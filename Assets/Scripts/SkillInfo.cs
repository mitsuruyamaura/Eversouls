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
    [Header("ボタン・オンの色")]
    public Color btnOnColor;
    [Header("ボタン・オフの色")]
    public Color btnOffColor;

    public PlayFabManager.SkillData skillData;
    public QuestManager questManager;

    [Header("重複タップ防止フラグ")]
    public bool isSubmit;

    [Header("スキル適用状態フラグ")]
    public bool isActive;

    // レリックによる修正があり/なし
    private bool isRelicFix;

    /// <summary>
    /// スキルの情報でパネルを設定
    /// </summary>
    /// <param name="data"></param>
    public void InitSkillPanelInfo(PlayFabManager.SkillData data) {
        skillData = data;

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
        if (!isSubmit) {
            isSubmit = true;
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
            isActive = !isActive;
            if (isActive) {
                Debug.Log("スキル 使用");
                skillData.amountCount--;
                btnSkillInfo.image.color = btnOnColor;
            } else {
                Debug.Log("スキル 解除");
                skillData.amountCount++;
                btnSkillInfo.image.color = btnOffColor;
            }
            txtAmountCount.text = skillData.amountCount.ToString();
            isSubmit = false;
        }
    }

    /// <summary>
    /// 移動後にスキルの残り回数を確認して使用の有無を更新
    /// </summary>
    public void UpdateActiveSkill() {
        if (skillData.amountCount <= 0) {
            btnSkillInfo.interactable = false;
        }
    }
}
