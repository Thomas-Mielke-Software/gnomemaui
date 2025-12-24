# Development Environment Setup for GNOME MAUI .NET on Linux

> [!CAUTION]
> The development environment can currently only be set up on Linux systems according to the instructions described here. It is very important to follow the steps exactly, step by step; otherwise, the build process will fail.

![Open Ptyxis terminal](/assets/PtyxisIcon.png)

This directory can be located anywhere and can have any name, but for the sake of example we will use `$HOME/gnomemaui` from here on:

```bash
mkdir $HOME/gnomemaui
```

```bash
cd $HOME/gnomemaui
```

Clone the repository:

```bash
git clone https://github.com/gnomemaui/gnomemaui.git
```

```bash
cd gnomemaui
```

Download devenv

```bash
wget https://github.com/czirok/devenv/releases/download/v2025.12.24/devenv.tar.bz2
```

Safe extraction

```bash
tar xjfv devenv.tar.bz2 --skip-old-files
```

Check dependencies:

```bash
.vscode/.linux/install.sh
```

If you see that everything is checked, then everything is fine. (This example was run on Arch Linux):

```bash
[✓] Trash CLI found (version: 0.24.5.26)
[✓] BC found (version: bc 1.08.2)
[✓] Curl found (version: curl 8.17.0 (x86_64-pc-linux-gnu) libcurl/8.17.0 OpenSSL/3.6.0 zlib/1.3.1 brotli/1.1.0 zstd/1.5.7 libidn2/2.3.8 libpsl/0.21.5 libssh2/1.11.1 nghttp2/1.68.0 nghttp3/1.13.1 mit-krb5/1.21.3)
[✓] Bash found (version: GNU bash, 5.3.9(1)-release (x86_64-pc-linux-gnu) version)
[✓] Visual Studio Code found (version: 1.107.1)
[✓] Ptyxis found (version: Ptyxis 49.2)
[✓] All dependencies are installed!

```

If something is not installed, you can find the documentation required for installation here: [devenv](https://github.com/czirok/devenv?tab=readme-ov-file#dependencies)

When everything is in order, the next step is to install the development environment:

```bash
.vscode/.linux/install.sh --all
```

You should see this or something similar:

```bash
[✓] GNOMEMAUI="/home/user/gnomemaui/gnomemaui" variable added to /home/user/.bash_profile successfully
[✓] .bashrc created and updated with GNOMEMAUI
[✓] Generated 21 theme palettes in /home/user/.local/share/org.gnome.Ptyxis/palettes
[✓] Ptyxis profile configuration created
[✓] Ptyxis profile configured
[✓] Icon installed as gnomemaui.svg
[✓] Desktop file installed as gnomemaui.desktop
[!] 3 fonts were already installed
[✓] Icon cache updated
[✓] Desktop database updated
[✓] Font cache updated
[✓] Configuration created and updated with GNOMEMAUI
[✓] Oh My Posh installed to /home/user/gnomemaui/gnomemaui/.vscode/.linux
[✓] VSCode settings updated with new colors
[✓] .NET 10 SDK installed
[✓] .NET 9 Runtime installed
[✓] .NET 9 ASP.NET Core Runtime installed
[✓] NuGet source configured
[✓] .NET tools installed
[✓] Installation completed successfully!
[!] You may need to log out and back in for the GNOMEMAUI environment variable to take effect, if you changed the git root directory.
```

> [!CAUTION]
> **VERY IMPORTANT!** The entire GNOME MAUI development environment is built on the `$GNOMEMAUI` variable. Log out of GNOME now and log back in so that the GNOMEMAUI environment variable takes effect! Then return here.

## Next step

You can now continue with the [patch](/docs/2-patch.md) documentation.
