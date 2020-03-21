﻿using System.Collections;
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
    [Header("Jsonファイル読み込み用")]
    public LoadMasterDataFromJson loadMasterDataFrom;

    private bool isClickable;     // 重複タップ防止用
    [Header("PlayFab読み込みON")]
    public bool isPlayFabOn;

    void Start() {
        isClickable = true;

        if (isPlayFabOn) {
            PlayFabManager.instance.ConnectPlayfab();
        }

        btnStart.onClick.AddListener(OnClickStart);
        btnStart.interactable = false;
        
        // スタートテキストの点滅アニメ再生
        lblTapStart.DOFade(1f, 1.5f).SetLoops(-1, LoopType.Yoyo);

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
            btnStart.interactable = true;
        }    
    }
}
