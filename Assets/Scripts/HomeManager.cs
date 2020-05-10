using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditorInternal;

public class HomeManager : MonoBehaviour
{
    // UI
    [Header("エリアボタン")]
    public Button[] btnAreas;
    [Header("BGM#1再生ボタン")]
    public Button btnBgm1;
    [Header("BGM#2再生ボタン")]
    public Button btnBgm2;
    [Header("設定ボタン")]
    public Button btnSetting;
    [Header("幻視ボタン")]
    public Button btnVision;
    [Header("ステータスボタン")]
    public Button btnStatus;
    [Header("背景イメージ")]
    public Image imgHome;

    [Header("設定用ポップアップのプレファブ")]
    public SettingPopup settingPopupPrefab;
    [Header("設定用ポップアップの生成位置")]
    public Transform canvasTran;
    [Header("幻視ポップアップのプレファブ")]
    public VisionPopup visionPopupPrefab;
    [Header("ステータスポップアップのプレファブ")]
    public StatusPopup statusPopupPrefab;

    public bool isClickable;    // 設定ボタン重複タップ防止用
    public QuestSelectPopup questSelectPopupPrefab;
    public SwipeMoveObject swipeMoveObject;

    void Start(){
        SoundManager.Instance.PlayBGM(GameData.instance.homeBgmType);
        TransitionManager.instance.TransFadeIn(0.7f);
        //StartCoroutine(TransitionManager.instance.EnterScene());

        // ボタンの登録
        for (int i = 0; i < btnAreas.Length; i++) {
            btnAreas[i].onClick.AddListener(() => OnClickOpenQuestSelectPopup(i - 1));
        }      
        btnBgm1.onClick.AddListener(() => OnClickChangeBGM(SoundManager.ENUM_BGM.HOME_1));
        btnBgm2.onClick.AddListener(() => OnClickChangeBGM(SoundManager.ENUM_BGM.HOME_2));
        btnSetting.onClick.AddListener(OnClickOpenSettingPopup);
        btnVision.onClick.AddListener(OnClickOpenVisionPopup);
        btnStatus.onClick.AddListener(OnClickOpenStatusPopup);

        StartCoroutine(SetupHomeImage());

        // Debug用
        GameData.instance.questClearCountsByArea = new int[btnAreas.Length];
        GameData.instance.questClearCountsByArea[0] = 3;
    }

    /// <summary>
    /// 幻視ポップアップを生成
    /// </summary>
    private void OnClickOpenVisionPopup() {
        if (isClickable) {
            return;
        }
        isClickable = true;
        swipeMoveObject.isWindowOpen = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        VisionPopup visionPopup = Instantiate(visionPopupPrefab, canvasTran, false);
        visionPopup.Setup(this);
    }

    /// <summary>
    /// QuestSelectPopupを生成
    /// </summary>
    /// <param name="areaNo"></param>
    private void OnClickOpenQuestSelectPopup(int areaNo) {
        if (isClickable) {
            return;
        }
        isClickable = true;
        swipeMoveObject.isWindowOpen = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        QuestSelectPopup questSelectPopup = Instantiate(questSelectPopupPrefab, canvasTran, false);
        questSelectPopup.CreateQuestPanels(areaNo, this);
    }

    /// <summary>
    /// Questシーンへ遷移(QuestSelectPopupから呼び出される)
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartQuestScene() {
        //Sequence seq = DOTween.Sequence();
        //seq.Append(btnAreas.transform.DOScale(1.2f, 0.15f));
        //seq.Append(btnAreas.transform.DOScale(1.0f, 0.15f));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SceneStateManager.instance.MoveScene(SCENE_TYPE.QUEST, 0.7f));
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
        btnBgm1.interactable = false;
        btnBgm2.interactable = false;
        // 変更を保存
        StartCoroutine(CoroutineCheck());
    }

    /// <summary>
    /// BGMの変更をPlayFabへ保存
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoroutineCheck() {
        yield return StartCoroutine(PlayFabManager.instance.UpdataUserDataInOptions());
        // 保存完了後にボタンを押せるようにする
        btnBgm1.interactable = true;
        btnBgm2.interactable = true;
    }

    /// <summary>
    /// 設定用ポップアップを生成
    /// </summary>
    private void OnClickOpenSettingPopup() {
        if (isClickable) {
            return;
        }
        isClickable = true;
        swipeMoveObject.isWindowOpen = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        SettingPopup setting = Instantiate(settingPopupPrefab, canvasTran, false);
        setting.Setup(this);
    }

    /// <summary>
    /// ステータスポップアップを生成
    /// </summary>
    private void OnClickOpenStatusPopup() {
        if (isClickable) {
            return;
        }
        isClickable = true;
        swipeMoveObject.isWindowOpen = true;
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        StatusPopup status = Instantiate(statusPopupPrefab, canvasTran, false);
        status.Setup(this);
    }
}
