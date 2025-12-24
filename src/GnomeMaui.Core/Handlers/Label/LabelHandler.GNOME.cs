using System;
using Gtk;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;

namespace Microsoft.Maui.Handlers;

public partial class LabelHandler : ViewHandler<ILabel, Gtk.Label>
{
	protected override Gtk.Label CreatePlatformView() => new Gtk.Label();

	public override bool NeedsContainer =>
	VirtualView?.Background != null ||
	(VirtualView != null && VirtualView.VerticalTextAlignment != TextAlignment.Start) ||
	base.NeedsContainer;

	protected override void SetupContainer()
	{
		PlatformView.HeightRequest = int.MinValue;
		MapHeight(this, VirtualView);
	}

	protected override void RemoveContainer()
	{
		MapHeight(this, VirtualView);
	}

	public static void MapHeight(ILabelHandler handler, ILabel view) =>
		// VerticalAlignment only works when the container's Height is set and the child's Height is Auto. The child's Height
		// is set to Auto when the container is introduced
		handler.ToPlatform().UpdateHeight(view);

	public static void MapText(ILabelHandler handler, ILabel label)
	{
		handler.PlatformView?.UpdateText(label);
	}

	public static void MapTextColor(ILabelHandler handler, ILabel label)
	{
		handler.PlatformView?.UpdateTextColor(label);
	}
	public static void MapCharacterSpacing(ILabelHandler handler, ILabel label) { }
	public static void MapFont(ILabelHandler handler, ILabel label) { }
	public static void MapHorizontalTextAlignment(ILabelHandler handler, ILabel label) { }
	public static void MapVerticalTextAlignment(ILabelHandler handler, ILabel label) { }
	public static void MapTextDecorations(ILabelHandler handler, ILabel label) { }
	public static void MapMaxLines(ILabelHandler handler, ILabel label) { }
	public static void MapPadding(ILabelHandler handler, ILabel label) { }
	public static void MapLineHeight(ILabelHandler handler, ILabel label) { }

}