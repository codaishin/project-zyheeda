namespace Routines
{
	public class Data
	{
		private Data? extention;
		private Data? host;

		public Data() { }

		public T? As<T>() where T : Data {
			return (this as T)
				?? Data.CheckExtention<T>(this)
				?? Data.CheckHost<T>(this);
		}

		public T Extent<T>() where T : Data, new() {
			return this.As<T>() ?? this.Extend<T>();
		}

		private T Extend<T>() where T : Data, new() {
			Data host = Data.FindWithoutExtention(this);
			T extention = new T();
			extention.host = host;
			host.extention = extention;
			return extention;
		}

		private static T? CheckExtention<T>(Data data) where T : Data {
			if (data.extention is null) {
				return null;
			}
			Data extension = data.extention;
			return (extension as T) ?? Data.CheckExtention<T>(extension);
		}

		private static T? CheckHost<T>(Data data) where T : Data {
			if (data.host is null) {
				return null;
			}
			Data host = data.host;
			return (host as T) ?? Data.CheckHost<T>(host);
		}

		private static Data FindWithoutExtention(Data data) {
			if (data.extention is null) {
				return data;
			}
			return Data.FindWithoutExtention(data.extention);
		}
	}
}
