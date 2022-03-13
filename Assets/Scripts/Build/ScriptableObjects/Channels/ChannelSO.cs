using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Channels/Channel")]
public class ChannelSO : ScriptableObject, IChannel
{
	private event Action? Listeners;

	public void Raise() => this.Listeners?.Invoke();

	public void AddListener(Action action) =>
		this.Listeners += action;
	public void RemoveListener(Action action) =>
		this.Listeners -= action;
}
