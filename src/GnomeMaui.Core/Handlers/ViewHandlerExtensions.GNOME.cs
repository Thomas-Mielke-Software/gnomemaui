using System;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui;

static partial class ViewHandlerExtensions
{

	internal static Size LayoutVirtualView(
		this IPlatformViewHandler viewHandler, Rect frame,
		Func<Rect, Size>? arrangeFunc = null)
	{
		var virtualView = viewHandler.VirtualView;
		var platformView = viewHandler.PlatformView;

		if (virtualView == null || platformView == null)
		{
			return Size.Zero;
		}

		arrangeFunc ??= virtualView.Arrange;

		return arrangeFunc(frame);
	}

	internal static Size MeasureVirtualView(
		this IPlatformViewHandler viewHandler,
		double widthConstraint,
		double heightConstraint,
		Func<double, double, Size>? measureFunc = null)
	{
		var virtualView = viewHandler.VirtualView;
		var platformView = viewHandler.PlatformView;

		if (virtualView == null || platformView == null)
		{
			return Size.Zero;
		}

		measureFunc ??= virtualView.Measure;
		var measure = measureFunc(widthConstraint, heightConstraint);

		return measure;
	}

	internal static Size GetDesiredSizeFromHandler(this IViewHandler viewHandler, double widthConstraint, double heightConstraint)
	{
		var platformView = viewHandler.ToPlatform();
		var virtualView = viewHandler.VirtualView;

		if (platformView == null || virtualView == null)
		{
			return virtualView == null || double.IsNaN(virtualView.Width) || double.IsNaN(virtualView.Height)
				? Size.Zero
				: new Size(virtualView.Width, virtualView.Height);
		}

		if (widthConstraint < 0 || heightConstraint < 0)
		{
			return Size.Zero;
		}

		// Adjust constraints for explicit sizes (similar to Windows logic)
		double? explicitWidth = (virtualView.Width >= 0) ? virtualView.Width : null;
		double? explicitHeight = (virtualView.Height >= 0) ? virtualView.Height : null;

		var adjustedWidth = AdjustForExplicitSize(widthConstraint, explicitWidth);
		var adjustedHeight = AdjustForExplicitSize(heightConstraint, explicitHeight);

		// Use GTK's GetPreferredSize to get minimum and natural sizes
		platformView.GetPreferredSize(out var minimumSize, out var naturalSize);

		// Return the natural size, constrained by the available space
		var width = Math.Min(naturalSize.Width, adjustedWidth);
		var height = Math.Min(naturalSize.Height, adjustedHeight);

		return new Size(width, height);
	}

	internal static void PlatformArrangeHandler(this IViewHandler viewHandler, Rect rect)
	{
		var platformView = viewHandler.ToPlatform();

		if (platformView == null)
		{
			return;
		}

		if (rect.Width < 0 || rect.Height < 0)
		{
			// This is just some initial Forms value nonsense, nothing is actually laying out yet
			return;
		}

		// GTK uses SizeAllocate for layout, but this is typically handled by the parent container
		// We just need to invoke the Frame update
		viewHandler.Invoke(nameof(IView.Frame), rect);
	}

	static double AdjustForExplicitSize(double externalConstraint, double? explicitValue)
	{
		// Similar to Windows logic: if the control has an explicit size, 
		// measure at at least that size to allow it to exceed container bounds if needed

		if (!explicitValue.HasValue || double.IsNaN(explicitValue.Value))
		{
			// No explicit value, use the external constraint
			return externalConstraint;
		}

		// If the control's explicit size is larger than the container's, use the control's value
		return Math.Max(externalConstraint, explicitValue.Value);
	}

}