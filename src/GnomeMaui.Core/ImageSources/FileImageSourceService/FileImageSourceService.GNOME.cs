using System;

namespace Microsoft.Maui;

public partial class FileImageSourceService
{
	public override Task<IImageSourceServiceResult<GdkPixbuf.Pixbuf>?> GetImageSourceAsync(IImageSource imageSource, float scale = 1, CancellationToken cancellationToken = default)
	{
		throw new NotImplementedException();
	}
}
