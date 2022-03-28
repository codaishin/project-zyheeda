using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AfterStartMB : MonoBehaviour
{
	public UnityEvent? onStart;

	private void Start() {
		this.StartCoroutine(this.OnStartNextUpdate());
	}

	private IEnumerator<WaitForEndOfFrame> OnStartNextUpdate() {
		yield return new WaitForEndOfFrame();
		this.onStart?.Invoke();
	}
}
