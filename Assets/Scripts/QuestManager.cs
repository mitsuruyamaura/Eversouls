using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestData questDataPrefab;
    public ActionInfo actionInfoPrefab;

    public List<QuestData> questList = new List<QuestData>();
    public List<ActionInfo> actionList = new List<ActionInfo>();
    
    public Transform questTran;
    public Transform actionTran;

    public int actionSetCount;

    void Start() {
        // スタートエリアを選択するためQuestDataオブジェクトをインスタンスする
        if (!GameData.instance.endTutorial) {
            // チュートリアルが終わっていなければ
            QuestData quest = Instantiate(questDataPrefab, questTran, false);
            quest.InitQuestData(0);
            quest.no = 0;
            quest.clearCount = 10;
            quest.questManager = this;
            questList.Add(quest);
        } else {
            for (int i = 0; 0 < GameData.instance.clearQuestCount; i++) {
                QuestData quest = Instantiate(questDataPrefab, questTran, false);
                int areaType = Random.Range(0, GameData.instance.areaDatas.areaDataList.Count);
                quest.InitQuestData(areaType);
                quest.no = GameData.instance.totalCount++;
                quest.questManager = this;

                // TODO 目標設定

                questList.Add(quest);
            }
        }
        Debug.Log("ok");
    }

    /// <summary>
    /// 移動開始前に敵が出現するかチェック
    /// </summary>
    public void CheckEnterEnemy() {

    }

    public void CheckTrap() {

    }

    public void CheckChest() {

    }

    public void CreateActions() {
        actionList = new List<ActionInfo>();

        // エリアごとに生成可能な地形の出現割合を合計
        int total = 0;
        for (int i = 0; i < questList[0].feildRates.Length; i++) {
            total += questList[0].feildRates[i];
        }
        Debug.Log(total);

        // 移動可能な地形をランダムに生成する
        for (int i = 0; i < actionSetCount; i++) {
            int value = Random.Range(0, total + 1);
            for (int j = 0; j < questList[0].feildRates.Length; j++) {
                Debug.Log(value);
                if (value < questList[0].feildRates[j]) {
                    ActionInfo action = Instantiate(actionInfoPrefab, actionTran, false);
                    action.questManager = this;
                    action.InitField(questList[0].fieldDatas[j]);
                    actionList.Add(action);
                    break;
                } else {
                    value -= questList[0].feildRates[j];
                }
            }
        }

        // 実行可能な行動を作成

    }

    public void DestroyActions() {
        for (int i = 0; i < actionList.Count; i++) {
            Destroy(actionList[i].gameObject);
        }
        CreateActions();
    }
}
