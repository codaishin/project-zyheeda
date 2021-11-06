using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public abstract class BaseContentMB<TElem, TContentElem> :
	MonoBehaviour,
	IHasValue<TElem[]>
	where TContentElem :
		MonoBehaviour,
		IHasValue<TElem>
{
	private TElem[]? value;

	public TContentElem? prefab;

	public TElem[] Value {
		get => this.value ?? throw this.NullError();
		set {
			this.value = value;
			TContentElem[] contents = this.GetComponentsInChildren<TContentElem>(
				includeInactive: true
			);
			BaseContentMB<TElem, TContentElem>.Pair(value, contents)
				.Select(this.MatchPairToAction)
				.Apply();
		}
	}

	private Action MatchPairToAction(
		(TElem? elem, TContentElem? contentElem) pair
	) {
		return pair switch {
			(_, null) => () => this.CreateContentElemFor(pair.elem!),
			(null, _) => () => this.DisableContentElem(pair.contentElem!),
			(_, _) => () => this.UpdateContentElem((pair.elem!, pair.contentElem!)),
		};
	}

	private void CreateContentElemFor(TElem elem) {
		if (this.prefab == null) throw this.NullError();
		TContentElem contentElem = GameObject.Instantiate(this.prefab);
		contentElem.Value = elem;
		contentElem.transform.SetParent(this.transform);
	}

	private void UpdateContentElem((TElem, TContentElem) pair) {
		(TElem elem, TContentElem contentElem) = pair;
		contentElem.Value = elem;
		contentElem.gameObject.SetActive(true);
	}

	private void DisableContentElem(TContentElem contentElem) {
		contentElem.gameObject.SetActive(false);
	}

	private static IEnumerable<(TElem?, TContentElem?)> Pair(
		TElem[] elems,
		TContentElem[] contents
	) {
		for (int i = 0; i < Mathf.Max(elems.Length, contents.Length); ++i) {
			yield return (
				i < elems.Length ? elems[i] : default,
				i < contents.Length ? contents[i] : default
			);
		}
	}
}
