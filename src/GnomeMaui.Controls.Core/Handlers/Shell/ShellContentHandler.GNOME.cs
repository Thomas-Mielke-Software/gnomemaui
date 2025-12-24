#nullable disable
using System;
using Gtk;

namespace Microsoft.Maui.Controls.Handlers;

public partial class ShellContentHandler : ElementHandler<ShellContent, Widget>
{
	public static PropertyMapper<ShellContent, ShellContentHandler> Mapper =
			new PropertyMapper<ShellContent, ShellContentHandler>(ElementMapper)
			{
				[nameof(ShellContent.Title)] = MapTitle
			};

	public static CommandMapper<ShellContent, ShellContentHandler> CommandMapper =
			new CommandMapper<ShellContent, ShellContentHandler>(ElementCommandMapper);

	public ShellContentHandler() : base(Mapper, CommandMapper)
	{
	}

	internal static void MapTitle(ShellContentHandler handler, ShellContent item)
	{
		// Title updates are handled by the parent ShellSection
	}

	protected override Widget CreatePlatformElement()
	{
		return (VirtualView as IShellContentController).GetOrCreateContent().ToPlatform(MauiContext);
	}
}
