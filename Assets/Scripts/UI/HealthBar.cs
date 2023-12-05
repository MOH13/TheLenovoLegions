using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HealthBar : MonoBehaviour
{

    [SerializeField] CatBehaviour _catBehaviour;
    private UIDocument _document;
    private VisualElement _health;

    private void Start()
    {
        _document = GetComponent<UIDocument>();
        _health = _document.rootVisualElement.Q<VisualElement>("Health");
        _catBehaviour.OnHit += SetHealth; 
    }

    public void SetHealth(object sender, EventArgs e)
    {
        var healthFraction = _catBehaviour.health / _catBehaviour.stats.GetValue(_catBehaviour.vitality);
        _health.style.width = Length.Percent( healthFraction * 100);
    }

    public void SetMaxHealth()
    {
        _health.style.width = _health.style.maxWidth;
    }
}
