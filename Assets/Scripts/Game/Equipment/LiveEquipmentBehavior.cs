using UnityEngine;
using LL.Game.Stats;
using LL.Framework.Equipment;

namespace LL.Game.Equipment
{
    public class LiveEquipmentBehavior : MonoBehaviour, ILiveEquipment<StatResource, EffectResource, EquipmentSlotResource, EquipmentResource>
    {
        [SerializeField]
        LiveEquipment<StatResource, EffectResource, EquipmentSlotResource, EquipmentResource, SlotContainerResource> equipment = new();

        [SerializeField]
        LiveStatsBehavior stats;

        public SlotContainerResource Container => equipment.Container;

        private void ApplyEffects(EquipmentResource piece)
        {
            foreach (var effect in piece.Effects)
            {
                stats.ApplyEffect(effect);
            }
        }

        private void RemoveEffects(EquipmentResource piece)
        {
            foreach (var effect in piece.Effects)
            {
                stats.RemoveEffect(effect);
            }
        }

        public EquipmentResource? Equip(EquipmentResource piece)
        {
            var previous = equipment.Equip(piece);

            ApplyEffects(piece);
            if (previous != null)
                RemoveEffects(previous);

            return previous;
        }

        public bool EquipNoReplace(EquipmentResource piece)
        {
            var wasAdded = equipment.EquipNoReplace(piece);
            if (wasAdded)
            {
                ApplyEffects(piece);
            }
            return wasAdded;
        }

        public EquipmentResource? EquipAtIndex(EquipmentResource piece, int i)
        {
            var previous = equipment.EquipAtIndex(piece, i);

            ApplyEffects(piece);
            if (previous != null)
                RemoveEffects(previous);

            return previous;
        }

        public bool Unequip(EquipmentResource piece)
        {
            var wasRemoved = equipment.Unequip(piece);
            if (wasRemoved)
            {
                RemoveEffects(piece);
            }
            return wasRemoved;
        }

        public EquipmentResource? UnequipAtIndex(int i)
        {
            var piece = equipment.UnequipAtIndex(i);

            if (piece != null)
                RemoveEffects(piece);

            return piece;
        }

        public EquipmentResource? GetAtIndex(int i)
        {
            return equipment.GetAtIndex(i);
        }
    }
}
