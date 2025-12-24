using System;

namespace Microsoft.Maui.Animations;

/// <summary>
/// A ticker implementation that uses the GLib main loop for GNOME/GTK applications.
/// </summary>
public class PlatformTicker : Ticker
{
	bool _isRunning;
	uint _sourceId;

	/// <inheritdoc/>
	public override bool IsRunning => _isRunning;

	/// <inheritdoc/>
	public override void Start()
	{
		if (_isRunning)
		{
			return;
		}

		_isRunning = true;

		// Fire approximately every 16ms (~60 FPS)
		_sourceId = GLib.Functions.TimeoutAdd(
			GLib.Constants.PRIORITY_DEFAULT,
			16,
			new GLib.SourceFunc(() =>
			{
				if (!_isRunning)
				{
					return GLib.Constants.SOURCE_REMOVE;
				}

				Fire?.Invoke();

				return GLib.Constants.SOURCE_CONTINUE;
			}));
	}

	/// <inheritdoc/>
	public override void Stop()
	{
		if (!_isRunning)
		{
			return;
		}

		_isRunning = false;

		if (_sourceId != 0)
		{
			try
			{
				GLib.Functions.SourceRemove(_sourceId);
			}
			catch
			{
				// Ignore errors during cleanup
			}
			finally
			{
				_sourceId = 0;
			}
		}
	}
}
