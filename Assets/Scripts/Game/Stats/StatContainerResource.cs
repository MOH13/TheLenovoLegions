using System;
using UnityEngine;
using LL.Framework.Stats;

namespace LL.Game.Stats
{
    [CreateAssetMenu(fileName = "StatContainer", menuName = "Stats/Stat Container", order = 3)]
    public class StatContainerResource : ScriptableObject, IStatContainer<StatResource>
    {
        [SerializeField]
        private StatEntry<StatResource>[] statEntries = new StatEntry<StatResource>[0];

        public ReadOnlySpan<StatEntry<StatResource>> GetStats()
        {
            return statEntries;
        }

        public bool TryGetStat(StatResource stat, out StatEntry<StatResource> entry)
        {
            foreach (var e in statEntries)
            {
                if (e.Stat == stat)
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

