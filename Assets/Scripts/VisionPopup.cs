using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class VisionPopup : PopupBase
{
    public Button btnVision;
    public Button btnReword;
    public TMP_Text txtSubtractCurrencyPoint;
    public TMP_Text txtRewordCurrencyPoint;

    protected override void Start() {
        base.Start();

        // ボタン設定
        btnVision.onClick.AddListener(()=> StartCoroutine(OnClickVision()));
        btnReword.onClick.AddListener(()=> StartCoroutine(OnClickAd()));
        // カレンシー表示設定
        txtSubtractCurrencyPoint.text = PlayFabManager.instance.subtractCurrencyPoint.ToString();
        txtRewordCurrencyPoint.text = PlayFabManager.instance.rewordCurrencyPoint.ToString();

        // 動画を視聴済で待機時間経過していないなら押せないようにする
        if (GameData.instance.rewordOn) {
            btnReword.interactable = false;
        }
    }

    /// <summary>
    /// メモリアを排出ボタン
    /// </summary>
    private IEnumerator OnClickVision() {
        if (!isSubmit) {
            isSubmit = true;
            Debug.Log("メモリア 排出");

            // カレンシーを減算
            GameData.instance.currency -= PlayFabManager.instance.subtractCurrencyPoint;
            yield return new WaitForSeconds(1.0f);

            // Debug用
            GameData.instance.rewordOn = false;

            yield return StartCoroutine(PlayFabManager.instance.UpdataUserReword());
            isSubmit = false;
        }
    }

    /// <summary>
    /// 広告視聴ボタン
    /// </summary>
    private IEnumerator OnClickAd() {
        if (!isSubmit) {
            isSubmit = true;
            // 動画リワードを再生し、カレンシーを付与。待機時間を設定。
            Debug.Log("動画再生");

            // カレンシーを加算
            GameData.instance.currency += PlayFabManager.instance.rewordCurrencyPoint;

            // Playfabの更新を入れて、待機時間中であることを保存する(あるいは時間を保存)
            GameData.instance.rewordOn = true;

            yield return StartCoroutine(PlayFabManager.instance.UpdataUserReword());
            isSubmit = false;
        }
    }
}
