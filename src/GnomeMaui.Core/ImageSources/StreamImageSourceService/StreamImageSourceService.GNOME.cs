using System;
using GdkPixbuf;

namespace Microsoft.Maui;

public partial class StreamImageSourceService
{
	public override Task<IImageSourceServiceResult<Pixbuf>?> GetImageSourceAsync(IImageSource imageSource, float scale = 1, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
