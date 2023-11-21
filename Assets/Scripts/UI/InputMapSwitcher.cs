using System.Collections;
using System.Collections.Generic;
using LL.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputMapSwitcher : MonoBehaviour
{
    [SerializeField]
    CatBehaviour cat;

    [SerializeField]
    List<UIDocument> documentsToWatch = new();

    // Update is called once per frame
    void LateUpdate()
    {
        bool uiEnabled = false;

        foreach (var go in documentsToWatch)
        {
            uiEnabled |= go.isActiveAndEnabled;
        }

        cat.DisableInput = uiEnabled;
    }
}
