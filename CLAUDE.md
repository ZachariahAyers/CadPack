# CadPack

AutoCAD 2027 plug-in: a ribbon tab of CadPack commands (C#, .NET 10) plus three bundled
third-party AutoLISP routines. How it fits together: `docs/ARCHITECTURE.md`.

## Commands

- `node --test 'tools/*.test.mjs'`: LISP structure and ribbon icon tests. Runs anywhere.
- `node tools/check-lisp.mjs lisp`: parens, dialog braces, data rows, support files.
- `dotnet build src/CadPack/CadPack.csproj`: builds the DLL and lays out `bin/<config>/net10.0-windows/CadPack.bundle`.
  Builds on Linux too (`EnableWindowsTargeting`); release builds come from the Windows CI job.

Nothing here runs inside AutoCAD. Behaviour is proved only by loading the bundle in AutoCAD 2027.

## Where things live

- `src/CadPack/Plugin.cs`: load entry, ribbon at startup, and the `[assembly: CommandClass]` list.
- `src/CadPack/Commands/`: one class per area (annotation, cleanup, layouts, selection, materials, ribbon).
- `src/CadPack/Ribbon/Tools.cs`: every ribbon button as data. `src/CadPack/Materials.cs`: material layers.
- `src/CadPack/Icons/`: embedded PNGs, kebab-case; material icons are `material-<slug of layer name>.png`.
- `lisp/`: the three routines; `lisp/support/`: their dialogs, slides, tables and help.
- `bundle/PackageContents.xml`: autoloader manifest.

## Conventions

- A command is a static method with `[CommandMethod("CPack_Name", ...)]` whose body runs in `Guard.Run`.
  A new command class also needs its own `[assembly: CommandClass]` line in `Plugin.cs`, or AutoCAD never finds it.
- A ribbon button is a row in `Tools.cs` plus a PNG in `Icons/`. The icon test fails on a missing or unused icon.
- AutoCAD commands in button macros are written `._NAME _Option`.
- Database edits go through one transaction, committed once. Selections pass `RejectObjectsOnLockedLayers`.
- LISP commands save the variables they change, install their own `*error*`, run in one UNDO group and restore on exit.
  Never redefine the global `*error*`. The routines are third-party: fixes only (`lisp/NOTICE.md`).
- Check any AutoCAD API or command name against Autodesk's 2027 documentation before using it. CI has no AutoCAD to catch a wrong one.
- Comments say what code does, never its history.

## Done means

- Both node commands pass, and the build has no errors and no new warnings.
- A behaviour change was tried in AutoCAD 2027, or the change says plainly that it was not.
- `README.md`, the command table in `docs/ARCHITECTURE.md` and `CHANGELOG.md` match the change.
