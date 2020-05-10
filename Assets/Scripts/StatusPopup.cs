using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusPopup : PopupBase
{
    HomeManager homeManager;

    public void Setup(HomeManager homeManager) {
        this.homeManager = homeManager;
    }

    public override void OnClickClosePopup() {
        // 再度設定ボタンを押せるようにする
        homeManager.swipeMoveObject.isWindowOpen = false;
        homeManager.isClickable = false;

        base.OnClickClosePopup();
    }
}
