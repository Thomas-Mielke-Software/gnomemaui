# Microsoft MAUI and SkiaSharp Patch

> [!NOTE]
> This documentation depends on the [Development Environment Setup for GNOME MAUI .NET on Linux](/docs/1-devenv.md) documentation.

## Start GNOME MAUI shell

You can start the environment in two ways:

**Option 1 - From the desktop launcher**:
The setup script installs a launcher named **GNOME MAUI** with its own icon (`gnomemaui.desktop`). You can start it directly from your GNOME application menu.

![Open GNOME MAUI terminal](/assets/GnomeMauiIcon.png)

**Option 2 - From Ptyxis**:
Open **Ptyxis** and select the **GNOME MAUI** profile.
This profile is also created automatically by the setup scripts.

Now type `h`. this is an internal function that navigates back to the `$GNOMEMAUI` directory:

```bash
h
```

Move one directory up:

```bash
cd ..
```

## MAUI

The current patch requires this exact version, do not change it:

```bash
wget https://github.com/dotnet/maui/archive/refs/tags/10.0.11.tar.gz
```

```bash
tar -xvf 10.0.11.tar.gz
```

## SkiaSharp

```bash
wget https://github.com/mono/SkiaSharp/archive/refs/tags/v3.119.1.tar.gz
```

```bash
tar -xvf v3.119.1.tar.gz
```

## Back to the GNOME MAUI home

```bash
h
```

## Symlinks

```bash
cd ext
```

```bash
ln -s $GNOMEMAUI/../maui-10.0.11 maui
```

```bash
ln -s $GNOMEMAUI/../SkiaSharp-3.119.1 skiasharp
```

## Patch MAUI

```bash
cd maui
```

```bash
patch -p1 < $GNOMEMAUI/ext/maui.patch
```

## Patch SkiaSharp

SkiaSharp currently does not require a patch, but this may be necessary in the future.

## Next step

You can continue with the GNOME MAUI .NET [Chicken-and-egg SDK](/docs/3-chicken-egg.md) documentation.
