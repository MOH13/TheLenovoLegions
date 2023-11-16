using System;
using UnityEngine;
using UnityEngine.Serialization;
using LL.Framework.Stats;

namespace LL.Game.Stats
{
    [CreateAssetMenu(fileName = "Effect", menuName = "Stats/Effect", order = 2)]
    public class EffectResource : ScriptableObject, IEffect<StatResource>
    {
        [field: SerializeField, FormerlySerializedAs("<Name>k__BackingField")]
        public string EffectName { get; private set; } = "NewEffect";

        [SerializeField, FormerlySerializedAs("<Stat>k__BackingField")]
        private StatResource? stat;

        public StatResource Stat
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
        public ModifierType ModifierType { get; private set; } = ModifierType.Flat;

        [field: SerializeField]
        public float Value { get; private set; } = 1;
    }
}
