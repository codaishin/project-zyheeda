public class PluginData
{
	private PluginData? extention;
	private PluginData? host;

	public PluginData() { }

	public T? As<T>() where T : PluginData {
		return (this as T)
			?? PluginData.CheckExtention<T>(this)
			?? PluginData.CheckHost<T>(this);
	}

	public T Extent<T>() where T : PluginData, new() {
		return this.As<T>() ?? this.Extend<T>();
	}

	private T Extend<T>() where T : PluginData, new() {
		PluginData host = PluginData.FindWithoutExtention(this);
		T extention = new T();
		extention.host = host;
		host.extention = extention;
		return extention;
	}

	private static T? CheckExtention<T>(PluginData data) where T : PluginData {
		if (data.extention is null) {
			return null;
		}
		PluginData extension = data.extention;
		return (extension as T) ?? PluginData.CheckExtention<T>(extension);
	}

	private static T? CheckHost<T>(PluginData data) where T : PluginData {
		if (data.host is null) {
			return null;
		}
		PluginData host = data.host;
		return (host as T) ?? PluginData.CheckHost<T>(host);
	}

	private static PluginData FindWithoutExtention(PluginData data) {
		if (data.extention is null) {
			return data;
		}
		return PluginData.FindWithoutExtention(data.extention);
	}
}
