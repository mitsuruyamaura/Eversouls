using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager : MonoBehaviour
{
    public static SceneStateManager instance;

    private void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public IEnumerator MoveScene(SCENE_TYPE sceneType, float waitTime) {
        // Title => Homeのときはページのトランジションを通さない
        if (sceneType != SCENE_TYPE.HOME) {
            StartCoroutine(TransitionManager.instance.ExitScene());
        }
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneType.ToString());
    }
}
