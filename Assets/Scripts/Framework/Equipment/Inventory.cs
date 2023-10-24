using System;
using System.Collections.Generic;
using LL.Framework.Stats;
using LL.Game.Equipment;
using UnityEngine;

namespace LL.Framework.Equipment
{
    public interface IInventory<TStat, TEffect, TSlot, TEquipment, TLiveEquipment>
    : ILiveEquipment<TStat, TEffect, TSlot, TEquipment>
    where TStat : IStat
    where TEffect : IEffect<TStat>
    where TSlot : IEquipmentSlot
    where TEquipment : IEquipment<TStat, TEffect, TSlot>
    where TLiveEquipment : ILiveEquipment<TStat, TEffect, TSlot, TEquipment>
    {
        bool AddToInventory(TEquipment piece);

        EquipmentCollectionView<TEquipment> GetView();
    }

    public struct EquipmentCollection<TEquipment>
    {
        private List<TEquipment>? list;

        private HashSet<TEquipment>? set;

        private List<TEquipment> List { get { list ??= new(); return list; } }

        private HashSet<TEquipment> Set { get { set ??= new(); return set; } }

        public bool AddPiece(TEquipment piece)
        {
            if (!Set.Add(piece)) return false;

            List.Add(piece);

            return true;
        }

        public bool RemovePiece(TEquipment piece)
        {
            if (!Set.Remove(piece)) return false;

            List.Remove(piece);

            return true;
        }

        public bool Contains(TEquipment piece) => Set.Contains(piece);

        public List<TEquipment>.Enumerator GetEnumerator() => List.GetEnumerator();

        public EquipmentCollectionView<TEquipment> GetView() => new(this);
    }

    public struct EquipmentCollectionView<TEquipment>
    {
        private EquipmentCollection<TEquipment> equipmentCollection;

        public EquipmentCollectionView(EquipmentCollection<TEquipment> equipmentCollection)
        {
            this.equipmentCollection = equipmentCollection;
        }

        public List<TEquipment>.Enumerator GetEnumerator() => equipmentCollection.GetEnumerator();
    }

    [Serializable]
    public struct Inventory<TStat, TEffect, TSlot, TEquipment, TLiveEquipment> : IInventory<TStat, TEffect, TSlot, TEquipment, TLiveEquipment>
    where TStat : IStat
    where TEffect : IEffect<TStat>
    where TSlot : IEquipmentSlot
    where TEquipment : IEquipment<TStat, TEffect, TSlot>
    where TLiveEquipment : ILiveEquipment<TStat, TEffect, TSlot, TEquipment>
    {
        [field: SerializeField]
        public TLiveEquipment CurrentEquipment { get; private set; }

        private EquipmentCollection<TEquipment> collection;

        public TEquipment? Equip(TEquipment piece)
        {
            if (!collection.RemovePiece(piece))
            {
                throw new ArgumentException($"Equipment '{piece.EquipmentName}' was not in inventory.");
            }
            var previous = CurrentEquipment.Equip(piece);
            if (previous != null)
            {
                collection.AddPiece(previous);
            }
            return previous;
        }

        public TEquipment? EquipAtIndex(TEquipment piece, int i)
        {
            if (!collection.RemovePiece(piece))
            {
                throw new ArgumentException($"Equipment '{piece.EquipmentName}' was not in inventory.");
            }
            ;
            var previous = CurrentEquipment.EquipAtIndex(piece, i);
            if (previous != null)
            {
                collection.AddPiece(previous);
            }
            return previous;
        }

        public bool EquipNoReplace(TEquipment piece)
        {
            if (!collection.Contains(piece))
            {
                throw new ArgumentException($"Equipment '{piece.EquipmentName}' was not in inventory.");
            }
            var success = CurrentEquipment.EquipNoReplace(piece);
            if (success)
            {
                collection.RemovePiece(piece);
            }
            return success;
        }

        public bool Unequip(TEquipment piece)
        {
            var success = CurrentEquipment.Unequip(piece);
            if (success)
            {
                collection.AddPiece(piece);
            }
            return success;
        }

        public TEquipment? UnequipAtIndex(int i)
        {
            var piece = CurrentEquipment.UnequipAtIndex(i);
            if (piece != null)
            {
                collection.AddPiece(piece);
            }
            return piece;
        }

        public bool AddToInventory(TEquipment piece) => collection.AddPiece(piece);

        public TEquipment? GetAtIndex(int i) => CurrentEquipment.GetAtIndex(i);

        public EquipmentCollectionView<TEquipment> GetView() => collection.GetView();
    }
}
