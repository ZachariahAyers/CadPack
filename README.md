# CadPack

[![CI](https://github.com/ZachariahAyers/CadPack/actions/workflows/ci.yml/badge.svg)](https://github.com/ZachariahAyers/CadPack/actions/workflows/ci.yml)

An AutoCAD 2027 plug-in that puts a 3D detailer's everyday tools on one ribbon tab.

## Features

- Solid modeling, face and edge editing, drawing views, dimensions, leaders and measuring on one tab.
- Material layers: pick a plywood, MDF, laminate, PVC edge, countertop, steel plate or tube stock to make its layer current and move parts onto it.
- Dimension fixes in bulk: O.D., I.D. and TYP. suffixes, and hidden extension lines, dimension lines or arrows.
- Drawing cleanup: purge, audit, solid repair, lineweight, isoline and viewport fixes, layout deletion and select-by-type.
- Steel shapes, fasteners and weld symbols from bundled AutoLISP routines.

## Tech stack

C#, .NET 10, AutoCAD .NET API (AutoCAD 2027), AutoLISP and DCL, Node.js checks, GitHub Actions.

## Getting started

Requires AutoCAD 2027 on 64-bit Windows, and the .NET 10 SDK to build.

```bash
git clone https://github.com/ZachariahAyers/CadPack.git
dotnet build CadPack/src/CadPack/CadPack.csproj --configuration Release
```

Copy `src/CadPack/bin/Release/net10.0-windows/CadPack.bundle` into `%APPDATA%\Autodesk\ApplicationPlugins`
and start AutoCAD. The CadPack tab appears on the ribbon; `CPack_Ribbon` brings it back if a workspace
change hides it. Each CI run also publishes the bundle as a build artifact.

## Development

```bash
node --test 'tools/*.test.mjs'     # LISP structure and ribbon icon tests
node tools/check-lisp.mjs lisp     # parens, dialog braces, data rows, support files
dotnet build src/CadPack/CadPack.csproj
```

How it is put together, with the full command list: [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## License

GNU General Public License v3.0. See [LICENSE](LICENSE). The bundled AutoLISP routines are third-party;
see [lisp/NOTICE.md](lisp/NOTICE.md).

---

Built by [Zachariah Ayers](https://ayers.repair).
