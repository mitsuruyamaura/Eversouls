using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusPopup : PopupBase
{
    HomeManager _homeManager;

    public void Setup(HomeManager homeManager) {
        _homeManager = homeManager;
    }

    public override void OnClickClosePopup() {
        // 再度設定ボタンを押せるようにする
        _homeManager.isClickable = false;

        base.OnClickClosePopup();
    }
}
