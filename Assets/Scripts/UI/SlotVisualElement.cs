using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using LL.Game.Equipment;
using UnityEngine;
using UnityEngine.UIElements;

namespace LL.UI
{
    public class SlotVisualElement : VisualElement
    {
        public static string SLOT_BASE_CLASS = "slot-visual-element";

        private EquipmentSlotResource? slot;

        private Label label;
        private EquipmentVisualElement equipmentElement;

        public EquipmentSlotResource? Slot => slot;

        public EquipmentResource? Equipment => equipmentElement.Equipment;

        public SlotVisualElement() : this(null, null) { }

        public SlotVisualElement(EquipmentSlotResource? slot, EquipmentResource? equipment)
        {
            this.slot = slot;

            this.label = new Label();
            this.equipmentElement = new EquipmentVisualElement(equipment);

            this.AddToClassList(SLOT_BASE_CLASS);
            this.Add(this.label);
            this.Add(this.equipmentElement);

            DoSetup();
        }

        public void DoSetup()
        {
            this.label.text = slot != null ? slot.SlotName : "NOSLOT";
        }

        public void SetEquipment(EquipmentResource? equipment)
        {
            this.equipmentElement.SetEquipment(equipment);
        }

        public void SetSlot(EquipmentSlotResource slot)
        {
            if (this.slot == slot) return;
            this.slot = slot;
            DoSetup();
        }

        public void SetHighlight(bool enabled)
        {
            this.equipmentElement.SetHighlight(enabled);
        }

        public new class UxmlFactory : UxmlFactory<SlotVisualElement, UxmlTraits> { }

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

