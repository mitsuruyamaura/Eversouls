using UnityEngine;

public class BrightnessChanger : MonoBehaviour
{
    private float currentBrightness;     // 端末の従来の輝度
    private float maxBrightness = 1.0f;  // 輝度の最大値

    void Start() {
        // 端末の現在の輝度を保存
        currentBrightness = Screen.brightness;

        // ここでDebugを入れておいて保存されたか、確認するといいです
        Debug.Log(currentBrightness);

        // 輝度を変更する
        ChangeBrightness(maxBrightness);
    }

    /// <summary>
    /// 画面の輝度を引数の値に変更する
    /// </summary>
    /// <param name="brightness"></param>
    private void ChangeBrightness(float brightness) {
        Screen.brightness = brightness;
    }

    /// <summary>
    /// アプリがバックグラウンドに入るか、バックグラウンドから戻ったら呼ばれる
    /// </summary>
    /// <param name="pause"></param>
    private void OnApplicationPause(bool pause) {
        if (pause) {
            // アプリがバックグラウンドの回るか端末がスリープしたら輝度を元の明るさに戻す
            ChangeBrightness(currentBrightness);
            Debug.Log("バックグラウンド処理");
        } else {
            // アプリが戻ったら輝度を最大値に変更
            ChangeBrightness(maxBrightness);
        }
    }
}
