public class PluginData
{
	private PluginData? source;

	public PluginData() { }

	public PluginData(PluginData source) {
		this.source = source;
	}

	public T? As<T>() where T : PluginData {
		return (this as T) ?? this.source?.As<T>();
	}

	public static PluginData Add<T>(PluginData data)
		where T : PluginData, new() {
		return data.As<T>() is null
			? new T { source = data }
			: data;
	}
}
