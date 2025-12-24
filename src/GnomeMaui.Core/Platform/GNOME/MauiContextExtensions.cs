using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui.Platform;

static partial class MauiContextExtensions
{
	public static NavigationRootManager GetNavigationRootManager(this IMauiContext mauiContext) =>
		mauiContext.Services.GetRequiredService<NavigationRootManager>();


	public static Adw.ApplicationWindow GetPlatformWindow(this IMauiContext mauiContext) =>
		mauiContext.Services.GetRequiredService<Adw.ApplicationWindow>();
}
