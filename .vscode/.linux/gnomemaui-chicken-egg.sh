#!/usr/bin/env bash

set -e

echo "[gnomemaui] Chicken-egg scenario. Dependency paradox neutralization…"

source "$GNOMEMAUI/.vscode/.linux/gnomemaui-functions.sh"

stop_dotnet_processes
delete_nuget_packages
delete_objbin_folders
oldpwd="$PWD"
cd $GNOMEMAUI/src
dotnet build -v:diag /p:IsGNOME=false
cd "$oldpwd"
