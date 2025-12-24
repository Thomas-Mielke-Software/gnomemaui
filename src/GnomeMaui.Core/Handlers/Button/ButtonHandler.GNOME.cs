using System;

namespace Microsoft.Maui.Handlers
{
	public partial class ButtonHandler : ViewHandler<IButton, Gtk.Button>
	{
		protected override Gtk.Button CreatePlatformView() => Gtk.Button.New();

		public static void MapStrokeColor(IButtonHandler handler, IButtonStroke buttonStroke) { }
		public static void MapStrokeThickness(IButtonHandler handler, IButtonStroke buttonStroke) { }
		public static void MapCornerRadius(IButtonHandler handler, IButtonStroke buttonStroke) { }
		public static void MapText(IButtonHandler handler, IText button)
		{

			var pv = handler.PlatformView;
			if (pv == null)
			{
				return;
			}

			bool handleOk = true;
			var hndl = pv.Handle;
			if (hndl.IsClosed || hndl.IsInvalid)
			{
				handleOk = false;
			}
			if (handleOk)
			{
				if (hndl.DangerousGetHandle() == IntPtr.Zero)
				{
					handleOk = false;
				}
			}

			if (!handleOk)
			{
				return;
			}

			pv.UpdateText(button);
		}

		public static void MapTextColor(IButtonHandler handler, ITextStyle button)
		{
			// handler.PlatformView?.UpdateTextColor(button);
		}
		public static void MapCharacterSpacing(IButtonHandler handler, ITextStyle button) { }
		public static void MapFont(IButtonHandler handler, ITextStyle button) { }
		public static void MapPadding(IButtonHandler handler, IButton button) { }
		public static void MapImageSource(IButtonHandler handler, IImage image) { }

		partial class ButtonImageSourcePartSetter
		{
			public override void SetImageSource(Gtk.Picture? platformImage) { }
		}

		protected override void ConnectHandler(Gtk.Button platformView)
		{
			base.ConnectHandler(platformView);
			platformView.OnClicked += OnButtonClicked;
		}

		protected override void DisconnectHandler(Gtk.Button platformView)
		{
			platformView.OnClicked -= OnButtonClicked;
			base.DisconnectHandler(platformView);
		}

		void OnButtonClicked(Gtk.Button sender, EventArgs args)
		{
			VirtualView?.Clicked();
		}
	}
}