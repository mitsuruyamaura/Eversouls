using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayDebugText : MonoBehaviour
{
    public Text txtDebug;

    public void DisplayText() {
        txtDebug.text = "EnemyDataCount : " + PlayFabManager.instance.enemyDataList.Count.ToString();
        txtDebug.text += "\nSkillDataCount : " + PlayFabManager.instance.skillDataList.Count.ToString();
        txtDebug.text += "\nhaveSkillDataCount : " + GameData.instance.haveSkillDatas.Count.ToString();
        txtDebug.text += "\nPlayFabSkillDataCount : " + PlayFabManager.instance.skillDataList.Count.ToString();
    }
}
