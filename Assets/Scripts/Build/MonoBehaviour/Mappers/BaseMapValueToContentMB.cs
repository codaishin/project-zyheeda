using UnityEngine;

public abstract class BaseMapValueToContentMB<TValue, TContentValue, TContent> :
	BaseHasValueMB<TValue>
	where TContent :
		IHasValue<TContentValue>
{
	private TValue value;

	public TContent content;

	public override TValue Value {
		get => this.value;
		set {
			this.content.Value = this.MapValueToContent(value);
			this.value = value;
		}
	}

	public abstract TContentValue MapValueToContent(TValue source);
}
