using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupBase : MonoBehaviour
{
    [Header("閉じるボタン")]
    public Button btnClose;
    [Header("枠外タップ時のボタン")]
    public Button btnFilter;

    public CanvasGroup canvasGroup;

    protected virtual void Start() {
        // フェイドイン処理
        canvasGroup.DOFade(1f, 0.5f);

        // ボタン設定
        btnClose.onClick.AddListener(OnClickClosePopup);
        btnFilter.onClick.AddListener(OnClickClosePopup);
    }

    /// <summary>
    /// ポップアップを閉じる
    /// </summary>
    public virtual void OnClickClosePopup() {
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        canvasGroup.DOFade(0f, 0.5f);      
        Destroy(gameObject, 0.5f);
    }
}
