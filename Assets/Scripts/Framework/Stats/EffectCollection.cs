using System.Collections.Generic;

namespace LL.Framework.Stats
{
    public struct EffectCollection<TStat, TEffect> where TStat : IStat where TEffect : IEffect<TStat>
    {
        private Dictionary<TStat, Dictionary<TEffect, int>>? activeEffects;

        private Dictionary<TStat, Dictionary<TEffect, int>> ActiveEffects
        {
            get
            {
                activeEffects ??= new();
                return activeEffects;
            }
        }

        public Dictionary<TEffect, int> GetEffectsOnStat(TStat stat)
        {
            if (ActiveEffects.TryGetValue(stat, out var effects))
            {
                return effects;
            }
            Dictionary<TEffect, int> empty = new();
            ActiveEffects.Add(stat, empty);
            return empty;
        }

        public bool ApplyEffect(TEffect effect)
        {
            var statEffects = GetEffectsOnStat(effect.Stat);

            if (statEffects.TryGetValue(effect, out var amount))
            {
                statEffects[effect] = amount + 1;
            }
            else
            {
                statEffects.Add(effect, 1);
            }
            return true;
        }

        public bool RemoveEffect(TEffect effect)
        {
            var statEffects = GetEffectsOnStat(effect.Stat);

            if (statEffects.TryGetValue(effect, out var amount))
            {
                if (amount > 1)
                {
                    statEffects[effect] = amount - 1;
                }
                else
                {
                    statEffects.Remove(effect);
                }
                return true;
            }
            return false;
        }

        public EffectCollectionView<TStat, TEffect> GetView()
        {
            return new EffectCollectionView<TStat, TEffect>(this);
        }

        public struct Enumerator
        {
            private Dictionary<TStat, Dictionary<TEffect, int>>.Enumerator outer;

            private Dictionary<TEffect, int>.Enumerator inner;

            private bool exit;

            public KeyValuePair<TEffect, int> Current => inner.Current;

            public bool MoveNext()
            {
                if (exit) return false;

                if (inner.MoveNext())
                {
                    return true;
                }
                if (outer.MoveNext())
                {
                    inner = outer.Current.Value.GetEnumerator();
                    return MoveNext();
                }
                return false;
            }

            public static Enumerator Create(EffectCollection<TStat, TEffect> effects)
            {
                var activeEffects = effects.ActiveEffects;
                var outer = activeEffects.GetEnumerator();

                if (outer.MoveNext())
                {
                    var inner = outer.Current.Value.GetEnumerator();
                    return new Enumerator
                    {
                        outer = outer,
                        inner = inner,
                        exit = false,
                    };
                }
                return new Enumerator
                {
                    exit = true,
                };
            }
        }
    }

    public struct EffectCollectionView<TStat, TEffect> where TStat : IStat where TEffect : IEffect<TStat>
    {
        private EffectCollection<TStat, TEffect> effects;

        public EffectCollectionView(EffectCollection<TStat, TEffect> effects)
        {
            this.effects = effects;
        }

        public EffectCollection<TStat, TEffect>.Enumerator GetEnumerator()
        {
            return EffectCollection<TStat, TEffect>.Enumerator.Create(effects);
        }
    }
}
