using UnityEngine;

public class ObjectCencerArea : MonoBehaviour
{
    public QuestSelectPopup questSelectPopup;

    private void OnTriggerEnter2D(Collider2D col) {
        questSelectPopup.selectQuestData = col.gameObject.GetComponent<QuestData>();
        Debug.Log(col.gameObject);
    }
}
