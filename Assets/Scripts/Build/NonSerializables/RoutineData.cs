namespace Routines
{
	public class RoutineData
	{
		private RoutineData? extention;
		private RoutineData? host;

		public RoutineData() { }

		public T? As<T>() where T : RoutineData {
			return (this as T)
				?? RoutineData.CheckExtention<T>(this)
				?? RoutineData.CheckHost<T>(this);
		}

		public T Extent<T>() where T : RoutineData, new() {
			return this.As<T>() ?? this.Extend<T>();
		}

		private T Extend<T>() where T : RoutineData, new() {
			RoutineData host = RoutineData.FindWithoutExtention(this);
			T extention = new T();
			extention.host = host;
			host.extention = extention;
			return extention;
		}

		private static T? CheckExtention<T>(RoutineData data) where T : RoutineData {
			if (data.extention is null) {
				return null;
			}
			RoutineData extension = data.extention;
			return (extension as T) ?? RoutineData.CheckExtention<T>(extension);
		}

		private static T? CheckHost<T>(RoutineData data) where T : RoutineData {
			if (data.host is null) {
				return null;
			}
			RoutineData host = data.host;
			return (host as T) ?? RoutineData.CheckHost<T>(host);
		}

		private static RoutineData FindWithoutExtention(RoutineData data) {
			if (data.extention is null) {
				return data;
			}
			return RoutineData.FindWithoutExtention(data.extention);
		}
	}
}
