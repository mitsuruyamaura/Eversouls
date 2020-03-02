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
        public int skillNo;       // 所持スキルの番号
        public string skillName;  // 所持スキルの名前
        public int cost;
        public string skillType;
        public int imageNo;
        public int skillAbilityNo;    // スキルの使用回数や効果を適用する際に参照する能力値()
        public string eventTypesString; 
        public EVENT_TYPE[] eventTypes;
        public int amountCount;
    }
    public List<SkillData> skillDatas;

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
    public List<ItemData> itemDatas;

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
            // 取得できない場合の処理
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

                GameData.instance.haveSkillNoList = new List<int>();
                for (int i = 0; i < 6; i++) {
                    GameData.instance.haveSkillNoList.Add(i);
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
                    // ストリング化されたリストがあれば、それを配列に入れてからリストに直す
                    GameData.instance.haveSkillNoList = GameData.instance.haveSkillNoListString.Split(',').Select(int.Parse).ToList();
                }
                // 所持リストにスキルデータの参照を取得
                foreach (SkillData skillData in skillDatas) {
                    for (int i = 0; i < GameData.instance.haveSkillNoList.Count; i++) {
                        if (skillData.skillNo == GameData.instance.haveSkillNoList[i]) {
                            GameData.instance.haveSkillDatas.Add(skillData);
                        }
                    }
                }
                Debug.Log("GetUserDataResult : Success!");
            }
            // 待機を終了
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log(error.GenerateErrorReport());
            isWait = false;
            // ポップアップなどを開く

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
                { "haveSkillNoList", GameData.instance.GetHaveSkillListString()},
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
                { "haveSkillNoList", GameData.instance.GetHaveSkillListString()},
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
        }
    }

    /// <summary>
    /// PlayFabにある、ユーザーの動画リワードの視聴状態を更新
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdataUserReword() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // 取得できない場合の処理
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
        }
    }

    /// <summary>
    /// ゲーム設定データをPlayfabから取得
    /// 取得時ログが発生するのでwhileで取得するまで待機
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetTitleData() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // ネットワークにアクセスできない場合の処理
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
            skillDatas = new List<SkillData>();
            skillDatas = JsonHelper.ListFromJson<SkillData>(result.Data["SkillData"]);
            // stringをEventTypeに変換
            foreach (SkillData skillData in skillDatas) {
                if (skillData.eventTypesString != "") {
                    string[] data = skillData.eventTypesString.Split(',').ToArray();
                    skillData.eventTypes = new EVENT_TYPE[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        skillData.eventTypes[i] = (EVENT_TYPE)Enum.Parse(typeof(EVENT_TYPE), data[i]);
                    }               
                }
            }

            itemDatas = new List<ItemData>();
            itemDatas = JsonHelper.ListFromJson<ItemData>(result.Data["ItemData"]);
            // stringを配列にして、各型に変換
            foreach (ItemData itemData in itemDatas) {
                if (itemData.rarelityString != "") {
                    string[] data = itemData.rarelityString.Split(',').ToArray();
                    itemData.rarelities = new RARE_TYPE[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        itemData.rarelities[i] = (RARE_TYPE)Enum.Parse(typeof(RARE_TYPE), data[i]);
                    }
                }
                if (itemData.appearanceString != "") {
                    string[] data = itemData.appearanceString.Split(',').ToArray();
                    itemData.appearances = new int[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        itemData.appearances[i] = int.Parse(data[i]);
                    }
                }
                if (itemData.seasonTypeString != "") {
                    string[] data = itemData.seasonTypeString.Split(',').ToArray();
                    itemData.seasonTypes = new SEASON_WIND_TYPE[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        itemData.seasonTypes[i] = (SEASON_WIND_TYPE)Enum.Parse(typeof(SEASON_WIND_TYPE), data[i]);
                    }
                }
                if (itemData.levelTypeString != "") {
                    string[] data = itemData.levelTypeString.Split(',').ToArray();
                    itemData.levelTypes = new ENEMY_LEVEL_TYPE[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        itemData.levelTypes[i] = (ENEMY_LEVEL_TYPE)Enum.Parse(typeof(ENEMY_LEVEL_TYPE), data[i]);
                    }
                }
                if (itemData.levelBonusString != "") {
                    string[] data = itemData.levelBonusString.Split(',').ToArray();
                    itemData.levelBonuses = new float[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        itemData.levelBonuses[i] = float.Parse(data[i]);
                    }
                }
                if (itemData.abilityValueString != "") {
                    string[] data = itemData.abilityValueString.Split(',').ToArray();
                    itemData.abilityValues = new float[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        itemData.abilityValues[i] = float.Parse(data[i]);
                    }
                }
                if (itemData.abilityNameString != "") {
                    string[] data = itemData.abilityNameString.Split(',').ToArray();
                    itemData.abilityNames = new string[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        itemData.abilityNames[i] = data[i];
                    }
                }
                if (itemData.effectString != "") {
                    string[] data = itemData.effectString.Split(',').ToArray();
                    itemData.effects = new string[data.Length];
                    for (int i = 0; i < data.Length; i++) {
                        itemData.effects[i] = data[i];
                    }
                }
            }
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log("GetTitleData : Failure...");
            Debug.Log(error.GenerateErrorReport());
            isWait = false;
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
}
