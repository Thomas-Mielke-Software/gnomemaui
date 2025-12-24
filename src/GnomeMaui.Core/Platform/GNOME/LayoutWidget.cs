#nullable enable

namespace Microsoft.Maui.Platform;

public partial class LayoutWidget : MauiWidget
{
	public LayoutWidget()
	{
		SetHexpand(true);
		SetVexpand(true);
		Show();
	}

	public bool ClipsToBounds { get; set; }

	Gtk.Box? _backgroundLayer;

	public void UpdateInputTransparent(bool inputTransparent, object? background)
	{
		if (inputTransparent)
		{
			MakeInputTransparent(background);
		}
		else
		{
			MakeInputVisible(background);
		}
	}

	void MakeInputTransparent(object? background)
	{
		// In GTK, making a widget input transparent is more involved; for now we remove any background widget
		RemoveBackgroundLayer();
	}

	void MakeInputVisible(object? background)
	{
		// Add background layer if a background is provided
		if (background == null)
		{
			RemoveBackgroundLayer();
			return;
		}

		AddBackgroundLayer();
	}

	void AddBackgroundLayer()
	{
		if (_backgroundLayer != null)
		{
			return;
		}

		_backgroundLayer = new Gtk.Box();
		CachedChildren.Insert(0, _backgroundLayer);
	}

	void RemoveBackgroundLayer()
	{
		if (_backgroundLayer == null)
		{
			return;
		}

		CachedChildren.Remove(_backgroundLayer);
		_backgroundLayer = null;
	}
}



