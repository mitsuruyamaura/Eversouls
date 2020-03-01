using UnityEngine;

public class ObjectCencerArea : MonoBehaviour
{
    public QuestSelectPopup questSelectPopup;

    private void OnTriggerEnter2D(Collider2D col) {
        // 選択中のQuestInfoを渡す
        questSelectPopup.DisplaySelectQuestInfo(col.gameObject.GetComponent<QuestInfo>());
    }
}
