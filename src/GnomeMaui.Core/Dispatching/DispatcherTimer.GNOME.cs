using Microsoft.Maui.Dispatching;

namespace Microsoft.Maui.Dispatching;

/// <summary>
/// A timer that is integrated with the GLib main loop.
/// </summary>
public class DispatcherTimer : IDispatcherTimer
{
	uint _sourceId;

	/// <summary>
	/// Gets or sets the interval at which the timer ticks.
	/// Default is approximately 17 milliseconds (about 60 FPS).
	/// </summary>
	public TimeSpan Interval { get; set; } = TimeSpan.FromMilliseconds(17);

	/// <summary>
	/// Gets or sets whether the timer should repeat.
	/// Default is <see langword="true"/>.
	/// </summary>
	public bool IsRepeating { get; set; } = true;

	/// <summary>
	/// Gets whether the timer is currently running.
	/// </summary>
	public bool IsRunning { get; private set; }

	/// <summary>
	/// Occurs when the timer ticks.
	/// </summary>
	public event EventHandler? Tick;

	/// <summary>
	/// Starts the timer.
	/// </summary>
	public void Start()
	{
		if (IsRunning)
		{
			return;
		}

		IsRunning = true;

		uint milliseconds = (uint)Math.Max(1, Interval.TotalMilliseconds);

		_sourceId = GLib.Functions.TimeoutAdd(
			GLib.Constants.PRIORITY_DEFAULT,
			milliseconds,
			new GLib.SourceFunc(() =>
			{
				if (!IsRunning)
				{
					return GLib.Constants.SOURCE_REMOVE;
				}

				Tick?.Invoke(this, EventArgs.Empty);

				if (!IsRepeating)
				{
					IsRunning = false;
					_sourceId = 0;
					return GLib.Constants.SOURCE_REMOVE;
				}

				return GLib.Constants.SOURCE_CONTINUE;
			}));
	}

	/// <summary>
	/// Stops the timer.
	/// </summary>
	public void Stop()
	{
		if (!IsRunning)
		{
			return;
		}

		IsRunning = false;

		if (_sourceId != 0)
		{
			try
			{ GLib.Functions.SourceRemove(_sourceId); }
			catch { }
			finally { _sourceId = 0; }
		}
	}
}
