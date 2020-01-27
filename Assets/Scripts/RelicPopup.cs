using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicPopup : PopupBase
{
    public ActionInfo actionInfoPrefab;
    public Transform actionTran;
    private QuestManager questManager;

    public void CreateHeroActionInfos(Hero hero) {
        questManager = GameObject.FindGameObjectWithTag("Quest").GetComponent<QuestManager>();

        // Heroの持つアイテム(レリック)の番号で検索して行動パネルを探す
        foreach (ItemMasterData.ItemData data in GameData.instance.itemSO.itemMasterData.item) {
            for (int i = 0; i < hero.relicActionList.Length; i++) {
                if (hero.relicActionList[i] == data.itemNo) {
                    ActionInfo actionInfo = Instantiate(actionInfoPrefab, actionTran, false);
                    actionInfo.InitRelicAction(data);
                    actionInfo.questManager = questManager;
                }
            }
        }
    }
}
