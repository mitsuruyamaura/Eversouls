using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuestSelectPopup : PopupBase {

    public QuestData questDataPrefab;
    public List<QuestData> questList = new List<QuestData>();

    public Transform questTran;
    public Button btnRightArrow;
    public Button btnLeftArrow;

    //重複タップ防止用フラグ
    private bool isMoveButton;
    //現在のボタンリストの番号
    private int currentButtonNo = 0;

    public PageScrollRect page;
    public QuestData selectQuestData;   // ObjectCencer.csよりもらう

    private int displayNo;

    protected override void Start() {
        base.Start();
        // 矢印ボタン設定
        btnRightArrow.onClick.AddListener(() => OnClickNextButtonList());
        btnLeftArrow.onClick.AddListener(() => OnClickPrevButtonList());
        //btnLeftArrow.gameObject.SetActive(false);

        btnFilter.interactable = false;
        page.questSelectPopup = this;
    }

    /// <summary>
    /// 選択したエリアのクエストパネルを生成
    /// </summary>
    /// <param name="areaNo"></param>
    public void CreateQuestPanels(int areaNo) {
        for (int i = 0; i < GameData.instance.questClearCountsByArea[areaNo]; i++) {
            QuestData quest = Instantiate(questDataPrefab, questTran, false);
            int areaType = Random.Range(0, GameData.instance.areaDatas.areaDataList.Count);
            quest.InitQuestData(areaType);
            questList.Add(quest);
        }
    }

    /// <summary>
    /// ボタンリストを1ページ進める
    /// </summary>
    private void OnClickNextButtonList(bool isSwipe = false) {
        if (!isMoveButton) {
            //ボタンの重複防止用のフラグを立てる
            isMoveButton = true;

            //左矢印ボタンを表示する
            btnLeftArrow.gameObject.SetActive(true);

            //ボタンリストの番号を１つ(ページ進める)
            currentButtonNo++;
            Debug.Log(currentButtonNo);

            //ボタンリストが最終ページなら右矢印ボタンを非表示にする
            if (currentButtonNo >= questList.Count - 1) {
                btnRightArrow.gameObject.SetActive(false);
            }

            //スワイプでボタンリストを移動させていたら無視
            if (!isSwipe) {
                //スワイプしていない（ボタンを押していない）なら、1ページ進めたボタンリストを表示
                float destX = -currentButtonNo * page.pageWidth;
                page.content.anchoredPosition = new Vector2(destX, page.content.anchoredPosition.y);
                page.tempIndex = -currentButtonNo;
            }
            //再度ボタンを押せるようにする
            isMoveButton = false;
        }
    }


    /// <summary>
    /// ボタンリストを1つもどす
    /// </summary>
    private void OnClickPrevButtonList(bool isSwipe = false) {
        if (!isMoveButton) {
            //ボタンの重複防止用のフラグを立てる
            isMoveButton = true;

            //右矢印ボタンを表示する
            btnRightArrow.gameObject.SetActive(true);

            //ボタンリストの番号を一つページを戻す
            currentButtonNo--;
            Debug.Log(currentButtonNo);

            //すでにリストが最初のページなら左矢印ボタンを非表示にする
            if (currentButtonNo <= 0) {
                btnLeftArrow.gameObject.SetActive(false);
            }

            //スワイプでボタンリストを移動させていたら無視
            if (!isSwipe) {
                //スワイプしていない（ボタンを押した）なら、1ページ戻したボタンリストを表示
                float destX = currentButtonNo * page.pageWidth;
                page.content.anchoredPosition = new Vector2(-destX, page.content.anchoredPosition.y);
                page.tempIndex = currentButtonNo;
            }

            //再度ボタンを押せるようにする
            isMoveButton = false;
        }
    }

    //スワイプに合わせてボタンの左右矢印ボタンの表示/非表示を切り替え
    public void OnClickArrowButton(int prevPageIndex) {
        //比較用にcurrentArrowButtonListNoを使うが、currentButtonListNoはこの後のメソッドで変更するので
        //ここではcurrentButtonListNoをいったん別の変数に入れて利用する
        int temp = currentButtonNo;

        //prevPageIndexの値をみて矢印ボタンの表示状態を非表示に変更する
        if (prevPageIndex == 0) {
            btnLeftArrow.gameObject.SetActive(false);
        }

        if (prevPageIndex == questList.Count - 1) {
            btnRightArrow.gameObject.SetActive(false);
        }

        //比較してどちらの矢印ボタンのメソッドにするか決定する
        if (temp > prevPageIndex) {
            OnClickPrevButtonList(true);
        } else {
            OnClickNextButtonList(true);
        }
    }
}
