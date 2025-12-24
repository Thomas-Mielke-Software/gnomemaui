
#nullable enable
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GObject;
using GObject.Internal;
using Gtk;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Platform;

public class MauiWidget : Gtk.Box, ICrossPlatformLayoutBacking, IVisualTreeElementProvidable
{
	public MauiWidget()
	{
		SetOrientation(Orientation.Vertical);
		SetSpacing(6);
		OnNotify += MauiWidget_OnNotify;
		OnMap += MauiWidget_OnMap;
	}

	void MauiWidget_OnMap(Widget sender, EventArgs args)
	{
		for (int i = 0; i < CachedChildren.Count; i++)
		{
			var child = CachedChildren[i];
			if (child == null)
			{
				continue;
			}

			bool handleOk = true;
			var hndl = child.Handle;
			if (hndl.IsClosed || hndl.IsInvalid)
			{
				handleOk = false;
			}
			if (handleOk)
			{
				var h = hndl.DangerousGetHandle();
				if (h == IntPtr.Zero)
				{
					handleOk = false;
				}
			}

			if (!handleOk)
			{
				continue;
			}
			child.Unparent();
			var dh = child.Handle.DangerousGetHandle();
			Append(child);

			var dh2 = child.Handle.DangerousGetHandle();
			child.Show();
			child.QueueResize();
			this.QueueResize();
		}
	}

	void MauiWidget_OnNotify(GObject.Object sender, NotifySignalArgs args)
	{
		var name = args.Pspec.GetName();

		if (name == "parent" || name == "root")
		{
			EnsureChildrenParented();
		}
	}

	void EnsureChildrenParented()
	{
		for (int i = 0; i < CachedChildren.Count; i++)
		{
			var child = CachedChildren[i];
			if (child == null)
			{
				continue;
			}

			// Skip if native handle not ready
			bool handleOk = true;
			var hndl = child.Handle;
			if (hndl.IsClosed || hndl.IsInvalid)
			{
				handleOk = false;
			}
			if (handleOk)
			{
				var h = hndl.DangerousGetHandle();
				if (h == IntPtr.Zero)
				{
					handleOk = false;
				}
			}

			if (!handleOk)
			{
				continue;
			}
			child.Unparent();
			Append(child);
			child.Show();
		}
	}
	public List<Widget> CachedChildren { get; } = new List<Widget>();

	public ICrossPlatformLayout? CrossPlatformLayout { get; set; }

	// Helpers mirroring the Windows MauiPanel cross-platform layout calls
	public Size CrossPlatformMeasure(double width, double height)
	{
		return CrossPlatformLayout?.CrossPlatformMeasure(width, height) ?? Size.Zero;
	}

	public Size CrossPlatformArrange(Rect bounds)
	{
		return CrossPlatformLayout?.CrossPlatformArrange(bounds) ?? Size.Zero;
	}

	public IVisualTreeElement? GetElement()
	{
		if (CrossPlatformLayout is IVisualTreeElement layoutElement &&
			layoutElement.IsThisMyPlatformView(this))
		{
			return layoutElement;
		}

		return null;
	}

	internal void UpdatePlatformViewBackground(ILayout layout)
	{
		// GNOME-specific background handling can be added here when needed.
	}

	internal MauiWidget(Gtk.Internal.BoxHandle handle) : base(handle) { }

	public MauiWidget(params GObject.ConstructArgument[] constructArguments) : this(Gtk.Internal.BoxHandle.For<MauiWidget>(constructArguments)) { }
}