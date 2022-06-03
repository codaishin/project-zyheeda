namespace Routines
{
	public class Data
	{
		private Data? extension;
		private Data? host;

		public Data() { }

		public T? As<T>() where T : Data {
			return (this as T)
				?? Data.CheckExtension<T>(this)
				?? Data.CheckHost<T>(this);
		}

		public T Extent<T>() where T : Data, new() {
			return this.As<T>() ?? this.Extend<T>();
		}

		private T Extend<T>() where T : Data, new() {
			Data host = Data.FindWithoutExtention(this);
			T extention = new T();
			extention.host = host;
			host.extension = extention;
			return extention;
		}

		private static T? CheckExtension<T>(Data data) where T : Data {
			if (data.extension is null) {
				return null;
			}
			Data extension = data.extension;
			return (extension as T) ?? Data.CheckExtension<T>(extension);
		}

		private static T? CheckHost<T>(Data data) where T : Data {
			if (data.host is null) {
				return null;
			}
			Data host = data.host;
			return (host as T) ?? Data.CheckHost<T>(host);
		}

		private static Data FindWithoutExtention(Data data) {
			if (data.extension is null) {
				return data;
			}
			return Data.FindWithoutExtention(data.extension);
		}
	}
}
