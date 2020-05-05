using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;

public class PlayFabManager : MonoBehaviour {

    public static PlayFabManager instance;

    // PlayFab Login or Error
    string loginMessage;
    public bool isLogin;
    public bool isError;
    public string conErrorReport;

    public int rewordCurrencyPoint;
    public int subtractCurrencyPoint;

    [System.Serializable]
    public class SkillData {
        public int skillNo;               // スキルの番号
        public string skillName;          // スキルの名前
        public int cost;
        public string skillType;
        public int imageNo;
        public int skillAbilityNo;        // スキルの使用回数や効果を適用する際に参照する能力値()
        public string eventTypesString;
        public string expListString;      // レベルアップに必要な経験値リスト

        public EVENT_TYPE[] eventTypes;   // ここからローカル
        public int amountCount;
        public int[] expList;

        public int level;                 // マスターでは持たない
    }
    public List<SkillData> skillDataList;

    [System.Serializable]
    public class ItemData {
        public int itrmNo;
        public int itemCd;
        public string itemName;
        public string itemIcon;
        public int itemType;                    // Conditionクラス生成時の参照(キャストしてITEM_TYPEにする)
        public int exp;                         // リザルト画面で獲得できる経験値
        public string info;
        public string rarelityString;
        public string appearanceString;
        public string seasonTypeString;
        public string levelTypeString;
        public string levelBonusString;
        public string abilityValueString;
        public string abilityNameString;
        public string effectString;

        public RARE_TYPE[] rarelities;             // 希少度。敵のタイプによって変わる 0 - 4
        public int[] appearances;                  // 出現率。敵のタイプによって変わる。
        public SEASON_WIND_TYPE[] seasonTypes;     // 落とすエリア
        public ENEMY_LEVEL_TYPE[] levelTypes;      // 落とす敵のタイプ
        public float[] levelBonuses;               // レベルの分だけ加算される値と加算される能力（能力値以外も含む）
        public float[] abilityValues;              // 能力値や他の値に加算される値群。levelBonusとセット 
        public string[] abilityNames;
        public string[] effects;

        public int level;                       // アイテムのレベル(Masterではもたない)
    }
    public List<ItemData> itemDataList;

    [System.Serializable]
    public class EnemyData {                       // expはシーズンとレアリティにより決定（あるいはアイテム）
        public int no;                             // 管理番号
        public string name;                        // 名前
        public int hp;
        public int vigilance;                      // 警戒。先制攻撃発生のペナルティになる
        public int appearance;                     // 出現割合  
        public string rarelityString;              // 変換用
        public string timeTypeString;
        public string seasonTypeString;
        public string lebelTypeString;
        public string skillAbilitiesString;
        public string lebelBonusString;
        public string habitatsString;

        public RARE_TYPE rarelity;                 // 希少度
        public TIME_TYPE timeType;                 // 出現時間帯。ALLなら常時出現        
        public WEAPON_TYPE weaponType;             // ランダムそのまま弱点属性になる
        public ELEMENT_TYPE elementType;           // ランダム
        public SEASON_WIND_TYPE seasonType;        // 生息エリア
        public ENEMY_LEVEL_TYPE levelType;         // ボスかどうか
        public int[] skillAbilities;          　　 // 能力値{武術、魔術、技術}の順番。戦闘時にランダムで選択され、目標値となる。差分がダメージになる
        public int[] levelBonus;                   // クエストの進行度によるレベルボーナス値{武術、魔術、技術}の順番でランダムにレベル回数だけ加算される
        public FIELD_TYPE[] habitats;              // 生息エリア
        public int level;
        public string chestType;                   // ENEMY_LEVEL_TYPE,SEASON_WIND_TYPE,RARE_TYPEから参照して作る。会話イベントはスキルがあれば発生。ランダム
    }
    public List<EnemyData> enemyDataList;

    [System.Serializable]
    public class MemoriaData {   // 必要があればSkillDataも持たせる
        public int no;
        public string title;
        public string name;
        public int maxHp;
        public int physical;      // 武術
        public int mental;        // 魔術
        public int technical;     // 技術
        public int actionPoint;   // 行動力
        public int response;      // 反応
        public int search;        // 探索
        public string skillListString;
        public string lebelBonusString;
        public int iconNo;

        public int[] skillList;   // ここからローカル
        public int[] levelBonus;  // maxHp => searchまでのレベルアップボーナス(7項目)
        
