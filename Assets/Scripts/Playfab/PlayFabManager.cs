using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Linq;

public class PlayFabManager : MonoBehaviour
{
    public static PlayFabManager instance;

    string loginMessage;

    // PlayFab
    public bool isLogin;
    public bool isError;
    public string conErrorReport;

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

    void Start() {
        ConnectPlayfab();       
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

        StartCoroutine(GetTitleData());
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
    /// ゲーム設定データをPlayfabから取得
    /// 取得時ログが発生するのでwhileで取得するまで待機
    /// </summary>
    /// <returns></returns>
    public IEnumerator GetTitleData() {
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            // 取得できない場合の処理
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

            GachaMaster[] gachaMaster = Utf8Json.JsonSerializer.Deserialize<GachaMaster[]>(result.Data["GachaMaster"]);
            foreach (var master in gachaMaster) {
                Debug.Log(master.Name);
            }
            isWait = false;
        }

        void OnError(PlayFabError error) {
            Debug.Log("GetTitleData : Failure...");
            Debug.Log(error.GenerateErrorReport());
            isWait = false;
        }
    }
}
