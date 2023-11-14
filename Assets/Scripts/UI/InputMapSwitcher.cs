using System.Collections;
using System.Collections.Generic;
using LL.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputMapSwitcher : MonoBehaviour
{
    [SerializeField]
    PlayerInput? input;

    [SerializeField]
    List<UIDocument> documentsToWatch = new();

    [SerializeField]
    string defaultActionMapName = "Player";

    [SerializeField]
    string uiActionMapName = "UI";

    // Update is called once per frame
    void LateUpdate()
    {
        bool shouldUseUI = false;

        foreach (var go in documentsToWatch)
        {
            shouldUseUI |= go.isActiveAndEnabled;
        }

        input?.SwitchCurrentActionMap(shouldUseUI ? uiActionMapName : defaultActionMapName);
    }
}
