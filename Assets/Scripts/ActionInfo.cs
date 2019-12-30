using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionInfo : MonoBehaviour
{
    public int cost;
    public TMP_Text txtActionName;
    public TMP_Text txtActionInfo;
    public TMP_Text txtCost;
    public TMP_Text txtAmountCount;
    public Image imgCharaIcon;
    public Image imgMainAction;
    public FIELD_TYPE fieldType;
    public ACTION_TYPE actionType;
    public Button btnSubmit;
    public QuestManager questManager;
    public bool isSubmit;

    private void Start() {
        btnSubmit.onClick.AddListener(OnClickSubmit);
    }

    public void InitField(FieldDataList.FieldData data) {
        fieldType = data.fieldType;
        cost = data.cost;
        imgMainAction.sprite = Resources.Load<Sprite>("Actions/" + data.imageNo);

        // TODO 行動の詳細や名称などを表示する
    }

    public void InitAction(ActionDataList.ActionData data) {
        actionType = data.actionType;
        cost = data.cost;
        imgMainAction.sprite = Resources.Load<Sprite>("Actions/" + data.imageNo);
        imgCharaIcon.sprite = Resources.Load<Sprite>("Charas/" + GameData.instance.charas[0].imageNo);
    }

    public void OnClickSubmit() {
        if (!isSubmit) {
            isSubmit = true;
            GameData.instance.ap -= cost;
            questManager.DestroyActions();
        }
    }
}
