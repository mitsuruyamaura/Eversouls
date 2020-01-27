using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RelicEffectIcon : MonoBehaviour
{
    public Button btnDelete;
    public Image imgIcon;

    void Setup(int iconNo) {
        btnDelete.onClick.AddListener(OnClickDelete);
        // Imageをリソースロードして差し替え

        DOTween.ToAlpha(() => imgIcon.color, color => imgIcon.color = color, 1f, 0.25f);
    }

    /// <summary>
    /// エフェクトを破棄する
    /// </summary>
    private void OnClickDelete() {
        DOTween.ToAlpha(() => imgIcon.color, color => imgIcon.color = color, 0f, 0.25f);
        Destroy(gameObject, 0.25f);
    }
    
}
