# Changelog

## v2.0.0 - 2026-10-19

AutoCAD 2027 ribbon plug-in for 3D detailing, with three bundled AutoLISP routines.

### Added

- Targets AutoCAD 2027 and .NET 10 through Autodesk's `AutoCAD.NET` package.
- Ships as an autoloader bundle that loads the plug-in and the LISP routines and sets their support path.
- The ribbon builds itself at startup. Its buttons and material layers are data tables.
- Commands are split by area, with one error guard for every command.
- Icons are embedded PNGs with readable names.
- Material buttons run `CPack_Material`.
- Each LISP command is one undo step and restores the variables it changes.
- CI runs the LISP and icon checks and builds the bundle; Dependabot keeps the actions current.
