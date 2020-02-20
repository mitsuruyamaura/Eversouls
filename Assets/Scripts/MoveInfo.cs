using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MoveInfo : MonoBehaviour
{
    public Button btnSubmit;
    public TMP_Text txtPlace;
    public Image imgPlace;

    void Start() {
        btnSubmit.onClick.AddListener(() => OnClickSubmit());
    }

    private void OnClickSubmit() {
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.2f, 0.3f));
        seq.Append(transform.DORotate(new Vector3(0, 720, 0), 1.0f, RotateMode.FastBeyond360));
        seq.Join(transform.DOScale(0, 1.0f));
    }
}
