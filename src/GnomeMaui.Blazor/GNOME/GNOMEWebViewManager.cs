using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Web;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using WebKitNavigationType = WebKit.NavigationType;
using WebKitURISchemeRequest = WebKit.URISchemeRequest;
using WebKitUserContentInjectedFrames = WebKit.UserContentInjectedFrames;
using WebKitUserContentManager = WebKit.UserContentManager;
using WebKitUserScript = WebKit.UserScript;
using WebKitUserScriptInjectionTime = WebKit.UserScriptInjectionTime;
using WebKitWebView = WebKit.WebView;

namespace Microsoft.AspNetCore.Components.WebView.Maui;

/// <summary>
/// An implementation of <see cref="WebViewManager"/> that uses the GNOME WebKit browser control
/// to render web content.
/// </summary>
[UnsupportedOSPlatform("OSX")]
[UnsupportedOSPlatform("Windows")]
class GNOMEWebViewManager : WebViewManager, IAsyncDisposable
{
	const string Scheme = "app";
	static string AppOrigin { get; } = $"{Scheme}://{BlazorWebView.AppHostAddress}/";
	static Uri AppOriginUri { get; } = new(AppOrigin);

	readonly WebKitWebView _webView;
	readonly string _contentRootRelativeToAppRoot;
	readonly string _hostPageRelativePath;
	readonly ILogger _logger;
	WebKitUserContentManager? _userContentManager;

	/// <summary>
	/// Constructs an instance of <see cref="GNOMEWebViewManager"/>.
	/// </summary>
	/// <param name="webView">The GNOME WebKit WebView instance.</param>
	/// <param name="services">A service provider containing services to be used by this class and also by application code.</param>
	/// <param name="dispatcher">A <see cref="Dispatcher"/> instance that can marshal calls to the required thread or sync context.</param>
	/// <param name="fileProvider">Provides static content to the webview.</param>
	/// <param name="jsComponents">Configuration for JS components.</param>
	/// <param name="contentRootRelativeToAppRoot">Path to the directory containing application content files.</param>
	/// <param name="hostPageRelativePath">Path to the host page within the <paramref name="fileProvider"/>.</param>
	/// <param name="logger">Logger to send log messages to.</param>
	public GNOMEWebViewManager(
		WebKitWebView webView,
		IServiceProvider services,
		Dispatcher dispatcher,
		IFileProvider fileProvider,
		JSComponentConfigurationStore jsComponents,
		string contentRootRelativeToAppRoot,
		string hostPageRelativePath,
		ILogger logger)
		: base(services, dispatcher, AppOriginUri, fileProvider, jsComponents, hostPageRelativePath)
	{
		ArgumentNullException.ThrowIfNull(webView);
		ArgumentNullException.ThrowIfNull(webView.WebContext);

		_webView = webView;
		_contentRootRelativeToAppRoot = contentRootRelativeToAppRoot;
		_hostPageRelativePath = hostPageRelativePath;
		_logger = logger;

		// Register custom URI scheme handler
		try
		{
			_webView.WebContext.RegisterUriScheme(Scheme, HandleUriScheme);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to register URI scheme: {Scheme}", Scheme);
			throw new InvalidOperationException($"Failed to register URI scheme: {Scheme}", ex);
		}

		// Set up JavaScript interop
		InitializeWebView();
	}

	void InitializeWebView()
	{
		_userContentManager = _webView.GetUserContentManager();

		// Add JavaScript for interop
		var script = WebKitUserScript.New(
			source:
			"""
				window.__receiveMessageCallbacks = [];

				window.__dispatchMessageCallback = function(message) {
					window.__receiveMessageCallbacks.forEach(function(callback) { callback(message); });
				};

				window.external = {
					sendMessage: function(message) {
						window.webkit.messageHandlers.webview.postMessage(message);
					},
					receiveMessage: function(callback) {
						window.__receiveMessageCallbacks.push(callback);
					}
				};

				Blazor.start();
			""",
			injectedFrames: WebKitUserContentInjectedFrames.AllFrames,
			injectionTime: WebKitUserScriptInjectionTime.End,
			null,
			null);

		_userContentManager.AddScript(script);

		// Register script message handler
		WebKitUserContentManager.ScriptMessageReceivedSignal.Connect(
			_userContentManager,
			WebviewInteropMessageReceived,
			true,
			"webview");

		if (!_userContentManager.RegisterScriptMessageHandler("webview", null))
		{
			_logger.LogError("Could not register script message handler");
			throw new InvalidOperationException("Could not register script message handler");
		}

		// Hook navigation events
		_webView.OnCreate += NavigationSignalHandler;
	}

