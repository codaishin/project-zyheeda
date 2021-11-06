using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseMorphMB<TSeed, TMorph> : MonoBehaviour
{
	[Serializable]
	public class MorphEvent : UnityEvent<TMorph> { }

	public MorphEvent? onMorph;

	public abstract TMorph DoMorph(TSeed seed);

	public void Morph(TSeed seed) => this.onMorph?.Invoke(this.DoMorph(seed));

	private void Start() {
		if (this.onMorph == null) {
			this.onMorph = new MorphEvent();
		}
	}
}
