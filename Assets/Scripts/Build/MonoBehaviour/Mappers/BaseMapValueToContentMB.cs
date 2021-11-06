using UnityEngine;

public abstract class BaseMapValueToContentMB<TValue, TContentValue, TContent> :
	BaseHasValueMB<TValue>
	where TContent :
		IHasValue<TContentValue>
{
	public TContent? content;

	public override TValue Value {
		set {
			if (this.content == null) throw this.NullError();
			this.content.Value = this.MapValueToContent(value);
			base.Value = value;
		}
	}

	public abstract TContentValue MapValueToContent(TValue source);
}
