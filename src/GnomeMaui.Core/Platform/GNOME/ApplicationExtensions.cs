using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.LifecycleEvents;

namespace Microsoft.Maui.Platform;

static class ApplicationExtensions
{
	public static IWindow GetWindow(this Adw.Application application)
	{
		foreach (var window in IPlatformApplication.Current?.Application?.Windows ?? Array.Empty<IWindow>())
		{
			if (window?.Handler?.PlatformView is Adw.ApplicationWindow win)
			{
				return window;
			}
		}

		throw new InvalidOperationException("Window Not Found");
	}

	public static IWindow? GetWindow(this Adw.ApplicationWindow? platformWindow)
	{
		if (platformWindow == null)
		{
			return null;
		}

		foreach (var window in IPlatformApplication.Current?.Application?.Windows ?? Array.Empty<IWindow>())
		{
			if (window?.Handler?.PlatformView is Adw.ApplicationWindow win && win == platformWindow)
			{
				return window;
			}
		}

		return null;
	}

	public static void CreatePlatformWindow(this Adw.Application platformApplication, IApplication application)
	{
		if (application.Handler?.MauiContext is not IMauiContext applicationContext)
		{
			return;
		}

		var adwWindow = new Adw.ApplicationWindow();
		adwWindow.Application = platformApplication;

		var mauiContext = applicationContext.MakeWindowScope(adwWindow, out var windowScope);

		applicationContext.Services.InvokeLifecycleEvents<GNOMELifecycle.OnMauiContextCreated>(del => del(mauiContext));

		var activationState = new ActivationState(mauiContext);
		var window = application.CreateWindow(activationState);

		adwWindow.SetWindowHandler(window, mauiContext);

		adwWindow.Present();
	}
}
