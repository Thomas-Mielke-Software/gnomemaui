#!/usr/bin/env bash

set -e

source "$GNOMEMAUI/.vscode/.linux/gnomemaui-functions.sh"

stop_dotnet_processes

print_step "Install workload dependencies"
dotnet workload install maui-android maui-windows wasm-tools

stop_dotnet_processes
