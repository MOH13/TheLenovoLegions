using System.Collections.Generic;
using System.Text;
using LL.Framework.Stats;
using LL.Game.Equipment;
using LL.Game.Stats;
using UnityEngine.UIElements;

namespace LL.UI
{
    public class EquipmentVisualElement : VisualElement
    {
        public const string EQUIPMENT_BASE_CLASS = "equipment-visual-element";
        public const string EQUIPMENT_NONEMPTY_CLASS = "equipment-visual-element-nonempty";
        public const string EQUIPMENT_EMPTY_CLASS = "equipment-visual-element-empty";
        public const string EQUIPMENT_ICON_CLASS = "equipment-icon";
        public const string EQUIPMENT_HIGHLIT_CLASS = "equipment-highlit";

        private Image icon;

        public EquipmentResource? Equipment { get; private set; }

        public EquipmentVisualElement() : this(null) { }

        public EquipmentVisualElement(EquipmentResource? equipment)
        {
            this.focusable = true;
            this.AddToClassList(EQUIPMENT_BASE_CLASS);
            this.Equipment = equipment;
            this.icon = new Image();
            icon.AddToClassList(EQUIPMENT_ICON_CLASS);
            this.Add(icon);
            this.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
            this.RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
            DoSetup();
        }

        private void OnPointerEnter(PointerEnterEvent evt)
        {
            if (Equipment == null) return;
            string tooltip = $"<b>{Equipment.EquipmentName}</b>";
            foreach (var effect in Equipment.Effects)
            {
                tooltip += $"\n{EffectUtils.Describe<StatResource, EffectResource>(effect)}";
            }
            using (var tooltipEvt = ShowTooltipEvent.GetPooled(this, tooltip, evt.pointerId, evt.position))
            {
                this.SendEvent(tooltipEvt);
            }
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            if (Equipment == null) return;
            HideTooltip();
        }

        private void HideTooltip()
        {
            using (var tooltipEvt = HideTooltipEvent.GetPooled(this))
            {
                this.SendEvent(tooltipEvt);
            }
        }

        public void DoSetup()
        {
            this.icon.image = this.Equipment == null ? null : this.Equipment.Icon;
            if (this.Equipment != null)
            {
                this.RemoveFromClassList(EQUIPMENT_EMPTY_CLASS);
                this.AddToClassList(EQUIPMENT_NONEMPTY_CLASS);
            }
            else
            {
                this.RemoveFromClassList(EQUIPMENT_NONEMPTY_CLASS);
                this.AddToClassList(EQUIPMENT_EMPTY_CLASS);
            }
        }

        public void SetEquipment(EquipmentResource? equipment)
        {
            if (this.Equipment == equipment) return;
            this.Equipment = equipment;
            HideTooltip();
            DoSetup();
        }

        public void SetHighlight(bool enabled)
        {
            if (enabled)
            {
                this.AddToClassList(EQUIPMENT_HIGHLIT_CLASS);
            }
            else
            {
                this.RemoveFromClassList(EQUIPMENT_HIGHLIT_CLASS);
            }
        }

        public new class UxmlFactory : UxmlFactory<EquipmentVisualElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
            }
        }
    }
}

