namespace Microsoft.Maui.LifecycleEvents;

public static class GNOMELifecycle
{
	// Gtk.Application events

	/// <summary>
	/// Triggered before the application shutdown request
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>
	public delegate void OnQueryEnd(Adw.Application application, EventArgs args);

	/// <summary>
	/// Triggered when a new window is added to the application
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>
	public delegate void OnWindowAdded(Adw.Application application, Gtk.Application.WindowAddedSignalArgs args);

	/// <summary>
	/// Triggered when a window is removed from the application
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>
	public delegate void OnWindowRemoved(Adw.Application application, Gtk.Application.WindowRemovedSignalArgs args);

	// Gio.Application events

	/// <summary>
	/// Triggered when the application is activated
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>
	public delegate void OnActivate(Adw.Application application, EventArgs args);

	/// <summary>
	/// Triggered when the primary application instance receives command-line arguments from a new process invocation
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>
	public delegate void OnCommandLine(Adw.Application application, Gio.Application.CommandLineSignalArgs args);

	/// <summary>
	/// Triggered in the local process after parsing command-line options but before contacting the primary instance
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>
	public delegate void OnHandleLocalOptions(Adw.Application application, Gio.Application.HandleLocalOptionsSignalArgs args);

	/// <summary>
	/// Triggered if the application loses its name on D-Bus
	/// </summary>
	/// <param name="application"></param>
	/// <param name="lost"></param>
	public delegate void OnNameLost(Adw.Application application, bool lost);

	/// <summary>
	/// Triggered when opening files
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>
	public delegate void OnOpen(Adw.Application application, Gio.Application.OpenSignalArgs args);

	/// <summary>
	/// Triggered when the application is shutting down
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>	
	public delegate void OnShutdown(Adw.Application application, EventArgs args);

	/// <summary>
	/// Triggered on application startup
	/// </summary>
	/// <param name="application"></param>
	/// <param name="args"></param>
	public delegate void OnStartup(Adw.Application application, EventArgs args);

	// Internal events
	internal delegate void OnMauiContextCreated(IMauiContext mauiContext);
}