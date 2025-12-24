namespace Microsoft.Maui.Platform;

static class ToolbarExtensions
{
	public static void UpdateTitle(this MauiToolbar nativeToolbar, IToolbar toolbar)
	{
		nativeToolbar.Title = toolbar.Title;
	}
}
