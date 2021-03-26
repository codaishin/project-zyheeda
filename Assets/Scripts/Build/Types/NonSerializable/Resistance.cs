using System;

public class Resistance : RecordsArray<EffectTag, float>
{
	public Resistance(Func<Record<EffectTag, float>[]> get, Action<Record<EffectTag, float>[]> set) : base(get, set) {}
}
