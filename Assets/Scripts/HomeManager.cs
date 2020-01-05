using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HomeManager : MonoBehaviour
{
    public Button btnQuest;
    public Image imgHome;

    void Start(){
        //StartCoroutine(TransitionManager.instance.EnterScene());
        btnQuest.onClick.AddListener(() => StartCoroutine(OnClickQuestScene()));
        StartCoroutine(SetupHomeImage());
    }

    public IEnumerator OnClickQuestScene() {
        Sequence seq = DOTween.Sequence();
        seq.Append(btnQuest.transform.DOScale(1.2f, 0.15f));
        seq.Append(btnQuest.transform.DOScale(1.0f, 0.15f));
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(SceneStateManager.instance.MoveScene(SCENE_TYPE.QUEST));
    }

    private IEnumerator SetupHomeImage() {
        imgHome.transform.DOScale(1.5f, 0.25f);
        yield return new WaitForSeconds(0.25f);
        imgHome.transform.DOScale(1.0f, 1.5f);
    }
}
