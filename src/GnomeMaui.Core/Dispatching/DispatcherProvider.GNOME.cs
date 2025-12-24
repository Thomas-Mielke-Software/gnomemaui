namespace Microsoft.Maui.Dispatching;

public partial class DispatcherProvider
{
	static Dispatcher? GetForCurrentThreadImplementation() => new();
}

