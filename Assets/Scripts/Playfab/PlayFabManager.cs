using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;

public class PlayFabManager : MonoBehaviour {
    public static PlayFabManager instance;

    string loginMessage;

    // PlayFab
    public bool isLogin;
    public bool isError;
    public string conErrorReport;

    [System.Serializable]
    public class PlayerStatus{
        public int level;
        public int exp;
        public int skillPoint;
        public int maxHp;
        public int maxSp;
        public int physical;      // 武術
        public int mental;        // 魔術
        public int technical;     // 技術
        public int actionPoint;   // 行動力
        public int response;      // 反応
        public int search;        // 探索
        public List<int> haveSkills;   // 所持スキルの番号
        public List<string> haveSkillsName;  // 所持スキルの名前
    }


    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }


    [System.Serializable]
    public class GachaMaster {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Rank { get; set; }
        public int Rate { get; set; }
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
    /// ゲームに必要な情報をPlayFabへアクセスして順番に取得する
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
    /// ユーザーデータをPlayFabから取得
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
            Debug.Log("GetUserDataResult : Success!");
            GameData.instance.isFirstAccess = GetPlayfabUserDataBoolean(result, "isFirstAccess");
            if (!GameData.instance.isFirstAccess) {
                // 初期設定を行う
                GameData.instance.isFirstAccess = true;
                GameData.instance.homeBgmType = SoundManager.ENUM_BGM.HOME_1;
                GameData.instance.volumeBGM = 0.5f;
                GameData.instance.volumeSE = 0.5f;
                // PlayFabを更新
                StartCoroutine(SetupUserDatas());
            } else {
                GameData.instance.homeBgmType = (SoundManager.ENUM_BGM)Enum.Parse(typeof(SoundManager.ENUM_BGM), GetPlayfabUserDataString(result, "homeBGM"));
                GameData.instance.volumeBGM = GetPlayfabUserDataFloat(result, "volumeBGM");
                GameData.instance.volumeSE = GetPlayfabUserDataFloat(result, "volumeSE");
            }
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log(error.GenerateErrorReport());
            isWait = false;
            // ポップアップなどを開く

        }
    }

    /// <summary>
    /// 初回起動時の初期値を更新
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetupUserDatas() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // 取得できない場合の処理
        }

        bool isWait = true;

        // 保存したいデータをStringでリクエストを作る
        UpdateUserDataRequest request = new UpdateUserDataRequest() {
            Data = new Dictionary<string, string> {
                { "isFirstAccess", GameData.instance.isFirstAccess.ToString()},
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
    /// オプション画面のデータをPlayFabに保存
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

            //GachaMaster[] gachaMaster = Utf8Json.JsonSerializer.Deserialize<GachaMaster[]>(result.Data["GachaMaster"]);
            //foreach (var master in gachaMaster) {
            //    Debug.Log(master.Name);
            //}
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log("GetTitleData : Failure...");
            Debug.Log(error.GenerateErrorReport());
            isWait = false;
        }
    }

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
