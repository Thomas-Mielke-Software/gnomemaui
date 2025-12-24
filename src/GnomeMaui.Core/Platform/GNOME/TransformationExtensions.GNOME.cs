namespace Microsoft.Maui.Platform;

public static class TransformationExtensions
{
	public static void UpdateTransformation(this Gtk.Widget? platformView, IView? view)
	{
		if (platformView == null || view == null)
			return;

		var frame = view.Frame;

		// Apply size
		var width = (int)Math.Max(0, Math.Round(frame.Width));
		var height = (int)Math.Max(0, Math.Round(frame.Height));
		platformView.SetSizeRequest(width, height);

		// Apply translation/position using margins (works for many containers)
		var x = (int)Math.Round(frame.X + view.TranslationX);
		var y = (int)Math.Round(frame.Y + view.TranslationY);

		platformView.SetMarginStart(x);
		platformView.SetMarginTop(y);
	}
}
