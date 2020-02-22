using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public int level;
    public int ap = 100;
    public string playerName;
    public int maxHp;
    public long money;
    public long exp;　　　　　  // 蓄積型。レベルが上がる
    public long soulPoint;      // 消費型。英傑のスキルレベル(ランク)を上げるのに必要。
    public int contactCount;    // 現在契約している英傑の数
    public int clearQuestCount;
    public long totalCount;
    public int skillPoint;

    public int maxSp;
    public int physical;      // 武術
    public int mental;        // 魔術
    public int technical;     // 技術
    public int actionPoint;   // 行動力
    public int response;      // 反応
    public int search;        // 探索

    public bool endTutorial;

    public bool isGetPlayfabDatas;   // PlayfabからTitleとUserデータ取得確認

    // データ関連
    public AreaDataList areaDatas;
    public CharaDataList charaDatas;
    public ActionDataList actionDataList;
    public FieldDataList fieldDataList;
    public EnemyDataList enemyDataList;
    public SecretItemDataList secretItemDataList;
    public LandscapeDataList landscapeDataList;
    public TrapDataList trapDataList;

    [Header("現在のパーティキャラリスト")]
    public List<CharaDataList.CharaData> charas;
    [Header("現在実行可能な行動リスト")]
    public List<ActionDataList.ActionData> actions;
    [Header("エリアリスト")]
    public List<AreaDataList.AreaData> areas;

    public float volumeBGM;
    public float volumeSE;
    public SoundManager.ENUM_BGM homeBgmType;

    public bool useDebugOn;
    public QuestManager questManager;

    public ItemSO itemSO;
    public bool isScriptableObjectLoad;
    public bool isFirstAccess;

    public string haveSkillNoListString;   // 所持しているスキルを１つの文字列として受け取ったり送ったりするための変数

    [Header("所持しているスキル番号リスト")]
    public List<int> haveSkillNoList;　　　// haveSkillNoListStringを元にリスト化し、これを元にhaveSkillNoListStringを作る

    public List<PlayFabManager.SkillData> haveSkillDatas;　　// haveSkillNoListを参照してSkillDataから所持しているスキルリストを作る

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }      
    }

    void Start() {
        if (useDebugOn) {
            // デバッグ用
            LoadDebug();
        }
    }

    private void LoadDebug() {
        level = 1;
        ap = 3000;
        playerName = "souls";
        maxHp = 100;
        money = 0;
        exp = 0;
        soulPoint = 0;
        contactCount = 0;
        //volumeBGM = 0.5f;
        //volumeSE = 0.5f;
        //homeBgmType = SoundManager.ENUM_BGM.HOME_1;
        //questManager.Init();
    }

    /// <summary>
    /// スキルリストをカンマ区切りの１つの文字列にして戻す
    /// </summary>
    /// <returns></returns>
    public string GetHaveSkillListString() {
        string retStr = "";
        for (int i = 0; i < haveSkillNoList.Count; i++) {
            retStr += haveSkillNoList[i] + ",";
        }
        if (retStr != "") {
            return retStr.Substring(0, retStr.Length - 1);
        } else {
            return "";
        }
    }
}
