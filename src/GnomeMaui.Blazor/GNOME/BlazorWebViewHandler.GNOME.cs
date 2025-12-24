using System;
using System.IO;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Maui;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Handlers;
using WebKitSettings = WebKit.Settings;
using WebKitWebView = WebKit.WebView;

namespace Microsoft.AspNetCore.Components.WebView.Maui;

[UnsupportedOSPlatform("OSX")]
[UnsupportedOSPlatform("Windows")]
public partial class BlazorWebViewHandler : ViewHandler<IBlazorWebView, WebKitWebView>
{
	GNOMEWebViewManager? _webviewManager;
	internal GNOMEWebViewManager? WebviewManager => _webviewManager;

	ILogger? _logger;
	internal ILogger Logger => _logger ??= Services!.GetService<ILogger<BlazorWebViewHandler>>() ?? NullLogger<BlazorWebViewHandler>.Instance;

	protected override WebKitWebView CreatePlatformView()
	{
		Logger.LogInformation("Creating GNOME WebKit WebView");

		var webView = new WebKitWebView();

		if (webView.WebContext == null)
		{
			throw new InvalidOperationException("WebContext is null");
		}

		// Get settings
		var settings = webView.GetSettings();

		if (settings != null)
		{
			// Enable JavaScript
			settings.EnableJavascript = true;
			settings.EnableHtml5LocalStorage = true;
			settings.EnableHtml5Database = true;

			// Enable developer tools if enabled
			if (DeveloperTools.Enabled)
			{
				settings.EnableDeveloperExtras = true;
			}
		}

		// Fire initialization events
		VirtualView.BlazorWebViewInitializing(new BlazorWebViewInitializingEventArgs());

		VirtualView.BlazorWebViewInitialized(new BlazorWebViewInitializedEventArgs
		{
			WebView = webView,
		});

		Logger.LogInformation("Created GNOME WebKit WebView");

		return webView;
	}

	protected override void DisconnectHandler(WebKitWebView platformView)
	{
		platformView.StopLoading();

		if (_webviewManager != null)
		{
			// Start the disposal...
			var disposalTask = _webviewManager
				.DisposeAsync()
				.AsTask();

			if (IsBlockingDisposalEnabled)
			{
				// If the app is configured to block on dispose via an AppContext switch,
				// we'll synchronously wait for the disposal to complete. This can cause a deadlock.
				disposalTask
					.GetAwaiter()
					.GetResult();
			}
			else
			{
				// Otherwise, by default, we'll fire-and-forget the disposal task.
				_ = Task.Run(async () =>
				{
					try
					{
						await disposalTask.ConfigureAwait(false);
					}
					catch (Exception ex)
					{
						Logger.LogError(ex, "Error during webview disposal");
					}
				});
			}

			_webviewManager = null;
		}
	}

	bool RequiredStartupPropertiesSet =>
		HostPage != null &&
		Services != null;

	void StartWebViewCoreIfPossible()
	{
		if (!RequiredStartupPropertiesSet ||
			_webviewManager != null)
		{
			return;
		}

		if (PlatformView == null)
		{
			throw new InvalidOperationException($"Can't start {nameof(BlazorWebView)} without native web view instance.");
		}

		// We assume the host page is always in the root of the content directory, because it's
		// unclear there's any other use case. We can add more options later if so.
		var contentRootDir = Path.GetDirectoryName(HostPage!) ?? string.Empty;
		var hostPageRelativePath = Path.GetRelativePath(contentRootDir, HostPage!);

		Logger.LogInformation("Creating file provider - ContentRoot: {ContentRoot}, HostPage: {HostPage}",
			contentRootDir, hostPageRelativePath);

		var fileProvider = VirtualView.CreateFileProvider(contentRootDir);

		_webviewManager = new GNOMEWebViewManager(
			PlatformView,
			Services!,
			new MauiDispatcher(Services!.GetRequiredService<IDispatcher>()),
			fileProvider,
			VirtualView.JSComponents,
			contentRootDir,
			hostPageRelativePath,
			Logger);

		StaticContentHotReloadManager.AttachToWebViewManagerIfEnabled(_webviewManager);

		if (RootComponents != null)
		{
			foreach (var rootComponent in RootComponents)
			{
				Logger.LogInformation("Adding root component: {ComponentType} to {Selector} with {ParameterCount} parameters",
					rootComponent.ComponentType?.FullName ?? string.Empty,
					rootComponent.Selector ?? string.Empty,
					rootComponent.Parameters?.Count ?? 0);

				// Since the page isn't loaded yet, this will always complete synchronously
				_ = rootComponent.AddToWebViewManagerAsync(_webviewManager);
			}
		}

		Logger.LogInformation("Starting initial navigation: {StartPath}", VirtualView.StartPath);
		_webviewManager.Navigate(VirtualView.StartPath);
	}

	internal IFileProvider CreateFileProvider(string contentRootDir)
	{
		// Ensure we have an absolute path for PhysicalFileProvider
		var absolutePath = Path.IsPathRooted(contentRootDir)
			? contentRootDir
			: Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, contentRootDir));

		Logger.LogInformation("Creating PhysicalFileProvider with absolute path: {AbsolutePath}", absolutePath);

		return new PhysicalFileProvider(absolutePath);
	}

	/// <summary>
	/// Calls the specified <paramref name="workItem"/> asynchronously and passes in the scoped services available to Razor components.
	/// </summary>
	/// <param name="workItem">The action to call.</param>
	/// <returns>Returns a <see cref="Task"/> representing <c>true</c> if the <paramref name="workItem"/> was called, or <c>false</c> if it was not called because Blazor is not currently running.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="workItem"/> is <c>null</c>.</exception>
	public virtual async Task<bool> TryDispatchAsync(Action<IServiceProvider> workItem)
	{
		ArgumentNullException.ThrowIfNull(workItem);

		if (_webviewManager is null)
		{
			return false;
		}

		return await _webviewManager.TryDispatchAsync(workItem);
	}
}
