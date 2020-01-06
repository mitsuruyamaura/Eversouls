using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HomeManager : MonoBehaviour
{
    [Header("クエスト開始ボタン")]
    public Button btnQuest;
    [Header("BGM#1再生ボタン")]
    public Button btnBgm1;
    [Header("BGM#2再生ボタン")]
    public Button btnBgm2;
    public Image imgHome;

    void Start(){
        SoundManager.Instance.PlayBGM(GameData.instance.homeBgmType);
        TransitionManager.instance.TransFadeIn(0.7f);
        //StartCoroutine(TransitionManager.instance.EnterScene());
        btnQuest.onClick.AddListener(() => StartCoroutine(OnClickQuestScene()));
        btnBgm1.onClick.AddListener(() => OnClickChangeBGM(SoundManager.ENUM_BGM.HOME_1));
        btnBgm2.onClick.AddListener(() => OnClickChangeBGM(SoundManager.ENUM_BGM.HOME_2));
        StartCoroutine(SetupHomeImage());
    }

    /// <summary>
    /// Questシーンへ遷移
    /// </summary>
    /// <returns></returns>
    public IEnumerator OnClickQuestScene() {
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        Sequence seq = DOTween.Sequence();
        seq.Append(btnQuest.transform.DOScale(1.2f, 0.15f));
        seq.Append(btnQuest.transform.DOScale(1.0f, 0.15f));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SceneStateManager.instance.MoveScene(SCENE_TYPE.QUEST));
    }

    /// <summary>
    /// HOMEシーンのメインキャラのアニメ表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetupHomeImage() {
        imgHome.transform.DOScale(1.5f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        imgHome.transform.DOScale(1.0f, 1.25f);
    }

    /// <summary>
    /// BGMの変更
    /// </summary>
    /// <param name="bgmType"></param>
    public void OnClickChangeBGM(SoundManager.ENUM_BGM bgmType) {
        SoundManager.Instance.PlayBGM(bgmType);
        GameData.instance.homeBgmType = bgmType;
    }
}
