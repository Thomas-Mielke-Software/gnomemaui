#!/usr/bin/env bash

set -e

source "$GNOMEMAUI/.vscode/.linux/gnomemaui-functions.sh"

delete_nuget_packages

print_step "Build GNOME MAUI .NET"
cd $GNOMEMAUI/src
dotnet build -v:diag
