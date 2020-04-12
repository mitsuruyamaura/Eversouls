using UnityEngine;
//using UnityEngine.UI;　　　// Scrollbar用

/// <summary>
/// ゲームオブジェクトをスワイプで移動させるクラス
/// </summary>
public class SwipeMoveObject : MonoBehaviour
{
    [Header("スワイプで移動させるオブジェクト")]
    public Transform targetTran;
    [Header("移動速度")]
    public float speed;      // 3-5位でいい
    Vector3 mousePos;        // タップ位置の保存用

    //Vector2 startPos;　　　// MoveSwipeメソッドで使用
    //float xSpeed;　　　　　// MoveSwipeメソッドで使用
    //float ySpeed;    　 　 // MoveSwipeメソッドで使用

    void Update() {
        //MoveSwipe();
        Move();
    }

    /// <summary>
    /// スワイプに合わせてすぐに移動するタイプ
    /// </summary>
    private void Move() {
        if (Input.GetMouseButtonDown(0)) {
            mousePos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)) {
            // 毎フレームタップの位置を更新するので、同時に移動する
            Vector3 mouseDiff = Input.mousePosition - mousePos;
            mousePos = Input.mousePosition;

            // 移動させる位置を設定
            Vector3 newPos = targetTran.localPosition + new Vector3(mouseDiff.x * speed, 0, 0);
            // 画面外に移動しないように制御
            newPos.x = Mathf.Clamp(newPos.x, -670, 670);

            // 対象となるオブジェクトの位置を更新(移動)
            targetTran.localPosition = newPos;
        }
    }

    /// <summary>
    /// スワイプ後タップをやめたあと、移動を開始するタイプ。（未使用）
    /// </summary>
    //private void MoveSwipe() {
    //    if (Input.GetMouseButtonDown(0)) {
    //        startPos = Input.mousePosition;
    //    } else if (Input.GetMouseButtonUp(0)) {
    //        Vector2 endPos = Input.mousePosition;
    //        float x = endPos.x - startPos.x;
    //        float y = endPos.y - startPos.y;
                       
    //        xSpeed = x / 250f;
    //        ySpeed = 0;
    //    }

    //    targetTran.Translate(xSpeed, ySpeed, 0);
    //    xSpeed *= 0.85f;

    //    float clamp = Mathf.Clamp(targetTran.localPosition.x, -670, 670);

    //    targetTran.localPosition = new Vector2(clamp, targetTran.localPosition.y);
    //}

    /// <summary>
    /// ScrollbarのOnValueChangedを利用する場合のメソッド（未使用）
    /// タップ時にその位置にスクロールするタイプ(スワイプではない)
    /// ScrollBarのOnValueChangedにScrollbarをアサインして使う
    /// </summary>
    /// <param name="scrollbar"></param>
    //public void ScrollWorldMapOnScrollBarValue(Scrollbar scrollbar) {
    //    float pos = -670 + (scrollbar.value * 1340);
    //    targetTran.localPosition = new Vector3(pos, targetTran.localPosition.y);
    //}
}
