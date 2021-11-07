using UnityEngine;

public abstract class BaseMapFieldToContentMB<TValue, TContentValue> :
	MonoBehaviour
{
	public TValue? value;
	public BaseHasValueMB<TContentValue>? content;

	public abstract TContentValue MapFieldToContentValue(TValue fieldValue);

	public void Apply() {
		if (this.content == null || this.value == null) throw this.NullError();
		this.content.Value = this.MapFieldToContentValue(this.value);
	}
}
