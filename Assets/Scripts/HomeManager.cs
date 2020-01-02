using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public Button btnQuest;

    void Start(){
        StartCoroutine(TransitionManager.instance.EnterScene());
        btnQuest.onClick.AddListener(() => StartCoroutine(SceneStateManager.instance.MoveScene(SCENE_TYPE.QUEST)));
    }
}
