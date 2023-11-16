using UnityEngine;
using LL.Framework.Equipment;

namespace LL.Game.Equipment
{
    [CreateAssetMenu(fileName = "NewSlot", menuName = "Equipment/Slot", order = 1)]
    public class EquipmentSlotResource : ScriptableObject, IEquipmentSlot
    {
        [field: SerializeField]
        public string SlotName { get; private set; } = "NewSlot";
    }
}
