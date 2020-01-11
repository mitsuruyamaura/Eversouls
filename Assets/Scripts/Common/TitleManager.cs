using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class TitleManager : MonoBehaviour
{
    [Header("スタートボタン(画面全体)")]
    public Button btnStart;
    [Header("Start点滅アニメ用")]
    public TMP_Text lblTapStart;

    private bool isEnterTitle;     // 重複タップ防止用

    void Start() {
        SoundManager.Instance.PlayBGM(SoundManager.ENUM_BGM.TITLE);
        btnStart.onClick.AddListener(OnClickStart);
        isEnterTitle = true;
        // スタートテキストの点滅アニメ再生
        lblTapStart.DOFade(1f, 1.5f).SetLoops(-1, LoopType.Yoyo);
    }

    /// <summary>
    /// ゲームスタート
    /// </summary>
    public void OnClickStart() {
        if (isEnterTitle) {
            isEnterTitle = false;
            SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);

            // トランジション処理してシーン遷移
            TransitionManager.instance.TransFadeOut(1.0f);
            StartCoroutine(SceneStateManager.instance.MoveHome(SCENE_TYPE.HOME));
        }
    }
}
