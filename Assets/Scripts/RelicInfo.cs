using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelicInfo : MonoBehaviour
{
    public Button btnOpen;
    public RelicPopup relicPopupPrefab;
    public Transform popupTran;
    public Hero hero;

    void Start() {
        btnOpen.onClick.AddListener(OnClickOpenRelicPopup);
    }

    private void OnClickOpenRelicPopup() {
        SoundManager.Instance.PlaySE(SoundManager.ENUM_SE.BTN_OK);
        RelicPopup relicPopup = Instantiate(relicPopupPrefab, popupTran, false);
        relicPopup.CreateHeroActionInfos(hero);
    }
}
