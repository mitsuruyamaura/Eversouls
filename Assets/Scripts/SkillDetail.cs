using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;

/// <summary>
/// SkillDetaを持つアイコンボタンを管理するクラス
/// </summary>
public class SkillDetail : MonoBehaviour
{
    // UI関連
    public Text txtSkillName;
    public Image imgSkillIcon;
    public Button btnSkill;

    [Header("SkillViewInfoのプレファブ")]
    public SkillViewInfo skillViewInfoPrefab;

    [Header("SkillDetailで使用するSkillData")]
    public PlayFabManager.SkillData skillData;
    QuestSelectPopup questSelectPopup;

    public bool isClickable;  // 重複タップ防止
   
    /// <summary>
    /// SkillDataを取得して設定(画面上のスキルアイコン１つ分)
    /// </summary>
    public void Setup(int skillNo, QuestSelectPopup questSelectPopup) {
        // SkillDataを取得(Listにして取得するならWhere(FindAll)、今回は１つでいいのでFind)
        skillData = PlayFabManager.instance.skillDataList.Find(x => x.skillNo == skillNo);
        this.questSelectPopup = questSelectPopup;

        txtSkillName.text = skillData.skillName;
        imgSkillIcon.sprite = GameData.instance.spriteAtlas.GetSprite("Skill_" + skillData.imageNo);

        btnSkill.onClick.AddListener(OnClickDetail);
    }

    /// <summary>
    /// 他のViewInfoを非表示にして、SkillViewInfoを表示する
    /// SkillViewInfoを生成されていない場合は一度だけ生成する
    /// </summary>
    private void OnClickDetail() {
        if (isClickable) {
            return;
        }
        isClickable = true;

        // ボタンをアニメさせる
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(0.75f, 0.25f));
        seq.Append(transform.DOScale(1.0f, 0.25f));

        // すべてのViewInfoを一旦非表示にする
        for (int i = 0; i < questSelectPopup.viewInfoList.Count; i++) {
            questSelectPopup.viewInfoList[i].SwitchActiveView(0f);
        }

        if (questSelectPopup.viewInfoList.Exists(x => x.viewNo == skillData.skillNo)) {
            // すでにリストにViewDetailがある場合には、生成せずにそれを再表示する。
            foreach (ViewInfoDetail viewInfo in questSelectPopup.viewInfoList.Where(x => x.viewNo == skillData.skillNo)) {
                // 再表示
                viewInfo.SwitchActiveView(1.0f);
                break;
            }
        } else {
            // リストにない場合にはSkillViewInfoを生成
            SkillViewInfo skillViewInfo = Instantiate(skillViewInfoPrefab, questSelectPopup.viewInfoTran, false);
            skillViewInfo.SetupSkillView(skillData);
            questSelectPopup.viewInfoList.Add(skillViewInfo);          
        }
        isClickable = false;
    }
}
