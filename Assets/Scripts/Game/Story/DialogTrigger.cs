using System.Collections;
using System.Collections.Generic;
using LL.UI.Dialog;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField]
    DialogUIBehavior dialogUI;

    [SerializeField]
    LayerMask layerMask;

    [SerializeField]
    DialogResource dialog;

    [SerializeField]
    bool onlyShowOnce = true;

    void OnTriggerEnter2D(Collider2D col)
    {
        if ((col.gameObject.layer | layerMask) != 0)
        {
            dialogUI.SetDialog(dialog);
        }
        if (onlyShowOnce)
        {
            gameObject.SetActive(false);
        }
    }
}
