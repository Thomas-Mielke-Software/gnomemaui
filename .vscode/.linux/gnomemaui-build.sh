#!/usr/bin/env bash

set -e

source "$GNOMEMAUI/.vscode/.linux/gnomemaui-functions.sh"

stop_dotnet_processes
delete_nuget_packages
delete_objbin_folders

print_step "Build GNOME MAUI .NET"
cd $GNOMEMAUI/src
dotnet build -v:diag

h
