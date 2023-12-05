using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{
    private UIDocument _document;
    private VisualElement _health;

    private void Start()
    {
        _document = GetComponent<UIDocument>();
        _health = _document.rootVisualElement.Q<VisualElement>("Health");
    }

    public void SetHealth(float healthFraction)
    {

        _health.style.width = Length.Percent(healthFraction * 100);
        //_health.style.width = new Length(_health.style.maxWidth.value.value * healthFraction);

    }

    public void SetMaxHealth()
    {
        _health.style.width = _health.style.maxWidth;
    }
}
