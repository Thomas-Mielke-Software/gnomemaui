#!/usr/bin/env bash

set -e

source "$GNOMEMAUI/.vscode/.linux/gnomemaui-functions.sh"

print_step "Uninstall GNOME MAUI .NET"

dotnet workload uninstall maui-gnome || true

print_step "Uninstall GnomeMaui.Tools"
dotnet gnomemaui --uninstall --verbose || true
dotnet tool uninstall GnomeMaui.Tools --tool-path "$DOTNET_ROOT" || true

stop_dotnet_processes

h
