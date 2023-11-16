using System;
using LL.Framework.Stats;

namespace LL.Framework.Equipment
{
    public interface IEquipment<TStat, TEffect, TSlot> where TStat : IStat where TEffect : IEffect<TStat> where TSlot : IEquipmentSlot
    {
        public string EquipmentName { get; }
        public TSlot Slot { get; }
        public ReadOnlySpan<TEffect> Effects { get; }
    }
}
