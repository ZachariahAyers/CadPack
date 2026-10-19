# Architecture

CadPack is two things AutoCAD loads together: a .NET plug-in that owns the ribbon tab and the
CadPack commands, and three AutoLISP routines that draw steel shapes, fasteners and weld symbols.

## Loading

The build lays out an autoloader bundle next to its output:

```
CadPack.bundle/
  PackageContents.xml      from bundle/
  Contents/Windows/        CadPack.dll
  Contents/Lisp/           the three .lsp routines
  Contents/Support/        dialogs (.dcl), slides (.slb), shape and fastener tables (.dim), help text
```

AutoCAD scans the `ApplicationPlugins` folders at startup and reads `PackageContents.xml`. It loads
the DLL and the routines, and adds `Contents/Support` to the support path, which is how the
routines find their dialogs and tables by name. `SeriesMin="R26.0"` limits the bundle to AutoCAD 2027
and later, the first release on .NET 10.

## Plug-in

| File | Role |
|---|---|
| `Plugin.cs` | Load entry point and the list of command classes. Builds the ribbon on the first idle after the ribbon exists. |
| `Guard.cs` | Wraps every command body and prints AutoCAD errors on the command line. |
| `Ribbon/Tools.cs` | Every button on the tab as data: label, command line, icon. Rows read left to right; `null` is a separator. |
| `Materials.cs` | Material layers: name, AutoCAD color index, and the icon name derived from the name. |
| `Ribbon/RibbonBuilder.cs` | Turns the tables into ribbon panels, rows and split buttons. |
| `Ribbon/CommandHandler.cs` | Sends a button's command line to the active drawing after two cancels. |
| `Ribbon/Icons.cs` | Loads PNGs embedded from `Icons/` under their file names. |
| `Commands/*.cs` | One class per area. Each command edits through a single transaction. |

Buttons run AutoCAD's own commands as `._NAME _Option` (native command, English option names, so they
work in any language) or a CadPack command by name.

## Commands

| Command | What it does |
|---|---|
| `CPack_Ribbon` | Shows the CadPack tab. |
| `CPack_Material` | Makes a material's layer current, creating it with its color, then moves picked objects onto it. |
| `CPack_DimValueEdit` | Adds an O.D., I.D. or TYP. suffix to selected dimensions, or resets the text. |
| `CPack_DimRemoveExtLines`, `CPack_DimRemoveDimLines`, `CPack_DimRemoveArrows` | Hides extension lines, dimension lines or arrows on selected dimensions. |
| `CPack_CleanupDWG` | Runs every cleanup step below in order. |
| `CPack_Purge`, `CPack_Audit` | Purges everything including registered applications; audits and fixes errors. |
| `CPack_CleanSolids` | Runs CleanBody on every 3D solid. |
| `CPack_LineweightFix`, `CPack_IsolineFix` | Sets layer lineweights to Default; sets ISOLINES to 4. |
| `CPack_ViewportPreparation` | Puts layout viewports on DEFPOINTS, 2D Wireframe, hidden shade plot, locked and on. |
| `CPack_DeleteAllLayouts`, `CPack_DeleteOtherLayouts` | Deletes all paper layouts, or all but the current one. |
| `CPack_Select2DElements`, `CPack_SelectAnnoElements`, `CPack_SelectHatchElements`, `CPack_SelectBlockElements` | Selects every object of that kind. |
| `CPack_SteelMill`, `CPack_Fasteners`, `CPack_Weld` | The bundled AutoLISP routines. |

## AutoLISP routines

The routines in `lisp/` are third-party (see `lisp/NOTICE.md`). Each command saves the system variables
it changes, installs its own `*error*` handler, wraps its work in one UNDO group and restores
everything on exit or cancel. Changes to them stay limited to fixes.

## Checks

AutoCAD's API only runs inside AutoCAD, so CI cannot execute the commands. It checks what can be
checked without AutoCAD:

- `tools/check-lisp.mjs`: parens in `.lsp`, braces in `.dcl`, one list per `.dim` row, and every file
  a routine opens by name exists in `support/`.
- `tools/icons.test.mjs`: every icon the ribbon names is embedded, and every embedded icon is used.
- The Windows job builds the plug-in against the AutoCAD 2027 reference assemblies and uploads the
  bundle.

Behaviour inside AutoCAD is checked by loading the bundle in AutoCAD 2027.