        public int level;                           // マスターデータでは持たない。
        public int exp;　　　　　　　　　　　　　　 // マスターデータでは持たない。
    }
    public List<MemoriaData> memoriaDataList;


    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初期化とPlayfabへのログイン
    /// </summary>
    public void ConnectPlayfab() {
        // ログイン用フラグ初期化
        isLogin = false;
        isError = false;
        conErrorReport = "";

        // Setting title Id here can be skipped if you have set the value in Editor Extensions already.
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId)) {
            // Please change this value to your own titleId from PlayFab Game Manager
            PlayFabSettings.TitleId = "Eversouls";
        }
        // ログインを試みる
        var request = new LoginWithCustomIDRequest { CustomId = SystemInfo.deviceUniqueIdentifier, CreateAccount = true };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    /// <summary>
    /// ログイン成功時
    /// </summary>
    /// <param name="result"></param>
    private void OnLoginSuccess(LoginResult result) {
        Debug.Log("Congratulations, you made your first successful API call!");
        isLogin = true;

        // ゲーム情報を取得
        StartCoroutine(SetUpDetas());
    }

    /// <summary>
    /// ログイン失敗時
    /// </summary>
    /// <param name="error"></param>
    private void OnLoginFailure(PlayFabError error) {
        Debug.LogWarning("Something went wrong with your first API cll. :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
        conErrorReport = error.GenerateErrorReport();
        isError = true;

        // TODO エラーポップアップを生成
    }

    /// <summary>
    /// PlayFabへアクセスしてゲームに必要な情報を順番に取得
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetUpDetas() {
        yield return StartCoroutine(GetTitleData());
        yield return StartCoroutine(GetUserData());

        GameData.instance.isGetPlayfabDatas = true;
        Debug.Log("Press OK Title");
        SoundManager.Instance.PlayBGM(SoundManager.ENUM_BGM.TITLE);
    }

    /// <summary>
    /// PlayFabからユーザーデータを取得
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetUserData() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // TODO 取得できない場合にはエラーポップアップを生成
        }

        bool isWait = true;

        // リクエストを作りPlayFabにアクセスし、データを取得できたらOnSuccessへ
        GetUserDataRequest request = new GetUserDataRequest();
        PlayFabClientAPI.GetUserData(request, OnSuccess, OnError);

        // falseになるまで待機
        while (isWait) {
            yield return null;
        }

        void OnSuccess(GetUserDataResult result) {

            // 初回起動か、それ以外かで分岐
            GameData.instance.isFirstAccess = GetPlayfabUserDataBoolean(result, "isFirstAccess");
            if (!GameData.instance.isFirstAccess) {
                Debug.Log("GetUserDataResult : SetUp!");
                // 初回起動の場合は初期設定
                // 環境設定
                GameData.instance.isFirstAccess = true;
                GameData.instance.homeBgmType = SoundManager.ENUM_BGM.HOME_1;
                GameData.instance.volumeBGM = 0.5f;
                GameData.instance.volumeSE = 0.5f;

                // ユーザー情報設定
                GameData.instance.playerName = "everSouls";
                GameData.instance.level = 10;
                GameData.instance.exp = 1000;
                GameData.instance.maxHp = 300;
                GameData.instance.physical = 100;
                GameData.instance.mental = 50;
                GameData.instance.technical = 70;

                // 初期スキルデータの番号を取得
                // TODO　(チュートリアルでタイプを選択した際に決めるようにする)
                GameData.instance.haveSkillNoList = new List<int>();
                for (int i = 0; i < 6; i++) {
                    GameData.instance.haveSkillNoList.Add(i);
                }

                // 初期スキルデータの番号から、初期所持スキルリストに初期のスキルデータを取得
                foreach (SkillData skillData in skillDataList) {
                    for (int i = 0; i < GameData.instance.haveSkillNoList.Count; i++) {
                        if (skillData.skillNo == GameData.instance.haveSkillNoList[i]) {
                            GameData.instance.haveSkillDatas.Add(skillData);
                        }
                    }
                }

                // PlayFabを更新
                StartCoroutine(SetupUserDatas());
                StartCoroutine(UpdataUserDataInOptions());
            } else {
                // 保存されているデータを取得
                GameData.instance.homeBgmType = (SoundManager.ENUM_BGM)Enum.Parse(typeof(SoundManager.ENUM_BGM), GetPlayfabUserDataString(result, "homeBGM"));
                GameData.instance.volumeBGM = GetPlayfabUserDataFloat(result, "volumeBGM");
                GameData.instance.volumeSE = GetPlayfabUserDataFloat(result, "volumeSE");

                GameData.instance.playerName = GetPlayfabUserDataString(result, "playerName");
                GameData.instance.level = GetPlayfabUserDataInt(result, "level");
                GameData.instance.exp = GetPlayfabUserDataInt(result, "exp");
                GameData.instance.maxHp = GetPlayfabUserDataInt(result, "maxHp");
                GameData.instance.physical = GetPlayfabUserDataInt(result, "physical");
                GameData.instance.mental = GetPlayfabUserDataInt(result, "mental");
                GameData.instance.technical = GetPlayfabUserDataInt(result, "technical");

                GameData.instance.rewordOn = GetPlayfabUserDataBoolean(result, "rewordOn", false);
                GameData.instance.currency = GetPlayfabUserDataInt(result, "currency");

                // 所持しているスキルリストを取得
                GameData.instance.haveSkillNoListString = GetPlayfabUserDataString(result, "haveSkillNoList");
                if (GameData.instance.haveSkillNoListString != "") {
                    // ストリング化されたListがあれば、それを配列に入れてからintのListに直す
                    GameData.instance.haveSkillNoList = GameData.instance.haveSkillNoListString.Split(',').Select(int.Parse).ToList();
                }
                // 所持リストにスキルデータの参照を取得
                foreach (SkillData skillData in skillDataList) {
                    for (int i = 0; i < GameData.instance.haveSkillNoList.Count; i++) {
                        if (skillData.skillNo == GameData.instance.haveSkillNoList[i]) {
                            GameData.instance.haveSkillDatas.Add(skillData);
                        }
                    }
                }
                // TODO メモリア関連を追加する



                Debug.Log("GetUserDataResult : Success!");
            }
            // 待機を終了
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log(error.GenerateErrorReport());
            isWait = false;
            // TODO エラーポップアップを生成

        }
    }

    /// <summary>
    /// Playfabへ初回起動時の初期値を設定して更新
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetupUserDatas() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // 取得できない場合の処理
        }

        bool isWait = true;

        // 保存したいデータをStringでリクエストを作る(1回に10個まで)
        UpdateUserDataRequest request = new UpdateUserDataRequest() {
            Data = new Dictionary<string, string> {
                { "isFirstAccess", GameData.instance.isFirstAccess.ToString()},
                { "playerName", GameData.instance.playerName},
                { "level", GameData.instance.level.ToString()},
                { "exp", GameData.instance.exp.ToString()},
                { "maxHp", GameData.instance.maxHp.ToString()},
                { "physical", GameData.instance.physical.ToString()},
                { "mental", GameData.instance.mental.ToString()},
                { "technical", GameData.instance.technical.ToString()},
                { "haveSkillNoList", GameData.instance.ConvertListIntToString(GameData.instance.haveSkillNoList)},
            }
        };
        // PlayFabへ送り、結果で分岐
        PlayFabClientAPI.UpdateUserData(request, OnSuccess, OnError);

        while (isWait) {
            yield return null;
        }

        void OnSuccess(UpdateUserDataResult result) {
            Debug.Log("SetupUserDatasResult : OnSuccess");
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log("SetupUserDatasResult : OnError");
            Debug.Log(error.GenerateErrorReport());
            isWait = false;

            // エラーポップアップを生成
        }
    }

    /// <summary>
    /// Playfabのユーザーの基本データを更新
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdateUserDatas() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // 取得できない場合の処理
        }

        bool isWait = true;

        // 保存したいデータをStringでリクエストを作る
        UpdateUserDataRequest request = new UpdateUserDataRequest() {
            Data = new Dictionary<string, string> {
                { "playerName", GameData.instance.playerName},
                { "level", GameData.instance.level.ToString()},
                { "exp", GameData.instance.exp.ToString()},
                { "maxHp", GameData.instance.maxHp.ToString()},
                { "physical", GameData.instance.physical.ToString()},
                { "mental", GameData.instance.mental.ToString()},
                { "technical", GameData.instance.technical.ToString()},
                { "haveSkillNoList", GameData.instance.ConvertListIntToString(GameData.instance.haveSkillNoList)},
                
                // TODO メモリア関連も追加する

            }
        };
        // PlayFabへ送り、結果で分岐
        PlayFabClientAPI.UpdateUserData(request, OnSuccess, OnError);

        while (isWait) {
            yield return null;
        }

        void OnSuccess(UpdateUserDataResult result) {
            Debug.Log("UpdateUserDataResult : OnSuccess");
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log("UpdataUserDataResult : OnError");
            Debug.Log(error.GenerateErrorReport());
            isWait = false;

            // エラーポップアップを生成
        }
    }

    /// <summary>
    /// PlayFabにある、ユーザーの環境設定データを更新（オプション画面）
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdataUserDataInOptions() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // 取得できない場合の処理
        }

        bool isWait = true;

        // 保存したいデータをStringでリクエストを作る
        UpdateUserDataRequest request = new UpdateUserDataRequest() {
            Data = new Dictionary<string, string> {
                { "homeBGM", GameData.instance.homeBgmType.ToString()},
                { "volumeBGM", GameData.instance.volumeBGM.ToString()},
                { "volumeSE", GameData.instance.volumeSE.ToString()},
            }
        };
        // PlayFabへ送り、結果で分岐
        PlayFabClientAPI.UpdateUserData(request, OnSuccess, OnError);

        while (isWait) {
            yield return null;
        }

        void OnSuccess(UpdateUserDataResult result) {
            Debug.Log("UpdateUserDataResult : OnSuccess");
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log("UpdataUserDataResult : OnError");
            Debug.Log(error.GenerateErrorReport());
            isWait = false;

            // エラーポップアップを生成
        }
    }

    /// <summary>
    /// PlayFabにある、ユーザーの動画リワードの視聴状態を更新
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdataUserReword() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // TODO 取得できない場合エラーポップアップを生成
        }

        bool isWait = true;

        // 保存したいデータをStringでリクエストを作る
        UpdateUserDataRequest request = new UpdateUserDataRequest() {
            Data = new Dictionary<string, string> {
                { "rewordOn", GameData.instance.rewordOn.ToString()},
                { "currency", GameData.instance.currency.ToString()}
            }
        };
        // PlayFabへ送り、結果で分岐
        PlayFabClientAPI.UpdateUserData(request, OnSuccess, OnError);

        while (isWait) {
            yield return null;
        }

        void OnSuccess(UpdateUserDataResult result) {
            Debug.Log("UpdateUserRewordResult : OnSuccess");
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log("UpdataUserRewordResult : OnError");
            Debug.Log(error.GenerateErrorReport());
            isWait = false;

            // エラーポップアップを生成
        }
    }

    /// <summary>
    /// ゲーム設定データをPlayfabから取得
    /// 取得時ログが発生するのでwhileで取得するまで待機
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetTitleData() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // TODO ネットワークにアクセスできない場合エラーポップアップを生成
        }

        bool isWait = true;

        GetTitleDataRequest request = new GetTitleDataRequest();
        PlayFabClientAPI.GetTitleData(request, OnSuccess, OnError);

        // 上記のメソッド処理が終わるまで条件を満たすので待機
        // OnSuccess内でFalseになったら抜ける
        while (isWait) {
            yield return null;
        }

        void OnSuccess(GetTitleDataResult result) {
            Debug.Log("GetTitleData : Success!");

            loginMessage = result.Data["LoginMessage"];
            Debug.Log(loginMessage);

            rewordCurrencyPoint = int.Parse(result.Data["rewordCurrencyPoint"]);
            subtractCurrencyPoint = int.Parse(result.Data["subtractCurrencyPoint"]);

            // スキルデータ取得
            skillDataList = new List<SkillData>();
            skillDataList = JsonHelper.ListFromJson<SkillData>(result.Data["SkillData"]);

            // 各stringを適宜な型にキャストして配列に変換
            foreach (SkillData skillData in skillDataList) {
                if (skillData.eventTypesString != "") {
                    skillData.eventTypes = skillData.eventTypesString.Split(',').Select(GetEnumTypeFromString<EVENT_TYPE>).ToArray();
                }
                if (skillData.expListString != "") {
                    skillData.expList = skillData.expListString.Split(',').Select(int.Parse).ToArray();
                }
            }

            // itemDataを取得
            itemDataList = new List<ItemData>();
            itemDataList = JsonHelper.ListFromJson<ItemData>(result.Data["ItemData"]);

            // 各stringを適宜な型にキャストして配列に変換
            foreach (ItemData itemData in itemDataList) {
                if (itemData.rarelityString != "") {
                    itemData.rarelities = itemData.rarelityString.Split(',').Select(GetEnumTypeFromString<RARE_TYPE>).ToArray();
                }
                if (itemData.appearanceString != "") {
                    itemData.appearances = itemData.appearanceString.Split(',').Select(int.Parse).ToArray();
                }
                if (itemData.seasonTypeString != "") {
                    itemData.seasonTypes = itemData.seasonTypeString.Split(',').Select(GetEnumTypeFromString<SEASON_WIND_TYPE>).ToArray();
                }
                if (itemData.levelTypeString != "") {
                    itemData.levelTypes = itemData.levelTypeString.Split(',').Select(GetEnumTypeFromString<ENEMY_LEVEL_TYPE>).ToArray();
                }
                if (itemData.levelBonusString != "") {
                    itemData.levelBonuses = itemData.levelBonusString.Split(',').Select(float.Parse).ToArray();
                }
                if (itemData.abilityValueString != "") {
                    itemData.abilityValues = itemData.abilityValueString.Split(',').Select(float.Parse).ToArray();
                }
                if (itemData.abilityNameString != "") {
                    itemData.abilityNames = itemData.abilityNameString.Split(',').ToArray();
                }
                if (itemData.effectString != "") {
                    itemData.effects = itemData.effectString.Split(',').ToArray();
                }
            }

            // EnemyDataを取得
            enemyDataList = new List<EnemyData>();
            enemyDataList = JsonHelper.ListFromJson<EnemyData>(result.Data["EnemyData"]);

            // 各stringを適宜な型にキャストして配列に変換
            foreach (EnemyData enemyData in enemyDataList) {
                enemyData.rarelity = GetEnumTypeFromString<RARE_TYPE>(enemyData.rarelityString);
                enemyData.timeType = GetEnumTypeFromString<TIME_TYPE>(enemyData.timeTypeString);
                enemyData.seasonType = GetEnumTypeFromString<SEASON_WIND_TYPE>(enemyData.seasonTypeString);
                enemyData.levelType = GetEnumTypeFromString<ENEMY_LEVEL_TYPE>(enemyData.lebelTypeString);
                if (enemyData.skillAbilitiesString != "") {
                    enemyData.skillAbilities = enemyData.skillAbilitiesString.Split(',').Select(int.Parse).ToArray();
                }
                if (enemyData.lebelBonusString != "") {
                    enemyData.levelBonus = enemyData.lebelBonusString.Split(',').Select(int.Parse).ToArray();
                }
                if (enemyData.habitatsString != "") {  // Select(x => GetEnumTypeFromString<FIELD_TYPE>(x))が正式
                    enemyData.habitats = enemyData.habitatsString.Split(',').Select(GetEnumTypeFromString<FIELD_TYPE>).ToArray();
                }
            }

            // MemoriaDataを取得
            memoriaDataList = new List<MemoriaData>();
            memoriaDataList = JsonHelper.ListFromJson<MemoriaData>(result.Data["MemoriaData"]);

            // 各stringを適宜な型にキャストして配列に変換
            foreach (MemoriaData memoriaData in memoriaDataList) {
                if (!string.IsNullOrEmpty(memoriaData.skillListString)) {
                    memoriaData.skillList = memoriaData.skillListString.Split(',').Select(int.Parse).ToArray();
                }
                if (!string.IsNullOrEmpty(memoriaData.lebelBonusString)) {
                    memoriaData.levelBonus = memoriaData.lebelBonusString.Split(',').Select(int.Parse).ToArray();
                }
            }


            // 取得完了したのでwhileを抜ける
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log("GetTitleData : Failure...");
            Debug.Log(error.GenerateErrorReport());
            isWait = false;

            // エラーポップアップを生成
        }
    }


    ///***** ゲッターメソッド *****///

    public string GetPlayfabUserDataString(GetUserDataResult result, string key) {
        if (result.Data.ContainsKey(key)) {
            // 取得したDataにkeyが含まれており、Valueがnull以外ならValueを返す
            if (result.Data[key].Value != null) {
                return result.Data[key].Value;
            } else {
                // keyはあるが中身がないなら空白を返す
                return "";
            }
        } else {
            // keyがないなら
            return "";
        }
    }

    public int GetPlayfabUserDataInt(GetUserDataResult result, string key) {
        if (result.Data.ContainsKey(key)) {
            return int.Parse(result.Data[key].Value);
        } else {
            return 0;
        }
    }

    public float GetPlayfabUserDataFloat(GetUserDataResult result, string key) {
        if (result.Data.ContainsKey(key)) {
            return float.Parse(result.Data[key].Value);
        } else {
            return 0;
        }
    }

    public bool GetPlayfabUserDataBoolean(GetUserDataResult result, string key, bool defaultRet = false) {
        if (result.Data.ContainsKey(key)) {
            return bool.Parse(result.Data[key].Value);
        } else {
            return defaultRet;
        }
    }

    /// <summary>
    /// stringとEnumのタイプをもらい、文字列をそのタイプのEnumにして返却
    /// </summary>
    public IEnum GetEnumTypeFromString<IEnum>(string str) where IEnum : struct {
        return (IEnum)Enum.Parse(typeof(IEnum), str, true);
    }
}
