using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GameData : MonoBehaviour
{
    public static GameData instance;

    public int currency;
    public bool rewordOn;
    public int level;
    public int ap = 100;
    public string playerName;
    public int maxHp;
    public long money;
    public long exp;　　　　　  // 蓄積型。レベルが上がる
    public long soulPoint;      // 消費型。英傑のスキルレベル(ランク)を上げるのに必要。
    public int contactCount;    // 現在契約している英傑の数
    public int[] questClearCountsByArea; // エリア別のクエストクリア回数
    public long totalCount;
    public int skillPoint;      // 不要になりそう

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
    public SecretItemDataList secretItemDataList;
    public LandscapeDataList landscapeDataList;
    public TrapDataList trapDataList;

    [System.Serializable]
    public class QuestData {
        // stringを配列に入れるための用意
        [Header("イベント発生率(敵/秘匿物/罠/景勝地)")]
        public int[] eventsRates;
        [Header("敵の出現率")]
        public int[] enemyEncountRates;
        public int[] secretItemRates;
        public int[] landscapeRates;
        public int[] trapRates;
        public FIELD_TYPE field;

        [Header("クエストに出現する地形リスト")]
        public List<FieldDataList.FieldData> fieldDatas;
        public FIELD_TYPE[] fieldTypes;                // 出現する地形タイプ
        [Header("地形の出現割合")]
        public int[] feildRates;
        [Header("エリアのタイプ")]
        public AREA_TYPE areaType;
        [Header("昼夜")]
        public TIME_TYPE timeType;
        [Header("希少度")]
        public RARE_TYPE areaRarelity;
        [Header("温度")]
        public TEMPERATURE_TYPE tempratureType;
        public string areaInfo;
        public int iconNo;
    }
    public List<QuestData> questDataList = new List<QuestData>();
    public QuestData questData = new QuestData();
    public int choiceAreaNo;   // Homeシーンで選択したエリアの番号(SEASON_TYPEにキャストする)

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

    public ItemSO itemSO;
    public bool isScriptableObjectLoad;
    public bool isFirstAccess;

    public string haveSkillNoListString;        // 所持しているスキルを１つの文字列として送受信するための変数
    public string memoriaLevelListString;   // メモリアのレベルを１つの文字列として送受信するための変数

    [Header("所持しているスキル番号リスト")]
    public List<int> haveSkillNoList = new List<int>();　　　// haveSkillNoListStringを元にリスト化し、これを元にhaveSkillNoListStringを作る
    [Header("所持しているスキルのデータリスト")]
    public List<PlayFabManager.SkillData> haveSkillDatas = new List<PlayFabManager.SkillData>();　　// haveSkillNoListを参照してSkillDataから所持しているスキルリストを作る
    
    [Header("現在のメモリアのレベルリスト")]
    public List<int> memoriaLevelList = new List<int>();
    [Header("現在のメモリアの経験値リスト")]
    public List<int> memoriaExpList = new List<int>();
    [Header("各メモリアの現在のスキルレベルリスト")]　// KeyはMemoriaNo、Valueはレベルの配列
    public Dictionary<int, int[]> currentSkillLevelOfMemorias = new Dictionary<int, int[]>();

    [Header("現在のメモリアのデータリスト")]
    public List<PlayFabManager.MemoriaData> currentMemoriaDataList = new List<PlayFabManager.MemoriaData>();

    public int activeMomoriaNo;

    public enum ABILITY_TYPE {
        PHYSICAL,
        MENTAL,
        TECHNICAL
    }
    public ABILITY_TYPE abilityType;
    public TEMPERATURE_TYPE temperatureType;

    public SpriteAtlas spriteAtlas;

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
    /// List<int>をカンマ区切りの１つの文字列にして戻す
    /// </summary>
    /// <returns></returns>
    public string ConvertListIntToString(List<int> intList) {
        string retStr = "";
        for (int i = 0; i < intList.Count; i++) {
            retStr += intList[i] + ",";
        }
        if (retStr != "") {
            return retStr.Substring(0, retStr.Length - 1);
        } else {
            return "";
        }
    }

    public int GetSkillAmountCount(int cost, int skillAbilityNo) {
        // スキルの参照する能力値を設定
        ABILITY_TYPE abilityType = (ABILITY_TYPE)skillAbilityNo;
        int baseAbility = 0;
        switch (abilityType) {
            case ABILITY_TYPE.PHYSICAL:
                baseAbility = physical;
                break;
            case ABILITY_TYPE.MENTAL:
                baseAbility = mental;
                break;
            case ABILITY_TYPE.TECHNICAL:
                baseAbility = technical;
                break;
        }
        int amountCount = 0;
        
        return amountCount = Mathf.FloorToInt(baseAbility / cost);
    }
}
