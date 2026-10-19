# Changelog

## v2.0.0 - 2026-10-19

Built for current AutoCAD.

### Changed

- Targets AutoCAD 2027 and .NET 10 through Autodesk's `AutoCAD.NET` package.
- Ships as an autoloader bundle that loads the plug-in and the LISP routines and sets their support path.
- The ribbon builds itself at startup. Its buttons and material layers are data tables.
- The single 2,750-line command file is split by area, with one error guard for every command.
- Icons are embedded PNGs with readable names instead of a generated resource file.
- Material buttons run `CPack_Material` instead of a raw `-LAYER` macro.
- Each LISP command is one undo step.
- CI runs the LISP and icon checks and builds the bundle; Dependabot keeps the actions current.

### Fixed

- The TYP. dimension suffix never applied.
- The Dimension Value Edit button called a command that does not exist.
- Deleting layouts walked the layout dictionary while removing from it.
- Viewport preparation left layer 0 current.
- One solid on a locked layer stopped Clean Solids for the whole drawing.
- The Steel Mill and Fastener Bin error handlers had their cancel test inverted.
- The Weld routine replaced AutoCAD's global error handler when it loaded.
- The Fastener Bin set ISOLINES to 0 on exit instead of restoring it.
- The Weld routine no longer sets the obsolete BLIPMODE variable.

### Removed

- 23 ribbon buttons that called commands defined outside this project.
- The unused Visual Studio template project, a zip duplicating the LISP files, unused data copies,
  placeholder icons and the Windows thumbnail cache.
