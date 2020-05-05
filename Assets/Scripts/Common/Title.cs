using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Title : MonoBehaviour {
    [Header("スタートボタン(画面全体)")]
    public Button btnStart;
    [Header("シェアボタン")]
    public Button btnShare;
    [Header("Start点滅アニメ用")]
    public Text txtTapStart;
    [Header("Start点滅アニメ用")]
    public Text txtVersion;

    //[Header("Jsonファイル読み込み用")]
    //public LoadMasterDataFromJson loadMasterDataFrom;

    public bool isClickable;     // 重複タップ防止用
    [Header("PlayFab読み込みON")]
    public bool isPlayFabOn;

    // 終了確認ポップアップ関連
    public ConfirmPopup confirmPopupPrefab;
    public Transform canvasTran;
    private ConfirmPopup confirmPopup;
    public bool isOpen;


    void Start() {
        isClickable = true;

        if (isPlayFabOn) {
            // PlayFabへ接続
            PlayFabManager.instance.ConnectPlayfab();
        }

        // ボタン設定
        btnStart.onClick.AddListener(OnClickStart);
        btnStart.interactable = false;
        btnShare.onClick.AddListener(() => StartCoroutine(OnClickShare()));

        // アプリバージョン表示
        txtVersion.text = "version " + Application.version.ToString();

        // スタートテキストの点滅アニメ再生
        txtTapStart.text = "Loading...";
        txtTapStart.DOFade(1f, 1.5f).SetLoops(-1, LoopType.Yoyo);

        isClickable = false;
    }

    /// <summary>
    /// ゲームスタート
    /// </summary>
    public void OnClickStart() {
        if (isClickable) {
            return;
        }
        isClickable = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);

        if (GameData.instance.isScriptableObjectLoad) {
            // ItemMasterData(スクリプタブル・オブジェクトで扱うマスターデータ用クラス)に
            // Jsonファイルのデータを読み込んで入れ込む
            //loadMasterDataFrom.LoadFromJson();
        }

        // トランジション処理してシーン遷移
        TransitionManager.instance.TransFadeOut(1.0f);
        StartCoroutine(SceneStateManager.instance.MoveScene(SCENE_TYPE.HOME, 1.0f));
    }

    void Update() {
        // Playfabのデータ取得が終わったら画面タップ許可
        if (GameData.instance.isGetPlayfabDatas && !btnStart.interactable) {
            // Loading表示をTapStartに変える
            UpDateDisplay();
        }

        if (isClickable) {
            return;
        }

        // アプリ終了確認ポップアップ
        if (Input.GetKeyDown(KeyCode.Escape) && confirmPopup != null) {
            CloseConfirmPopup();
        } else if (Input.GetKeyDown(KeyCode.Escape) && confirmPopup == null) {
            OpenConfirmPopup();
        }
    }

    /// <summary>
    /// Loading =>TapStartに表示変更
    /// </summary>
    private void UpDateDisplay() {
        txtTapStart.DOFade(0f, 1.0f);
        txtTapStart.text = "Tap Start";
        btnStart.interactable = true;
    }

    /// <summary>
    /// シェア
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnClickShare() {
        if (isClickable) {
            yield break;
        }
        isClickable = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        yield return StartCoroutine(ShareController.ShareScreenShot());
        isClickable = false;
    }

    /// <summary>
    /// 終了確認ポップアップを生成
    /// </summary>
    private void OpenConfirmPopup() {
        isClickable = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        confirmPopup = Instantiate(confirmPopupPrefab, canvasTran, false);
        isClickable = false;
    }

    /// <summary>
    /// 終了確認ポップアップを破棄
    /// </summary>
    private void CloseConfirmPopup() {
        isClickable = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        confirmPopup.OnClickClosePopup();
        isClickable = false;
    }
}
