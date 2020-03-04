using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Test : MonoBehaviour
{
    public ScrollRect scrollRect;

    void Start()
    {
        scrollRect.DOHorizontalNormalizedPos(1f, 2.0f);
    }
}
