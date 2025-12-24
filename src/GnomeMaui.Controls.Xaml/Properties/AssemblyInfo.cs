using System.Runtime.CompilerServices;

using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;

// [assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Xaml.UnitTests")]
// [assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Xaml.Benchmarks")]
// [assembly: InternalsVisibleTo("MockSourceGenerator.Generated")]
[assembly: InternalsVisibleTo("GnomeMaui.Controls.Build.Tasks")]
// [assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Xaml.Design")]
// [assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Loader")]// Microsoft.Maui.Controls.Loader.dll Microsoft.Maui.Controls.Xaml.XamlLoader.Load(object, string), kzu@microsoft.com
// [assembly: InternalsVisibleTo("Microsoft.Maui.Controls.HotReload.Forms")]
// [assembly: InternalsVisibleTo("Microsoft.Maui.Controls.HotReload.UnitTests")]
[assembly: InternalsVisibleTo("GnomeMaui.Controls.SourceGen")]
// [assembly: InternalsVisibleTo("Microsoft.Maui.Controls.Compatibility")]
// [assembly: InternalsVisibleTo("CommunityToolkit.Maui")]
// [assembly: InternalsVisibleTo("CommunityToolkit.Maui.Core")]
// [assembly: InternalsVisibleTo("CommunityToolkit.Maui.Embedding")]
// [assembly: InternalsVisibleTo("CommunityToolkit.Maui.UnitTests")]
// [assembly: InternalsVisibleTo("CommunityToolkit.Maui.Markup")]
// [assembly: InternalsVisibleTo("CommunityToolkit.Maui.Markup.UnitTests")]
// [assembly: InternalsVisibleTo("Microsoft.Maui.Controls.DeviceTests")]

[assembly: Preserve]

// Global namespace aggregation (required by XAML runtime)
// This defines that http://schemas.microsoft.com/dotnet/maui/global aggregates http://schemas.microsoft.com/dotnet/2021/maui
[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.MauiGlobalUri, Microsoft.Maui.Controls.Xaml.XamlParser.MauiUri)]

[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.MauiUri, "Microsoft.Maui.Controls.Xaml")]
[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.MauiDesignUri, "Microsoft.Maui.Controls.Xaml")]
[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.X2006Uri, "Microsoft.Maui.Controls.Xaml")]
[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.X2006Uri, "System", AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.X2006Uri, "System", AssemblyName = "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.X2009Uri, "Microsoft.Maui.Controls.Xaml")]
[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.X2009Uri, "System", AssemblyName = "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
[assembly: XmlnsDefinition(Microsoft.Maui.Controls.Xaml.XamlParser.X2009Uri, "System", AssemblyName = "System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]