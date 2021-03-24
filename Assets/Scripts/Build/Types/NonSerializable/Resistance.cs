using System.Collections.Generic;

public class Resistance : AsDictWrapper<EffectTag, float>
{
	public Resistance(List<Record<EffectTag, float>> records) : base(records) {}
}
