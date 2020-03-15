using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画面を指定した色に一瞬光らせるエフェクト用クラス
/// </summary>
public class FlushController : MonoBehaviour
{
    public Image imgFlush;
    private bool isSwitch; 
    private float timer;

    void Start() {
        ClearColor();
    }
    /// <summary>
    /// オブジェクトの色を透明にする
    /// </summary>
    public void ClearColor() {
        imgFlush.color = Color.clear;
    }

    /// <summary>
    /// オブジェクトを引数の色に一定時間点滅させる
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="flushTime"></param>
    /// <returns></returns>
    public IEnumerator FlushImage(float[] colors, float flushTime) {
        imgFlush.color = new Color(colors[0], colors[1], colors[2], colors[3]);
        isSwitch = true;

        yield return new WaitForSeconds(flushTime);
        isSwitch = false;
    }

    void Update() {
        if (isSwitch) {
            // 第1引数の色を第2引数の色に時間をかけて変更する
            imgFlush.color = Color.Lerp(imgFlush.color, Color.clear, Time.deltaTime);       
        }
    }
}
