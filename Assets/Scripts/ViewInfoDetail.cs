using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ViewInfoの親クラス
/// </summary>
public class ViewInfoDetail : MonoBehaviour
{
    public int viewNo;

    public CanvasGroup canvasGroup;

    protected virtual void Start()　 {        
    }

    /// <summary>
    /// SkillViewInfoを設定
    /// </summary>
    public virtual void SetupSkillView(PlayFabManager.SkillData skillData) {      
    }

    /// <summary>
    /// MemoriaStatusViewInfoを設定
    /// </summary>
    public virtual void SetupMemoriaStatusView(PlayFabManager.MemoriaData memoriaData, int memoriaViewNo) {
    }

    /// <summary>
    /// ViewInfoの表示/非表示の切り替え
    /// </summary>
    /// <param name="alpha">0f or 1.0f</param>
    public void SwitchActiveView(float alpha) {
        canvasGroup.alpha = alpha;
    }
}
