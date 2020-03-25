using UnityEngine;

public class GameBase : MonoBehaviour
{
    public PlayFabManager playFabManager;
    public GameData gameData;
    public SceneStateManager sceneStateManager;
    public TransitionManager transitionManager;
    public SoundManager soundManager;
    public TapEffect tapEffect;

    void Awake() {
        // インスタンスして名前を変える
        Instantiate(gameData).name = "GameData";
        Instantiate(playFabManager).name = "PlayFabManager";       
        Instantiate(soundManager).name = "SoundManager";
        Instantiate(transitionManager).name = "TransitionCanvas";
        Instantiate(sceneStateManager).name = "SceneStateManager";
        Instantiate(tapEffect).name = "Camera_TapEffect";
    }
}
