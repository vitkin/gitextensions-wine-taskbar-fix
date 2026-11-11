#!/usr/bin/env bash
set -euo pipefail
dir="$(dirname "$(readlink -f "$0")")"
if [ "$#" -gt 1 ]; then
    echo "Usage: $0 [path-to-DLL]" >&2
    exit 1
fi
dll="${1:-$dir/../Microsoft.WindowsAPICodePack.Shell.dll}"
if [ ! -f "$dll" ]; then
    echo "error: $dll not found" >&2
    exit 1
fi

patched="$(mktemp "${dll}.patched.XXXXXX")"
trap 'rm -f -- "$patched"' EXIT
dotnet run --project "$dir/src/patchge.csproj" --configuration Release -- "$dll" "$patched"
chmod --reference="$dll" "$patched"
if [ ! -e "$dll.bak" ]; then
    cp --update=none -- "$dll" "$dll.bak"
else
    echo "Existing backup left unchanged: $dll.bak"
fi
mv -f -- "$patched" "$dll"
echo "OK: taskbar feature disabled. Backup at $dll.bak"
