using UnityEngine;

public class GameBase : MonoBehaviour
{
    public PlayFabManager playFabManager;

    void Start() {
        // クローンではない状態でインスタンスする
        Instantiate(playFabManager).name = "PlayFabManager";
    }
}
