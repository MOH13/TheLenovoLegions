using UnityEngine;
using LL.Game.Stats;
using LL.Framework.Equipment;

namespace LL.Game.Equipment
{
    public class InventoryBehavior : MonoBehaviour, IInventory<StatResource, EffectResource, EquipmentSlotResource, EquipmentResource, LiveEquipmentBehavior>
    {
        public SlotContainerResource Container => inventory.CurrentEquipment.Container;

        [SerializeField]
        Inventory<StatResource, EffectResource, EquipmentSlotResource, EquipmentResource, LiveEquipmentBehavior> inventory;

        public EquipmentResource? Equip(EquipmentResource piece) => inventory.Equip(piece);

        public bool EquipNoReplace(EquipmentResource piece) => inventory.EquipNoReplace(piece);

        public EquipmentResource? EquipAtIndex(EquipmentResource piece, int i) => inventory.EquipAtIndex(piece, i);

        public bool Unequip(EquipmentResource piece) => inventory.Unequip(piece);

        public EquipmentResource? UnequipAtIndex(int i) => inventory.UnequipAtIndex(i);

        public EquipmentResource? GetAtIndex(int i) => inventory.GetAtIndex(i);

        public bool AddToInventory(EquipmentResource piece) => inventory.AddToInventory(piece);

        public EquipmentCollectionView<EquipmentResource> GetView() => inventory.GetView();
    }
}
