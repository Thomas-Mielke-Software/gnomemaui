using System.Threading.Tasks;

namespace Microsoft.Maui.Platform;

public static partial class ViewExtensions
{
	public static void UpdateIsEnabled(this Gtk.Widget platformView, IView view) { }

	public static void Focus(this Gtk.Widget platformView, FocusRequest request) { }

	public static void Unfocus(this Gtk.Widget platformView, IView view) { }

	public static void UpdateVisibility(this Gtk.Widget platformView, IView view) { }

	public static Task UpdateBackgroundImageSourceAsync(this Gtk.Widget platformView, IImageSource? imageSource, IImageSourceServiceProvider? provider)
		=> Task.CompletedTask;

	public static void UpdateBackground(this Gtk.Widget platformView, IView view) { }

	public static void UpdateClipsToBounds(this Gtk.Widget platformView, IView view) { }

	public static void UpdateAutomationId(this Gtk.Widget platformView, IView view) { }

	public static void UpdateClip(this Gtk.Widget platformView, IView view) { }

	public static void UpdateShadow(this Gtk.Widget platformView, IView view) { }

	public static void UpdateBorder(this Gtk.Widget platformView, IView view) { }

	public static void UpdateOpacity(this Gtk.Widget platformView, IView view) { }

	internal static void UpdateOpacity(this Gtk.Widget platformView, double opacity) { }

	public static void UpdateSemantics(this Gtk.Widget platformView, IView view) { }

	public static void UpdateFlowDirection(this Gtk.Widget platformView, IView view) { }

	public static void UpdateTranslationX(this Gtk.Widget platformView, IView view) { }

	public static void UpdateTranslationY(this Gtk.Widget platformView, IView view) { }

	public static void UpdateScale(this Gtk.Widget platformView, IView view) { }

	public static void UpdateRotation(this Gtk.Widget platformView, IView view) { }

	public static void UpdateRotationX(this Gtk.Widget platformView, IView view) { }

	public static void UpdateRotationY(this Gtk.Widget platformView, IView view) { }

	public static void UpdateAnchorX(this Gtk.Widget platformView, IView view) { }

	public static void UpdateAnchorY(this Gtk.Widget platformView, IView view) { }

	public static void InvalidateMeasure(this Gtk.Widget platformView, IView view) { }

	public static void UpdateWidth(this Gtk.Widget platformView, IView view) { }

	public static void UpdateHeight(this Gtk.Widget platformView, IView view) { }

	public static void UpdateMinimumHeight(this Gtk.Widget platformView, IView view) { }

	public static void UpdateMaximumHeight(this Gtk.Widget platformView, IView view) { }

	public static void UpdateMinimumWidth(this Gtk.Widget platformView, IView view) { }

	public static void UpdateMaximumWidth(this Gtk.Widget platformView, IView view) { }

	internal static Graphics.Rect GetPlatformViewBounds(this IView view) => view.Frame;

	internal static System.Numerics.Matrix4x4 GetViewTransform(this IView view) => new System.Numerics.Matrix4x4();

	// Used by MAUI XAML Hot Reload.
	// Consult XET if updating!
	internal static Graphics.Rect GetBoundingBox(this IView view) => view.Frame;

	internal static object? GetParent(this Gtk.Widget? view)
	{
		return null;
	}

	internal static IWindow? GetHostedWindow(this IView? view)
		=> null;

	public static void UpdateInputTransparent(this Gtk.Widget nativeView, IViewHandler handler, IView view) { }

	[MissingMapper]
	public static void UpdateToolTip(this Gtk.Widget? platformView, ToolTip? tooltip) { }

	public static bool IsLoaded(this Gtk.Widget? platformView)
	{
		if (platformView is not { })
		{
			return false;
		}

		return platformView.GetRealized();
	}

	internal static IDisposable OnLoaded(this Gtk.Widget platformView, Action action)
	{
		if (platformView.IsLoaded())
		{
			action();
			return new ActionDisposable(() => { });
		}

		GObject.SignalHandler<Gtk.Widget>? routedEventHandler = null;

		ActionDisposable? disposable = new ActionDisposable(() =>
		{
			if (routedEventHandler != null)
			{
				platformView?.OnRealize -= routedEventHandler;
			}
		});

		routedEventHandler = (_, __) =>
		{
			disposable?.Dispose();
			disposable = null;
			action();
		};

		platformView?.OnRealize += routedEventHandler;

		return disposable;
	}

	internal static IDisposable OnUnloaded(this Gtk.Widget platformView, Action action)
	{
		if (!platformView.IsLoaded())
		{
			action();
			return new ActionDisposable(() => { });
		}

		GObject.SignalHandler<Gtk.Widget>? routedEventHandler = null;


		ActionDisposable? disposable = new ActionDisposable(() =>
		{
			if (routedEventHandler != null)
			{
				platformView.OnUnrealize -= routedEventHandler;
			}
		});

		routedEventHandler = (_, __) =>
		{
			disposable?.Dispose();
			disposable = null;
			action();
		};

		platformView.OnUnrealize += routedEventHandler;

		return disposable;
	}

	internal static T? GetChildAt<T>(this Gtk.Widget platformView, int index) where T : Gtk.Widget
	{
		// if (platformView is Gtk.Container container && container.Children.Length < index)
		// {
		// 	return (T)container.Children[index];
		// }

		// if (platformView is Gtk.Bin bin && index == 0 && bin.Child is T child)
		// {
		// 	return child;
		// }

		return default;
	}

}
