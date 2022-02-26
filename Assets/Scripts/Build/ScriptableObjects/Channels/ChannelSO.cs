using System;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Channels/Channel")]
public class ChannelSO : BaseChannelSO
{
	private event Action? Listeners;

	public void Raise() => this.Listeners?.Invoke();

	public override void AddListener(Action action) =>
		this.Listeners += action;
	public override void RemoveListener(Action action) =>
		this.Listeners -= action;
}
