namespace Microsoft.Maui.LifecycleEvents;

public static class GNOMELifecycleBuilderExtensions
{
	// Gtk.Application events
	public static IGNOMELifecycleBuilder OnQueryEnd(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnQueryEnd del) => lifecycle.OnEvent(del);
	public static IGNOMELifecycleBuilder OnWindowAdded(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnWindowAdded del) => lifecycle.OnEvent(del);
	public static IGNOMELifecycleBuilder OnWindowRemoved(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnWindowRemoved del) => lifecycle.OnEvent(del);

	// Gio.Application events
	public static IGNOMELifecycleBuilder OnActivate(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnActivate del) => lifecycle.OnEvent(del);
	// public static IGNOMELifecycleBuilder OnCommandLine(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnCommandLine del) => lifecycle.OnEvent(del);
	// public static IGNOMELifecycleBuilder OnHandleLocalOptions(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnHandleLocalOptions del) => lifecycle.OnEvent(del);
	// public static IGNOMELifecycleBuilder OnNameLost(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnNameLost del) => lifecycle.OnEvent(del);
	public static IGNOMELifecycleBuilder OnOpen(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnOpen del) => lifecycle.OnEvent(del);
	public static IGNOMELifecycleBuilder OnShutdown(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnShutdown del) => lifecycle.OnEvent(del);
	public static IGNOMELifecycleBuilder OnStartup(this IGNOMELifecycleBuilder lifecycle, GNOMELifecycle.OnStartup del) => lifecycle.OnEvent(del);
}