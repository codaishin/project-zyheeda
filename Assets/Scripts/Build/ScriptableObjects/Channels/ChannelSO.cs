using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Channels/Channel")]
public class ChannelSO : ScriptableObject
{
	public event Action? Listeners;

	public void Raise() => this.Listeners?.Invoke();
}
