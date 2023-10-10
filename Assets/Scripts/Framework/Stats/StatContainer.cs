using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace LL.Framework.Stats
{
    [Serializable]
    public struct StatEntry<TStat> where TStat : IStat
    {
        [SerializeField, FormerlySerializedAs("<Stat>k__BackingField")]
        private TStat? stat;

        public TStat Stat
        {
            get
            {
                if (stat == null)
                {
                    throw new NullReferenceException("Stat is uninitialized");
                }
                return stat;
            }
        }
        [field: SerializeField]
        public float BaseValue { get; private set; }
    }

    public interface IStatContainer<TStat> where TStat : IStat
    {
        public ReadOnlySpan<StatEntry<TStat>> GetStats();

        public bool TryGetStat(TStat stat, out StatEntry<TStat> entry);
    }

    // [Serializable]
    // public struct StatContainer<TStat> : IStatContainer<TStat> where TStat : IStat
    // {
    //     [SerializeField]
    //     private List<StatEntry<TStat>> statEntries;

    //     public StatContainer(bool dummy)
    //     {
    //         statEntries = new();
    //     }

    //     public ReadOnlyCollection<StatEntry<TStat>> GetStats()
    //     {
    //         return statEntries.AsReadOnly();
    //     }

    //     public bool TryGetStat(TStat stat, out StatEntry<TStat> entry)
    //     {
    //         foreach (var e in statEntries)
    //         {
    //             if (e.Stat.StatName == stat.StatName)
    //             {
    //                 entry = e;
    //                 return true;
    //             }
    //         }
    //         entry = new StatEntry<TStat>();
    //         return false;
    //     }
    // }
}
