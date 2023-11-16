using UnityEngine;
using LL.Framework.Stats;

namespace LL.Game.Stats
{
    [CreateAssetMenu(fileName = "NewStat", menuName = "Stats/Stat", order = 1)]
    public class StatResource : ScriptableObject, IStat
    {
        [field: SerializeField]
        public string StatName { get; private set; } = "NewStat";
    }
}
