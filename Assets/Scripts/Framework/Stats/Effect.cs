namespace LL.Framework.Stats
{
    public enum ModifierType
    {
        Flat = 1,
        Increase = 2,
        More = 4,
    }

    public interface IEffect
    {
        string EffectName { get; }
        ModifierType ModifierType { get; }
        float Value { get; }
    }

    public interface IEffect<TStat> : IEffect where TStat : IStat
    {
        TStat Stat { get; }
    }

    public static class EffectUtils
    {
        public static string Describe<TStat, TEffect>(TEffect effect) where TStat : IStat where TEffect : IEffect<TStat>
        {
            return effect.ModifierType switch
            {
                ModifierType.Flat =>
                    effect.Value >= 0
                    ? $"+{effect.Value:#.##} {effect.Stat.StatName}"
                    : $"{effect.Value:#.##} {effect.Stat.StatName}",
                ModifierType.Increase =>
                    effect.Value >= 0
                    ? $"{effect.Value:0.##%} increased {effect.Stat.StatName}"
                    : $"{-effect.Value:0.##%} reduced {effect.Stat.StatName}",
                ModifierType.More =>
                    effect.Value >= 0
                    ? $"{effect.Value:0.##%} more {effect.Stat.StatName}"
                    : $"{-effect.Value:0.##%} less {effect.Stat.StatName}",
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}

