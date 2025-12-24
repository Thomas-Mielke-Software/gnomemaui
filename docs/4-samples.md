# Build and run sample applications

> [!NOTE]
> This documentation depends on the [Development Environment Setup for GNOME MAUI .NET on Linux](/docs/1-devenv.md), the [Microsoft MAUI and SkiaSharp Patch](/docs/2-patch.md), and the [Chicken-egg scenario. Dependency paradox neutralization…](/docs/3-chicken-egg.md) documentation.

![Open GNOME MAUI terminal](/assets/GnomeMauiIcon.png)

## Build and run MAUI sample app

```bash
cd $GNOMEMAUI/samples/MauiTest1
```

```bash
dotnet build -f net10.0-gnome -v:diag
```

```bash
GSK_RENDERER=opengl dotnet ./bin/Debug/net10.0-gnome/MauiTest1.dll
```

## Build and run MAUI Blazor sample app

The Blazor application does not include an Adwaita header because it is not yet implemented in MAUI. By pressing `Alt`+`Space`, the window menu can be opened, where close, resize, and move actions are available.

```bash
cd $GNOMEMAUI/samples/MauiBlazorApp1
```

```bash
dotnet build -f net10.0-gnome -v:diag
```

```bash
dotnet ./bin/Debug/net10.0-gnome/MauiBlazorApp1.dll
```
