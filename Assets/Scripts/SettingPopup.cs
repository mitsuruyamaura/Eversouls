using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingPopup : MonoBehaviour
{
    [Header("BGM音量調整用")]
    public Slider sliderBGM;
    [Header("SE音量調整用")]
    public Slider sliderSE;
    [Header("閉じるボタン")]
    public Button btnClose;

    public CanvasGroup canvasGroup;

    public void Setup() {
        canvasGroup.DOFade(1f, 0.5f);
        sliderBGM.value = GameData.instance.volumeBGM;
        sliderSE.value = GameData.instance.volumeSE;

        // Sliderにイベントを登録
        // UnityAction(float value)がデリゲードされているので引数不要
        sliderBGM.onValueChanged.AddListener(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().SetBGM);
        sliderSE.onValueChanged.AddListener(GameObject.FindGameObjectWithTag("SoundManager").GetComponent<SoundManager>().SetSE);

        btnClose.onClick.AddListener(OnClickClosePopup);
    }

    private void OnClickClosePopup() {
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        canvasGroup.DOFade(0f, 0.5f);
        Destroy(gameObject, 0.5f);
    }
}
