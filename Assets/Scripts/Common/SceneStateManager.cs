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

    public IEnumerator MoveScene(SCENE_TYPE sceneType) {
        StartCoroutine(TransitionManager.instance.ExitScene());
        yield return new WaitForSeconds(0.7f);
        SceneManager.LoadScene(sceneType.ToString());
    }

    public IEnumerator MoveHome(SCENE_TYPE sceneType) {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(sceneType.ToString());
    }
}
