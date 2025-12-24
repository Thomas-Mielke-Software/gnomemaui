using Microsoft.Maui.Dispatching;

namespace Microsoft.Maui.Dispatching;

// A MAUI IDispatcher implementation that uses GLib main loop (Idle/Timeout)
public partial class Dispatcher : IDispatcher
{
	readonly SynchronizationContext? _context = SynchronizationContext.Current;

	bool IsDispatchRequiredImplementation() => _context != SynchronizationContext.Current;

	bool DispatchImplementation(Action action)
	{
		if (!IsDispatchRequiredImplementation())
		{
			action();
			return true;
		}

		_context?.Post(_ => action(), null);
		return true;
	}

	static bool DispatchDelayedImplementation(TimeSpan delay, Action action)
	{
		uint milliseconds = (uint)Math.Max(1, delay.TotalMilliseconds);

		GLib.Functions.TimeoutAdd(
			GLib.Constants.PRIORITY_DEFAULT,
			milliseconds,
			new GLib.SourceFunc(() =>
			{
				action();
				return GLib.Constants.SOURCE_REMOVE;
			}));

		return true;
	}

	static DispatcherTimer CreateTimerImplementation()
	{
		return new DispatcherTimer();
	}
}
