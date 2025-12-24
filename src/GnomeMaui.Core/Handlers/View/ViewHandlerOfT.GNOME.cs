using System;
using System.Diagnostics;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Handlers;

public partial class ViewHandler<TVirtualView, TPlatformView> : IPlatformViewHandler
{
	Gtk.Widget? IPlatformViewHandler.PlatformView => (Gtk.Widget?)base.PlatformView;

	public override void PlatformArrange(Rect rect)
	{
		this.PlatformArrangeHandler(rect);
	}

	public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		=> this.GetDesiredSizeFromHandler(widthConstraint, heightConstraint);

	protected override void SetupContainer()
	{

	}

	protected override void RemoveContainer()
	{

	}
}