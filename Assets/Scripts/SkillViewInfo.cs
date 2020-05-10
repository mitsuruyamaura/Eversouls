using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// スキルの詳細説明表示用クラス
/// SkillDetailから生成される
/// </summary>
public class SkillViewInfo : ViewInfoDetail
{
    public Text txtSkillName;
    public Text txtSkillCost;
    public Text txtSkillExp;
    public Text txtInfo;
    public Text txtSkillLevel;
    public Text txtSkillSuccessRate;
    public Text txtType;

    /// <summary>
    /// SkillViewInfoを設定
    /// </summary>
    public override void SetupSkillView(PlayFabManager.SkillData skillData) {
        viewNo = skillData.skillNo;

        // UI設定
        txtSkillName.text = skillData.skillName;
        txtSkillCost.text = skillData.cost.ToString();
        txtSkillExp.text = skillData.expList[skillData.level].ToString();
        txtInfo.text = skillData.info;
        txtSkillLevel.text = skillData.level.ToString();
        txtSkillSuccessRate.text = skillData.successRate.ToString();
    }
}
