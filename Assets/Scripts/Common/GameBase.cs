using UnityEngine;

public class GameBase : MonoBehaviour
{
    public PlayFabManager playFabManager;

    void Awake() {
        // クローンではない状態でインスタンスする
        Instantiate(playFabManager).name = "PlayFabManager";
    }
}
