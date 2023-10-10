using UnityEngine;
using LL.Framework.Stats;

namespace LL.Game.Stats
{
    public class LiveStatsBehavior : MonoBehaviour, ILiveStats<StatResource, EffectResource>
    {
        [SerializeField]
        LiveStats<StatResource, StatContainerResource, EffectResource> stats = new();

        public StatContainerResource Container => stats.Container;

        public bool ApplyEffect(EffectResource effect)
        {
            return stats.ApplyEffect(effect);
        }

        public bool RemoveEffect(EffectResource effect)
        {
            return stats.RemoveEffect(effect);
        }

        public float GetValue(StatResource stat)
        {
            return stats.GetValue(stat);
        }

        public EffectCollectionView<StatResource, EffectResource> GetEffectCollectionView()
        {
            return stats.GetEffectCollectionView();
        }

#if UNITY_EDITOR
        public void RecalculateStats()
        {
            stats.RecalculateStats();
        }
#endif
    }
}
