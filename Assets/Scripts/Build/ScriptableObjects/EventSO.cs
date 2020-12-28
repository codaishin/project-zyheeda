using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Event")]
public class EventSO : ScriptableObject
{
	public event Action Callbacks;

	public void Raise() => this.Callbacks?.Invoke();
}
