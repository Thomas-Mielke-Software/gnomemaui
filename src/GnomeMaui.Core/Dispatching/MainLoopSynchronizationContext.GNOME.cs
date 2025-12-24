using GLib;

namespace Microsoft.Maui.Dispatching;

/// <summary>
/// A synchronization context that posts work to the GLib main loop.
/// </summary>
public sealed class MainLoopSynchronizationContext : SynchronizationContext
{
	/// <summary>
	/// Creates a copy of the synchronization context.
	/// </summary>
	/// <returns>>A new <see cref="MainLoopSynchronizationContext"/> instance.</returns>
	public override SynchronizationContext CreateCopy()
		=> new MainLoopSynchronizationContext();

	/// <summary>
	/// Posts a callback to be executed asynchronously on the main loop.
	/// </summary>
	/// <param name="callback">The callback to execute.</param>
	/// <param name="state">The state to pass to the callback.</param>
	public override void Post(SendOrPostCallback callback, object? state)
	{
		var proxy = new GLib.Internal.SourceFuncNotifiedHandler(() =>
		{
			try
			{ callback(state); }
			catch (Exception ex) { UnhandledException.Raise(ex); }
			return false;
		});

		_ = GLib.Internal.Functions.TimeoutAdd(GLib.Constants.PRIORITY_DEFAULT, 0, proxy.NativeCallback!, IntPtr.Zero, proxy.DestroyNotify!);
	}

	/// <summary>
	/// Sends a callback to be executed synchronously on the main loop.
	/// </summary>
	/// <param name="callback">The callback to execute.</param>
	/// <param name="state">The state to pass to the callback.</param>
	public override void Send(SendOrPostCallback callback, object? state)
	{
		var proxy = new GLib.Internal.SourceFuncAsyncHandler(() =>
		{
			try
			{ callback(state); }
			catch (Exception ex) { UnhandledException.Raise(ex); }
			return false;
		});

		GLib.Internal.MainContext.Invoke(GLib.Internal.MainContextUnownedHandle.NullHandle, proxy.NativeCallback, IntPtr.Zero);
	}
}
