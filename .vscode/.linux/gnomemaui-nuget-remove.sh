#!/usr/bin/env bash

set -e

source "$GNOMEMAUI/.vscode/.linux/gnomemaui-functions.sh"

print_step "Removing GNOME MAUI .NET packages"

delete_nuget_packages

h
