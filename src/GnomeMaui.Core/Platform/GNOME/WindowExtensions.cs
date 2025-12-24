using System;
using Microsoft.Maui.Devices;

namespace Microsoft.Maui.Platform;

public static partial class WindowExtensions
{
	internal static DisplayOrientation GetOrientation(this IWindow? window)
	{
		if (window == null)
		{
			return DeviceDisplay.Current.MainDisplayInfo.Orientation;
		}
		return DeviceDisplay.Current.MainDisplayInfo.Orientation;

		// var appWindow = window.Handler?.MauiContext?.GetPlatformWindow()?.GetAppWindow();

		// if (appWindow == null)
		// {
		// 	return DisplayOrientation.Unknown;
		// }

		// DisplayOrientations orientationEnum;
		// int theScreenWidth = appWindow.Size.Width;
		// int theScreenHeight = appWindow.Size.Height;
		// if (theScreenWidth > theScreenHeight)
		// {
		// 	orientationEnum = DisplayOrientations.Landscape;
		// }
		// else
		// {
		// 	orientationEnum = DisplayOrientations.Portrait;
		// }

		// return orientationEnum == DisplayOrientations.Landscape
		// 	? DisplayOrientation.Landscape
		// 	: DisplayOrientation.Portrait;
	}

}
