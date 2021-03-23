using UnityEngine;
using UnityEngine.Events;

public abstract class BaseOnMorphMB<TSeed, TMorph> : MonoBehaviour
{
	public class SeedEvent : UnityEvent<TSeed> {};
	public class MorphEvent : UnityEvent<TMorph> {};

	[Header("Success Callbacks")]
	public SeedEvent onSuccessSeed;
	public MorphEvent onSuccessMorph;

	[Header("Fail Callback")]
	public SeedEvent onFailSeed;

	public abstract bool TryMorph(TSeed seed, out TMorph morph);

	public void Morph(TSeed seed)
	{
		if (this.TryMorph(seed, out TMorph morph)) {
			this.onSuccessSeed.Invoke(seed);
			this.onSuccessMorph.Invoke(morph);
		} else {
			this.onFailSeed.Invoke(seed);
		}
	}

	private void Start()
	{
		if (this.onSuccessSeed == null) {
			this.onSuccessSeed = new SeedEvent();
		}
		if (this.onSuccessMorph == null) {
			this.onSuccessMorph = new MorphEvent();
		}
		if (this.onFailSeed == null) {
			this.onFailSeed = new SeedEvent();
		}
	}
}
