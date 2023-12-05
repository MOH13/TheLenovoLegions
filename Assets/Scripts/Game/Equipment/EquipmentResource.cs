using System;
using LL.Framework.Equipment;
using LL.Game.Stats;
using UnityEngine;

namespace LL.Game.Equipment
{
    [CreateAssetMenu(fileName = "Equipment", menuName = "Equipment/Equipment", order = 2)]
    public class EquipmentResource : ScriptableObject, IEquipment<StatResource, EffectResource, EquipmentSlotResource>
    {
        [field: SerializeField]
        public string EquipmentName { get; private set; } = "NewEquipment";

        [field: SerializeField, TextArea]
        public string PickupText { get; private set; } = "";

        [field: SerializeField]
        public Sprite? Icon { get; private set; }

        [SerializeField]
        private EquipmentSlotResource? slot;

        public EquipmentSlotResource Slot
        {
            get
            {
                if (slot == null)
                {
                    throw new NullReferenceException("Slot is uninitialized");
                }
                return slot;
            }
        }

        [SerializeField]
        private EffectResource[] effects = new EffectResource[0];

        public ReadOnlySpan<EffectResource> Effects => effects; // TODO handle nullability
    }
}
