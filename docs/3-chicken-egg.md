# Chicken-egg scenario. Dependency paradox neutralization…

> [!NOTE]
> This documentation depends on the [Development Environment Setup for GNOME MAUI .NET on Linux](/docs/1-devenv.md) and the [Microsoft MAUI and SkiaSharp Patch](/docs/2-patch.md) documentation.

This document exists because, in the case of GNOME MAUI .NET, a classic chicken-and-egg situation arises:
the `net10.0-gnome` TFM only exists if the maui-gnome workload is already installed, but building and testing the workload itself requires the `net10.0-gnome` TFM. This circular dependency cannot be avoided; it can only be resolved through deliberate bootstrap steps. The following steps neutralize this paradox and ensure that the GNOME MAUI development environment can be set up in a reproducible way.

![Open GNOME MAUI terminal](/assets/GnomeMauiIcon.png)

Return to the GNOME MAUI home directory:

```bash
h
```

Install the required workloads:

```bash
.vscode/.linux/install.sh --maui
```

## Build and install GNOME MAUI SDK

### Remove existing SDK

This does not need to be run the first time.

```bash
gnomemaui-remove-sdk.sh
```

### Remove existing NuGet packages

This does not need to be run the first time.

```bash
gnomemaui-nuget-remove.sh
```

### Build chicken-egg SDK for the first time

```bash
gnomemaui-chicken-egg.sh
```

### Install chicken-egg SDK

```bash
gnomemaui-install.sh
```

### Build full version SDK

```bash
gnomemaui-build.sh
```

### Remove chicken-egg SDK

```bash
gnomemaui-remove-sdk.sh
```

### Install full version SDK

```bash
gnomemaui-install.sh
```

From this point on, the TFM for GNOME MAUI .NET exists:

![GNOME MAUI TFM](/assets/TFM.png)

## Next step

To run the sample applications, continue with the [samples](/docs/4-samples.md) documentation.
