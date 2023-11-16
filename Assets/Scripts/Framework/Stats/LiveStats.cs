using System;
using System.Collections.Generic;
using UnityEngine;

namespace LL.Framework.Stats
{
    public interface ILiveStats<TStat, TEffect>
    where TStat : IStat
    where TEffect : IEffect<TStat>
    {
        public bool ApplyEffect(TEffect effect);
        public bool RemoveEffect(TEffect effect);
        public float GetValue(TStat stat);
    }

    [Serializable]
    public struct LiveStats<TStat, TContainer, TEffect> : ILiveStats<TStat, TEffect>
    where TStat : IStat
    where TContainer : IStatContainer<TStat>
    where TEffect : IEffect<TStat>
    {
        [field: SerializeField]
        public TContainer Container { get; private set; }

        private EffectCollection<TStat, TEffect> effects;

        private Dictionary<TStat, float>? values;

        private Dictionary<TStat, float> Values
        {
            get
            {
                values ??= new();
                return values;
            }
        }

        public bool ApplyEffect(TEffect effect)
        {
            var wasApplied = effects.ApplyEffect(effect);
            if (wasApplied)
            {
                RecalculateStat(effect.Stat);
            }
            return wasApplied;
        }

        public bool RemoveEffect(TEffect effect)
        {
            var wasRemoved = effects.RemoveEffect(effect);
            if (wasRemoved)
            {
                RecalculateStat(effect.Stat);
            }
            return wasRemoved;
        }

        public void RecalculateStats()
        {
            foreach (var stat in Container.GetStats())
            {
                RecalculateStat(stat);
            }
        }

        private void RecalculateStat(TStat stat)
        {
            if (Container.TryGetStat(stat, out var entry))
            {
                RecalculateStat(entry);
            }
            else
            {
                throw new ArgumentException("Invalid stat '" + stat.StatName + "'");
            }
        }

        private void RecalculateStat(StatEntry<TStat> stat)
        {
            float flat = 0, additive = 1, multiplicative = 1;

            foreach (var (effect, amount) in effects.GetEffectsOnStat(stat.Stat))
            {
                switch (effect.ModifierType)
                {
                    case ModifierType.Flat:
                        flat += effect.Value * amount;
                        break;
                    case ModifierType.Increase:
                        additive += effect.Value * amount;
                        break;
                    case ModifierType.More:
                        multiplicative *= Mathf.Pow(1 + effect.Value, amount);
                        break;
                };
            }

            float result = (stat.BaseValue + flat) * additive * multiplicative;
            Values[stat.Stat] = result;
        }

        public float GetValue(TStat stat)
        {
            if (Values.TryGetValue(stat, out var value))
            {
                return value;
            }

            RecalculateStat(stat);

            return Values[stat];
        }

        public EffectCollectionView<TStat, TEffect> GetEffectCollectionView()
        {
            return effects.GetView();
        }
    }
}

