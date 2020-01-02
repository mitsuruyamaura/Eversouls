using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 各ステージの開始時と終了時のトランジション用クラス
/// フェードイン中に画面が見えないようにカメラとUIは非アクティブにしておく
/// </summary>
public class TransitionManager : MonoBehaviour {

    public static TransitionManager instance;

    [Header("フェイドイン／フェイドアウト制御用クラス")]
    public Fade fade;
    [Header("マスク用イメージ制御用")]
    public Image imgMask;

    //[SerializeField, Header("マスク用イメージ制御用")]
    //public GameObject maskImage;
    //private bool isSet;

    private void Awake() {
        imgMask.material.SetFloat("_Flip", imgMask.material.GetFloat("_Flip") + 2.0f);
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Start () {
        //TransFadeIn(0.7f);
    }

    /// <summary>
    /// フェイドイン処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnterScene() {
        // マスク画像をセットする
        // GetFloatしているのは現在値を取得し、そこに加算したいため
        // 値を直接入れればその値になる
        imgMask.material.SetFloat("_Flip", imgMask.material.GetFloat("_Flip") + 2.0f);
        yield return new WaitForSeconds(0.2f);

        // マスク画像がゲーム画面から消えるまでアニメさせる
        while (imgMask.material.GetFloat("_Flip") > -1.0f) {
            // マテリアルのFlipプロパティを徐々に減算することでマスク画像を消す
            imgMask.material.SetFloat("_Flip", imgMask.material.GetFloat("_Flip") - 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// フェイドアウト処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator ExitScene() {
        // マスク画像がゲーム画面を隠すまでアニメさせる
        while (imgMask.material.GetFloat("_Flip") < 1.0f) {
            // マテリアルのFlipプロパティを徐々に加算することでマスク画像を表示する
            imgMask.material.SetFloat("_Flip", imgMask.material.GetFloat("_Flip") + 0.05f);
            yield return new WaitForSeconds(0.01f);
        }
    }


    /// <summary>
    /// 未使用
    /// 各ステージの開始時のフェイドイン処理
    /// フェイドインが終わってからゲーム画面関連を表示させる
    /// </summary>
    //public void TransFadeIn(float time) {
    //    fade.FadeIn(0.1f, () => {
    //        if (!isSet) {
    //            isSet = true;
    //            imgMask.material.SetFloat("_Flip", imgMask.material.GetFloat("_Flip") - 0.05f); ;
    //            SetUp();
    //        }
    //        fade.FadeOut(time);
    //    });
    //}

    /// <summary>
    /// 未使用
    /// カメラなどを有効化する処理
    /// </summary>
    //private void SetUp() {
    //    maskImage.SetActive(false);
    //}

    /// <summary>
    /// 未使用
    /// 各ステージ終了時のフェイドアウト処理
    /// </summary>
    /// <param name="time"></param>
	//public void TransFadeOut(float time) {
 //       fade.FadeIn(0.7f, () =>
 //       {
 //           fade.FadeOut(time);
 //       });
 //   }
}
