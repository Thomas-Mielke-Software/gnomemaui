#!/usr/bin/env bash

set -e

source "$GNOMEMAUI/.vscode/.linux/gnomemaui-functions.sh"

stop_dotnet_processes

print_step "Install GNOME MAUI .NET tool"
dotnet tool install GnomeMaui.Tools --tool-path "$DOTNET_ROOT"

print_step "Install GNOME MAUI .NET SDK"
dotnet gnomemaui --install --verbose

print_step "Install GNOME MAUI .NET workload"
dotnet workload install maui-gnome

stop_dotnet_processes

h
