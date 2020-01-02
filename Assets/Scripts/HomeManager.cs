using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class HomeManager : MonoBehaviour
{
    public Button btnQuest;

    void Start(){
        StartCoroutine(TransitionManager.instance.EnterScene());
        btnQuest.onClick.AddListener(() => StartCoroutine(OnClickQuestScene()));
    }

    public IEnumerator OnClickQuestScene() {
        Sequence seq = DOTween.Sequence();
        seq.Append(btnQuest.transform.DOScale(1.2f, 0.15f));
        seq.Append(btnQuest.transform.DOScale(1.0f, 0.15f));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SceneStateManager.instance.MoveScene(SCENE_TYPE.QUEST));
    }
}
