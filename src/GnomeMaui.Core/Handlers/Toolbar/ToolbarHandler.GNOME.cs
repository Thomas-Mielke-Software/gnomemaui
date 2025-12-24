namespace Microsoft.Maui.Handlers;

public partial class ToolbarHandler : ElementHandler<IToolbar, MauiToolbar>
{
	protected override MauiToolbar CreatePlatformElement() => new MauiToolbar();

	public static void MapTitle(IToolbarHandler handler, IToolbar toolbar)
	{
		handler.PlatformView.UpdateTitle(toolbar);
	}

	private protected override void OnDisconnectHandler(object platformView)
	{
		base.OnDisconnectHandler(platformView);
		if (platformView is MauiToolbar mauiToolbar)
		{
			var navRootManager = MauiContext?.GetNavigationRootManager();
			if (navRootManager != null && navRootManager.Toolbar == mauiToolbar)
			{
				navRootManager.SetToolbar(null);
			}
		}
	}
}
