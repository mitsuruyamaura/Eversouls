using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ConfirmPopup : PopupBase {

    public Button btnQuit;
    public TMP_Text txtTitle;

    protected override void Start() {
        base.Start();
        btnQuit.onClick.AddListener(() => OnClickQuit());
        txtTitle.text = "ゲームを終了しますか？";
    }

    /// <summary>
    /// ゲームを終了する
    /// </summary>
    public void OnClickQuit() {
        if (isSubmit) {
            return;
        }
        isSubmit = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        canvasGroup.DOFade(0f, 0.5f);
        Destroy(gameObject, 0.5f);

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }
}
