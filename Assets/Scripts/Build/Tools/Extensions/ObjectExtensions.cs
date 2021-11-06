public static class ObjectExtensions
{
	public static WasNullException NullError(this object script) {
		throw new WasNullException(script);
	}
}
