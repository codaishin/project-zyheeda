using System;
using System.Collections;

public class Finalizable : IEnumerator
{
	public IEnumerator wrapped;

	public event Action? OnFinalize;

	public object Current => this.wrapped.Current;

	public Finalizable(IEnumerator wrapped) {
		this.wrapped = wrapped;
	}

	public bool MoveNext() {
		if (this.wrapped.MoveNext()) {
			return true;
		}
		this.OnFinalize?.Invoke();
		this.OnFinalize = null;
		return false;
	}

	public void Reset() => this.wrapped.Reset();
}
