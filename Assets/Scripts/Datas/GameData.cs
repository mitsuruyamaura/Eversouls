using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public int level;
    public int ap;
    public string playerName;
    public int maxHp;
    public long money;
    public long exp;　　　　　  // 蓄積型。レベルが上がる
    public long visionPoint;    // 消費型。英傑のスキルレベル(ランク)を上げるのに必要。
    public int contactCount;    // 現在契約している英傑の数
    public int clearQuestCount;
    public long totalCount;

    public bool endTutorial;
    public AreaDataList areaDatas;
    public CharaDataList charaDatas;
    public ActionDataList actionDataList;
    public FieldDataList fieldDataList;

    [Header("現在のパーティキャラリスト")]
    public List<CharaDataList.CharaData> charas;
    [Header("現在実行可能な行動リスト")]
    public List<ActionDataList.ActionData> actions;
    [Header("エリアリスト")]
    public List<AreaDataList.AreaData> areas;

    public float volumeBGM;
    public float volumeSE;

    public bool useDebugOn;
    public QuestManager questManager;

    public void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
        if (useDebugOn) {
            LoadDebug();
        }
    }

    private void LoadDebug() {
        level = 1;
        ap = 3000;
        playerName = "visions";
        maxHp = 100;
        money = 0;
        exp = 0;
        visionPoint = 0;
        contactCount = 0;

        questManager.Init();
    }
}
