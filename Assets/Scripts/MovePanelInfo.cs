using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class MovePanelInfo : MonoBehaviour
{
    public Button btnSubmit;
    public TMP_Text txtPlace;
    public Image imgPlace;

    // 配列かリストで選択可能なスキルを取得し、タップしたかどうかを監視する

    public void InitMovePanel() {
        // 成功率、イベントの種類に合わせたアイコン、地形の名前などを受け取りセットする

    }

    void Start() {
        btnSubmit.onClick.AddListener(() => OnClickSubmit());
    }

    private void OnClickSubmit() {
        // パネルをアニメさせて見えなくさせる
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(1.2f, 0.3f));
        seq.Append(transform.DORotate(new Vector3(0, 720, 0), 1.0f, RotateMode.FastBeyond360));
        seq.Join(transform.DOScale(0, 1.0f));
    }

    // Updateでスキルのオンオフを監視
    // タップされたスキルのリストをゲッターメソッドに渡し、この移動パネルに該当する要素がある場合には
    // その要素の数値や内容を変更する
}
