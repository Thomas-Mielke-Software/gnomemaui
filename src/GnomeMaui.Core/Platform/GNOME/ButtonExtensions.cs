namespace Microsoft.Maui.Platform
{
	public static class ButtonExtensions
	{
		public static void UpdateText(this Gtk.Button platformButton, IText button)
		{
			platformButton.Label = button.Text ?? string.Empty;
		}

		public static void UpdateTextColor(this Gtk.Button platformButton, ITextStyle button)
		{
			// TODO: Implement text color using CSS
		}
	}
}
