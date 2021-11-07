using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Event")]
public class EventSO : ScriptableObject
{
	public event Action? Listeners;

	public void Raise() => this.Listeners?.Invoke();
}
