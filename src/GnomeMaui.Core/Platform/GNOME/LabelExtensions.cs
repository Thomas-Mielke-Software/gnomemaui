namespace Microsoft.Maui.Platform
{
    public static class LabelExtensions
    {
        public static void UpdateText(this Gtk.Label platformLabel, ILabel label)
        {
            platformLabel.Label_ = label.Text ?? string.Empty;
        }

        public static void UpdateTextColor(this Gtk.Label platformLabel, ILabel label)
        {
            // TODO: Implement text color using CSS or markup
        }
    }
}