	void HandleUriScheme(WebKitURISchemeRequest request)
	{
		var scheme = request.GetScheme();
		if (scheme != Scheme)
		{
			_logger.LogError("Invalid scheme: {Scheme}", scheme);
			throw new InvalidOperationException($"Invalid scheme \"{scheme}\"");
		}

		var uri = request.GetUri();
		var requestPath = request.GetPath();

		_logger.LogDebug("HandleUriScheme - URI: {Uri}, Path: {Path}", uri, requestPath);

		// Check if it's a static file or Blazor route
		var contentType = StaticContentProvider.GetResponseContentTypeOrDefault(uri);
		var isStaticFile = !string.IsNullOrEmpty(contentType) && contentType != "application/octet-stream";

		if (requestPath == "/" || !isStaticFile)
		{
			// Root path or Blazor route - serve index.html
			uri = requestPath == "/"
				? uri + _hostPageRelativePath
				: AppOriginUri.ToString() + _hostPageRelativePath;
		}

		_logger.LogDebug("Fetching: {Uri}", uri);

		if (TryGetResponseContent(uri, false, out var statusCode, out var statusMessage, out var content, out var headers))
		{
			using var memoryStream = new MemoryStream();
			content.CopyTo(memoryStream);

			var bytes = memoryStream.GetBuffer().AsSpan(0, (int)memoryStream.Length);
			using var inputStream = Gio.MemoryInputStream.NewFromBytes(GLib.Bytes.New(bytes));

			var finalContentType = isStaticFile ? contentType : headers["Content-Type"];

			request.Finish(inputStream, memoryStream.Length, finalContentType);

			_logger.LogDebug("Served: {Uri} ({StatusCode}) - {ContentType}", uri, statusCode, finalContentType);
		}
		else
		{
			_logger.LogError("Failed to serve: {Uri} - {StatusCode} {StatusMessage}", uri, statusCode, statusMessage);
			throw new InvalidOperationException($"Failed to serve \"{uri}\". {statusCode} - {statusMessage}");
		}
	}

	/// <inheritdoc />
	protected override void NavigateCore(Uri absoluteUri)
	{
		_logger.LogDebug("NavigateCore: {Uri}", absoluteUri);
		_webView.LoadUri(absoluteUri.ToString());
	}

	/// <inheritdoc />
	protected override async void SendMessage(string message)
	{
		var script = $"__dispatchMessageCallback(\"{HttpUtility.JavaScriptStringEncode(message)}\")";
		_logger.LogDebug("SendMessage: {Script}", script);

		_ = await _webView.EvaluateJavascriptAsync(script);
	}

	void WebviewInteropMessageReceived(WebKitUserContentManager sender, WebKitUserContentManager.ScriptMessageReceivedSignalArgs args)
	{
		var result = args.Value;
		var message = result.ToString();

		_logger.LogDebug("WebviewInteropMessageReceived: {Message}", message);

		MessageReceived(AppOriginUri, message);
	}

	Gtk.Widget NavigationSignalHandler(WebKitWebView sender, WebKitWebView.CreateSignalArgs args)
	{
		var navigationType = WebKit.Internal.NavigationAction.GetNavigationType(args.NavigationAction.Handle);

		_logger.LogDebug("NavigationSignalHandler - Type: {NavigationType}", navigationType);

		if (navigationType != WebKitNavigationType.LinkClicked)
		{
			return default!;
		}

		var request = WebKit.Internal.NavigationAction.GetRequest(args.NavigationAction.Handle);
		var uriHandle = WebKit.Internal.URIRequest.GetUri(request);
		var uri = uriHandle.ConvertToString();

		_logger.LogDebug("Opening external link: {Uri}", uri);

		LaunchUriInExternalBrowser(uri);
		return default!;
	}

	static void LaunchUriInExternalBrowser(string webviewUri)
	{
		if (Uri.TryCreate(webviewUri, UriKind.Absolute, out var uri))
		{
			using var launchBrowser = new Process();
			launchBrowser.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			launchBrowser.StartInfo.UseShellExecute = true;
			launchBrowser.StartInfo.FileName = uri.ToString();
			_ = launchBrowser.Start();
		}
	}

	internal bool TryGetResponseContentInternal(string uri, bool allowFallbackOnHostPage, out int statusCode, out string statusMessage, out Stream content, out IDictionary<string, string> headers)
	{
		var defaultResult = TryGetResponseContent(uri, allowFallbackOnHostPage, out statusCode, out statusMessage, out content, out headers);
		var hotReloadedResult = StaticContentHotReloadManager.TryReplaceResponseContent(_contentRootRelativeToAppRoot, uri, ref statusCode, ref content, headers);
		return defaultResult || hotReloadedResult;
	}

	/// <inheritdoc />
	protected override async ValueTask DisposeAsyncCore()
	{
		await base.DisposeAsyncCore();

		_webView.OnCreate -= NavigationSignalHandler;

		if (_userContentManager is not null)
		{
			WebKitUserContentManager.ScriptMessageReceivedSignal.Disconnect(_userContentManager, WebviewInteropMessageReceived);
			_userContentManager.Dispose();
		}
	}
}
