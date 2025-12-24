using System;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Microsoft.Maui.Platform;

public class NavigationRootManager
{
	readonly WeakReference<Adw.ApplicationWindow> _platformWindow;
	WindowRootView _rootView;
	bool _disconnected = true;
	internal event EventHandler? OnApplyTemplateFinished;

	public NavigationRootManager(Adw.ApplicationWindow platformWindow)
	{
		_platformWindow = new(platformWindow);
		_rootView = new WindowRootView();
		_rootView.BackRequested += OnBackRequested;
		_rootView.OnApplyTemplateFinished += WindowRootViewOnApplyTemplateFinished;
	}

	public Gtk.Widget RootView => _rootView;

	internal MauiToolbar? Toolbar => _rootView.Toolbar;

	void WindowRootViewOnApplyTemplateFinished(object? sender, EventArgs e) =>
		OnApplyTemplateFinished?.Invoke(this, EventArgs.Empty);

	void OnBackRequested(object? sender, EventArgs e)
	{
		if (_platformWindow.TryGetTarget(out var window))
		{
			window.GetWindow()?.BackButtonClicked();
		}
	}

	internal void Connect(IWindowHandler handler)
	{
		_ = handler.MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

		if (!_platformWindow.TryGetTarget(out var platformWindow))
		{
			return;
		}

		// Preserve existing toolbar instance across disconnect/reconnect so RootNavigationView
		// receives a non-null toolbar reference when UpdateBackButtonVisibility runs.
		var preservedToolbar = _rootView.Toolbar;

		Disconnect();
		var platformView = handler.VirtualView.Content?.ToPlatform(handler.MauiContext);
		Connect(platformView);

		// Restore the preserved toolbar reference (if any)
		SetToolbar(preservedToolbar);
	}

	public virtual void Connect(Gtk.Widget? platformView)
	{

		if (_rootView.Content != null)
		{
			// Clear out the toolbar that was set from the previous content
			SetToolbar(null);
			_rootView.Content = null;
		}

		if (platformView is Adw.NavigationView navView)
		{
			_rootView.Content = navView;
		}
		else
		{
			var rootNavView = new RootNavigationView();
			rootNavView.Content = platformView;
			_rootView.Content = rootNavView;
		}

		// Ensure toolbar reference is restored on the root view (match Windows behavior)
		SetToolbar(_rootView.Toolbar);

		if (_disconnected && _platformWindow.TryGetTarget(out var platformWindow))
		{
			platformWindow.Content = _rootView;
			// Ensure the platform window is presented/shown so it becomes visible and maps
			platformWindow.Present();
			platformWindow.Show();
			// Ensure the root view and its content are visible so GTK will map/realize them
			_rootView.Show();
			if (_rootView.Content is Gtk.Widget content)
			{
				content.Show();
				// If content is a RootNavigationView, also show its inner content if present
				if (content is RootNavigationView rootNav && rootNav.Content is Gtk.Widget contentInner)
				{
					contentInner.Show();
				}
			}

			_disconnected = false;
		}
	}

	public virtual void Disconnect()
	{
		SetToolbar(null);

		if (_rootView.Content is RootNavigationView navView)
		{
			navView.Content = null;
		}

		_rootView.Content = null;
		_disconnected = true;
	}

	internal void SetMenuBar(Gio.Menu? menuBar)
	{
		_rootView.MenuBar = menuBar;
	}

	internal void SetToolbar(MauiToolbar? toolBar)
	{
		_rootView.Toolbar = toolBar;
	}

	internal string? WindowTitle
	{
		get => _rootView.WindowTitle;
		set => _rootView.WindowTitle = value;
	}

	internal void SetTitle(string? title) =>
		_rootView.WindowTitle = title;

	internal void SetTitleBar(ITitleBar? titlebar, IMauiContext? mauiContext)
	{
		_rootView.SetTitleBar(titlebar, mauiContext);
	}
}
