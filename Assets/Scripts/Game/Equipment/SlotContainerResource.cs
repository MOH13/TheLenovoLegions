using System;
using UnityEngine;
using LL.Framework.Equipment;

namespace LL.Game.Equipment
{
    [CreateAssetMenu(fileName = "SlotContainer", menuName = "Equipment/Slot Container", order = 3)]
    public class SlotContainerResource : ScriptableObject, ISlotContainer<EquipmentSlotResource>
    {
        [SerializeField]
        private SlotEntry<EquipmentSlotResource>[] slotEntries = new SlotEntry<EquipmentSlotResource>[0];

        public ReadOnlySpan<SlotEntry<EquipmentSlotResource>> GetSlots()
        {
            return slotEntries;
        }

        public bool TryGetSlot(EquipmentSlotResource slot, out SlotEntry<EquipmentSlotResource> entry)
        {
            foreach (var e in slotEntries)
            {
                if (e.Slot == slot)
                {
                    entry = e;
                    return true;
                }
            }
            entry = default;
            return false;
        }
    }
}

