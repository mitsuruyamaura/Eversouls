using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ボタンをスワイプで変更するためのクラス
/// </summary>
public class PageScrollRect : ScrollRect {

    public QuestSelectPopup questSelectPopup;
    public float pageWidth;
    public int prevPageIndex = 0;
    public int tempIndex;

    protected override void Awake() {
        base.Awake();
        GridLayoutGroup grid = content.GetComponent<GridLayoutGroup>();

        // 1ページの幅を取得
        pageWidth = grid.cellSize.x + grid.spacing.x;
        //Debug.Log(pageWidth);
    }

    protected override void Start() {
        base.Start();
    }

    /// <summary>
    /// ドラッグを開始したとき
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnBeginDrag(PointerEventData eventData) {
        base.OnBeginDrag(eventData);
    }

    /// <summary>
    /// ドラッグを終了したとき
    /// </summary>
    /// <param name="eventData"></param>
    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);
        //Debug.Log(eventData);
        //Debug.Log(eventData.delta.x);

        // ドラッグを終了したとき、スクロールをとめる
        // スナップさせるページが決まった後も慣性が効いてしまうため
        StopMovement();

        // RoundToint　四捨五入
        // 元の位置に戻らずに移動しているか確認用
        // スナップさせるページを決定する
        // スナップさせるページのインデックスを決定する
        bool isMove = false;
        int pageIndex = Mathf.RoundToInt(content.anchoredPosition.x / pageWidth);

        if (tempIndex != pageIndex) {
            isMove = true;
            tempIndex = pageIndex;
        }

        // Abs絶対値
        // -の数値が出たら-がとった整数にする処理
        // 例えば　-3が3になる　3はそのまま3
        // ページが変わっていない且つ、素早くドラッグした場合
        if (pageIndex == prevPageIndex && Mathf.Abs(eventData.delta.x) >= 1) {
            pageIndex += (int)Mathf.Sign(eventData.delta.x);
        }

        // スワイプ距離計算
        // Contentをスクロール位置を決定する
        // 必ずページにスナップさせるような位置にする
        float destX = pageIndex * pageWidth;

        // content.anchoredPosition 箱ごと滑らかに動く
        content.anchoredPosition = new Vector2(destX, content.anchoredPosition.y);

        // 元の位置に戻らずに移動しているなら
        if (isMove) {
            // 矢印の表示を連動させる
            prevPageIndex = Mathf.Abs(pageIndex);
            questSelectPopup.OnClickArrowButton(prevPageIndex);
        }
        prevPageIndex = pageIndex;
        //Debug.Log(prevPageIndex);
    }
}

