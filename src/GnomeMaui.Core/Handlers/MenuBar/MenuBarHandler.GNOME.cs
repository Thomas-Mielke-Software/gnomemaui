namespace Microsoft.Maui.Handlers;

public partial class MenuBarHandler : ElementHandler<IMenuBar, Gio.Menu>, IMenuBarHandler
{
	// TODO : Need to implement
	protected override Gio.Menu CreatePlatformElement()
	{
		throw new NotImplementedException();
	}

	public void Add(IMenuBarItem view)
	{
	}

	public void Remove(IMenuBarItem view)
	{
	}

	public void Clear()
	{
	}

	public void Insert(int index, IMenuBarItem view)
	{
	}
}
