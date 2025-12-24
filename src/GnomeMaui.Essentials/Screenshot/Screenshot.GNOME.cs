using System;
using System.IO;
using System.Threading.Tasks;
using Adw;
using Gtk;
using Microsoft.Maui.ApplicationModel;

namespace Microsoft.Maui.Media
{
	partial class ScreenshotImplementation : IPlatformScreenshot, IScreenshot
	{
		public bool IsCaptureSupported =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		public Task<IScreenshotResult> CaptureAsync() =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		public Task<IScreenshotResult> CaptureAsync(Adw.Window window) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		public Task<IScreenshotResult> CaptureAsync(Widget view) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;
	}

	partial class ScreenshotResult
	{
		ScreenshotResult()
		{
		}

		Task<Stream> PlatformOpenReadAsync(ScreenshotFormat format, int quality) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		Task PlatformCopyToAsync(Stream destination, ScreenshotFormat format, int quality) =>
			throw ExceptionUtils.NotSupportedOrImplementedException;

		Task<byte[]> PlatformToPixelBufferAsync() =>
			throw ExceptionUtils.NotSupportedOrImplementedException;
	}
}
