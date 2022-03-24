using UnityEngine;

public enum ItemType
{
	Rifle,
}

public interface IItem
{
	Animation.Stance IdleStance { get; }
	Animation.State ActiveState { get; }
	Transform transform { get; }
}

public class ItemMB : MonoBehaviour, IItem
{
	public Animation.Stance idleStance;
	public Animation.State activeState;

	public Animation.Stance IdleStance => this.idleStance;
	public Animation.State ActiveState => this.activeState;
}
