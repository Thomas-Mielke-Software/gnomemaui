using System.Threading;
using System.Threading.Tasks;
using Microsoft.Maui;

namespace SkiaSharp.Views.Maui.Handlers;

public partial class SKImageSourceService
{
	public override Task<IImageSourceServiceResult<GdkPixbuf.Pixbuf>?> GetImageSourceAsync(IImageSource imageSource, float scale = 1, CancellationToken cancellationToken = default)
	{
		var bitmap = imageSource switch
		{
			ISKImageImageSource img => img.Image?.ToImage(),
			ISKBitmapImageSource bmp => bmp.Bitmap?.ToImage(),
			ISKPixmapImageSource pix => pix.Pixmap?.ToImage(),
			ISKPictureImageSource pic => pic.Picture?.ToImage(pic.Dimensions),
			_ => null,
		};

		return bitmap != null
			? FromResult(new ImageSourceServiceResult(bitmap))
			: FromResult(null);
	}

	static Task<IImageSourceServiceResult<GdkPixbuf.Pixbuf>?> FromResult(ImageSourceServiceResult? result) =>
		Task.FromResult<IImageSourceServiceResult<GdkPixbuf.Pixbuf>?>(result);
}

public static class SKImageSourceExtensions
{
	public static GdkPixbuf.Pixbuf ToImage(this SKImage? image)
	{
		if (image == null)
		{
			return default!;
		}

		using var data = image.Encode();
		using var loader = GdkPixbuf.PixbufLoader.New();

		var bytes = data.ToArray();
		loader.Write(bytes.AsSpan());
		loader.Close();

		return loader.GetPixbuf()!;
	}

	public static GdkPixbuf.Pixbuf ToImage(this SKBitmap? bitmap)
	{
		if (bitmap == null)
		{
			return default!;
		}

		using var image = SKImage.FromBitmap(bitmap);
		return image.ToImage();
	}

	public static GdkPixbuf.Pixbuf ToImage(this SKPixmap? pixmap)
	{
		if (pixmap == null)
		{
			return default!;
		}

		using var image = SKImage.FromPixels(pixmap);
		return image.ToImage();
	}

	public static GdkPixbuf.Pixbuf ToImage(this SKPicture? picture, SKSizeI dimensions)
	{
		if (picture == null)
		{
			return default!;
		}

		using var surface = SKSurface.Create(new SKImageInfo(dimensions.Width, dimensions.Height));
		var canvas = surface.Canvas;
		canvas.Clear(SKColors.Transparent);
		canvas.DrawPicture(picture);
		canvas.Flush();

		using var image = surface.Snapshot();
		return image.ToImage();
	}
}
