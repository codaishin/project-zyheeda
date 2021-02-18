using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnDisposeFunc<T>(in T value);

public class Disposable<T> : IDisposable
{
	private OnDisposeFunc<T> onDispose;

	public T Value { get; }

	public Disposable(in T value, in OnDisposeFunc<T> onDispose)
	{
		this.onDispose = onDispose;
		this.Value = value;
	}

	public void Dispose() => this.onDispose(this.Value);
}
