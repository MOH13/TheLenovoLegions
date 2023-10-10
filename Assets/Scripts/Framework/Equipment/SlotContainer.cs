using System;
using UnityEngine;

namespace LL.Framework.Equipment
{
    [Serializable]
    public struct SlotEntry<TSlot> where TSlot : IEquipmentSlot
    {
        [field: SerializeField]
        public TSlot Slot { get; private set; }
        // [field: SerializeField]
        // public float BaseValue { get; private set; }
    }

    public interface ISlotContainer<TSlot> where TSlot : IEquipmentSlot
    {
        public ReadOnlySpan<SlotEntry<TSlot>> GetSlots();

        public bool TryGetSlot(TSlot stat, out SlotEntry<TSlot> entry);
    }
}
