using System;
using LL.Framework.Stats;
using UnityEngine;

namespace LL.Framework.Equipment
{
    public interface ILiveEquipment<TStat, TEffect, TSlot, TEquipment>
    where TStat : IStat
    where TEffect : IEffect<TStat>
    where TSlot : IEquipmentSlot
    where TEquipment : IEquipment<TStat, TEffect, TSlot>
    {
        public TEquipment? Equip(TEquipment piece);
        public TEquipment? EquipAtIndex(TEquipment piece, int i);
        public bool EquipNoReplace(TEquipment piece);
        public bool Unequip(TEquipment piece);

        public TEquipment? UnequipAtIndex(int i);
        public TEquipment? GetAtIndex(int i);

    }

    [Serializable]
    public struct LiveEquipment<TStat, TEffect, TSlot, TEquipment, TContainer> : ILiveEquipment<TStat, TEffect, TSlot, TEquipment>
    where TStat : IStat
    where TEffect : IEffect<TStat>
    where TSlot : IEquipmentSlot
    where TEquipment : IEquipment<TStat, TEffect, TSlot>
    where TContainer : ISlotContainer<TSlot>
    {
        [field: SerializeField]
        public TContainer Container { get; private set; }

        private TEquipment?[]? equipment;

        private TEquipment?[] Equipment
        {
            get
            {
                equipment ??= new TEquipment[Container.GetSlots().Length];
                return equipment;
            }
        }

        public TEquipment? Equip(TEquipment piece)
        {
            var slots = Container.GetSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i].Slot;
                if (slot.SlotName == piece.Slot.SlotName)
                {
                    var previous = Equipment[i];
                    Equipment[i] = piece;
                    return previous;
                }
            }
            throw new ArgumentException($"Had no slot for equipment '{piece.EquipmentName}' of slot type '{piece.Slot.SlotName}'");
        }

        public TEquipment? EquipAtIndex(TEquipment piece, int i)
        {
            var actualSlot = Container.GetSlots()[i].Slot;

            if (actualSlot.SlotName != piece.Slot.SlotName)
                throw new ArgumentException($"Tried to equip piece '{piece.EquipmentName}' of slot type '{piece.Slot}' in slot {i}/'{actualSlot.SlotName}'");

            var previous = Equipment[i];
            Equipment[i] = piece;
            return previous;
        }

        public bool EquipNoReplace(TEquipment piece)
        {
            var slots = Container.GetSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                var slot = slots[i].Slot;
                if (slot.SlotName == piece.Slot.SlotName)
                {
                    if (Equipment[i] == null)
                    {
                        Equipment[i] = piece;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Unequip(TEquipment piece)
        {
            var slots = Container.GetSlots();
            for (int i = 0; i < slots.Length; i++)
            {
                var equipment = Equipment;
                var previous = Equipment[i];
                if (previous != null && previous.EquipmentName == piece.EquipmentName)
                {
                    equipment[i] = default;
                    return true;
                }
            }
            return false;
        }

        public TEquipment? UnequipAtIndex(int i)
        {
            var previous = Equipment[i];
            Equipment[i] = default;
            return previous;
        }

        public TEquipment? GetAtIndex(int i)
        {
            return Equipment[i];
        }
    }
}
