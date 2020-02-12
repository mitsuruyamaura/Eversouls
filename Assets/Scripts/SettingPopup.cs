using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingPopup : PopupBase
{
    [Header("BGM音量調整用")]
    public Slider sliderBGM;
    [Header("SE音量調整用")]
    public Slider sliderSE;

    private HomeManager _homeManager;

    public void Setup(HomeManager homeManager) {
        _homeManager = homeManager;

        // 現在の音量を取得してスライダーセット
        sliderBGM.value = GameData.instance.volumeBGM;
        sliderSE.value = GameData.instance.volumeSE;

        // Sliderにイベントを登録
        // UnityAction(float value)がデリゲードされているので引数不要
        sliderBGM.onValueChanged.AddListener(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().SetBGMVolume);
        sliderSE.onValueChanged.AddListener(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().SetSEVolume);   
    }

    public override void OnClickClosePopup() {
        //StartCoroutine(SaveSettings());
        // 再度設定ボタンを押せるようにする
        _homeManager.isSetting = false;

        base.OnClickClosePopup();
    }

    private IEnumerator SaveSettings() {
        yield return StartCoroutine(PlayFabManager.instance.UpdataUserDataInOptions());
        _homeManager.isSetting = false;

        base.OnClickClosePopup();
    }
}
