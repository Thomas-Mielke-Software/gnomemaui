using System;
using System.Collections.Concurrent;
using System.IO;

namespace Microsoft.Maui;

public class FontManager : IFontManager
{
	public double DefaultFontSize => 13.0;

	readonly IFontRegistrar _fontRegistrar;
	readonly IServiceProvider? _serviceProvider;

	/// <summary>
	/// Creates a new <see cref="EmbeddedFontLoader"/> instance.
	/// </summary>
	/// <param name="fontRegistrar">An <see cref="IFontRegistrar"/> instance for retrieving details about the registered fonts.</param>
	/// <param name="serviceProvider">The applications <see cref="IServiceProvider"/>.
	/// Typically this is provided through dependency injection.</param>
	public FontManager(IFontRegistrar fontRegistrar, IServiceProvider? serviceProvider = null)
	{
		_fontRegistrar = fontRegistrar;
		_serviceProvider = serviceProvider;
	}
}