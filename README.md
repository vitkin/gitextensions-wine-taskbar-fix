# GitExtensions Wine Taskbar Fix

This tool patches `Microsoft.WindowsAPICodePack.Shell.dll` to disable the
Windows taskbar integration that causes JumpList errors under Wine.

In particular, `ICustomDestinationList.SetAppID` throws a
`System.NotImplementedException` when GitExtensions updates the JumpList.
Specifically, it replaces `TaskbarManager.IsPlatformSupported` with a method
that returns `false`.

GitExtensions updates may replace the patched DLL.

Wine's [bug 39161](https://bugs.winehq.org/show_bug.cgi?id=39161) tracks
missing JumpList support for another application.

## Requirements

- .NET 9 SDK and access to NuGet to restore Mono.Cecil 0.11.6.
- Bash and GNU coreutils (9.2 or newer).
- A writable GitExtensions installation containing
  `Microsoft.WindowsAPICodePack.Shell.dll`.

The repository contains the source in `src/`; published binaries from an
older version are not required.

## Apply the fix

Close GitExtensions, then from this directory run:

```bash
./apply.sh
```

By default the script targets the DLL in the parent directory. To target a
different installation or a test copy, provide the DLL path:

```bash
./apply.sh /path/to/Microsoft.WindowsAPICodePack.Shell.dll
```

The script builds the tool, patches to a temporary file, and replaces the DLL
only after the patch succeeds. It saves the original as `<DLL>.bak` if that
backup does not already exist. Existing backups are never overwritten, so
check their provenance before relying on them; a backup made from an already
patched DLL is itself patched. To restore a verified backup, copy it over the
DLL. Reapplying the patch is safe.

The tool can also write a patched copy without replacing the input:

```bash
dotnet run --project src/patchge.csproj -c Release -- input.dll output.dll
```
