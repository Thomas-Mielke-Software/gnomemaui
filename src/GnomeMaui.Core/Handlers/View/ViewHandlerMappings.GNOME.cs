using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;

namespace Microsoft.Maui.Handlers;

public partial class ViewHandler
{
	static partial void MappingFrame(IViewHandler handler, IView view)
	{
		var platform = handler.ToPlatform();
		platform?.UpdateTransformation(view);
	}
}
