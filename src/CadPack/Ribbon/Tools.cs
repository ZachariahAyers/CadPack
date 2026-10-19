namespace CadPack.Ribbon;

/// <summary>A ribbon button: its label, the command line it sends, and its icon in Icons/.</summary>
internal sealed record Tool(string Label, string Command, string Icon);

/// <summary>
/// Everything on the CadPack tab except the materials. Rows read left to right; null draws a separator.
/// Commands starting with "._" are AutoCAD's own; the rest are defined by this plug-in or its LISP routines.
/// </summary>
internal static class Tools
{
    public static readonly Tool[] Solids =
    [
        new("Polysolid", "._POLYSOLID", "polysolid.png"),
        new("Box", "._BOX", "box.png"),
        new("Cylinder", "._CYLINDER", "cylinder.png"),
        new("Sphere", "._SPHERE", "sphere.png"),
        new("Cone", "._CONE", "cone.png"),
    ];

    public static readonly Tool[] Generators =
    [
        new("Steel\nMill", "CPack_SteelMill", "steel-mill.png"),
        new("Fastener\nBin", "CPack_Fasteners", "fastener-bin.png"),
        new("Weld\nSymbol", "CPack_Weld", "weld-symbol.png"),
    ];

    public static readonly Tool?[][] Edit =
    [
        [
            new("Presspull", "._PRESSPULL", "presspull.png"),
            new("Extrude", "._EXTRUDE", "extrude.png"),
            new("Extrude Faces", "._SOLIDEDIT _Face _Extrude", "extrude-faces.png"),
            new("Offset Faces", "._SOLIDEDIT _Face _Offset", "offset-faces.png"),
            null,
            new("Sweep", "._SWEEP", "sweep.png"),
            new("Revolve", "._REVOLVE", "revolve.png"),
            new("Loft", "._LOFT", "loft.png"),
            new("Shell", "._SOLIDEDIT _Body _Shell", "shell.png"),
        ],
        [
            new("Move Faces", "._SOLIDEDIT _Face _Move", "move-faces.png"),
            new("Delete Faces", "._SOLIDEDIT _Face _Delete", "delete-faces.png"),
            new("Rotate Faces", "._SOLIDEDIT _Face _Rotate", "rotate-faces.png"),
            new("Taper Faces", "._SOLIDEDIT _Face _Taper", "taper-faces.png"),
            null,
            new("Filter: Vertices", "_SUBOBJSELECTIONMODE 1", "filter-vertices.png"),
            new("Filter: Edges", "_SUBOBJSELECTIONMODE 2", "filter-edges.png"),
            new("Filter: Faces", "_SUBOBJSELECTIONMODE 3", "filter-faces.png"),
            new("Filter: None", "_SUBOBJSELECTIONMODE 0", "filter-none.png"),
        ],
        [
            new("Union", "._UNION", "union.png"),
            new("Subtract", "._SUBTRACT", "subtract.png"),
            new("Intersect", "._INTERSECT", "intersect.png"),
            null,
            new("Fillet Edge", "._FILLETEDGE", "fillet-edge.png"),
            new("Chamfer Edge", "._CHAMFEREDGE", "chamfer-edge.png"),
            new("Slice", "._SLICE", "slice.png"),
            new("Separate Solids", "._SOLIDEDIT _Body _Separate", "separate.png"),
            null,
            new("Extract Edges", "._XEDGES", "extract-edges.png"),
            new("Offset Edge", "._OFFSETEDGE", "offset-edge.png"),
            new("Check Interference", "._INTERFERE", "interference.png"),
        ],
    ];

    public static readonly Tool?[][] Annotation =
    [
        [
            new("Base View", "._VIEWBASE _Model", "base-view.png"),
            new("Projected View", "._VIEWPROJ", "projected-view.png"),
            new("Section View", "._VIEWSECTION", "section-view.png"),
            new("Detail View", "._VIEWDETAIL", "detail-view.png"),
            null,
            new("Linear Dimension", "._DIMLINEAR", "dim-linear.png"),
            new("Aligned Dimension", "._DIMALIGNED", "dim-aligned.png"),
            new("Angular Dimension", "._DIMANGULAR", "dim-angular.png"),
            new("Diameter Dimension", "._DIMDIAMETER", "dim-diameter.png"),
            null,
            new("Arc Length Dimension", "._DIMARC", "dim-arc.png"),
            new("Jogged Dimension", "._DIMJOGGED", "dim-jogged.png"),
            new("Ordinate Dimension", "._DIMORDINATE", "dim-ordinate.png"),
            null,
            new("Centerline", "._CENTERLINE", "centerline.png"),
            new("Center Mark", "._CENTERMARK", "center-mark.png"),
            new("Jog Line", "._DIMJOGLINE", "dim-jog-line.png"),
        ],
        [
            new("Quick Leader", "._QLEADER", "quick-leader.png"),
            new("Multileader", "._MLEADER", "multileader.png"),
            new("Wipeout", "._WIPEOUT", "wipeout.png"),
            new("Revision Cloud", "._REVCLOUD", "revision-cloud.png"),
            null,
            new("Dimension Suffix", "CPack_DimValueEdit", "dim-suffix.png"),
            new("Edit Dimension Text", "._DIMTEDIT", "dim-text-edit.png"),
            new("Flip Arrow", "._AIDIMFLIPARROW", "dim-flip-arrow.png"),
            new("Break Dimension", "._DIMBREAK", "dim-break.png"),
            null,
            new("Hide Extension Lines", "CPack_DimRemoveExtLines", "dim-hide-extension-lines.png"),
            new("Hide Dimension Lines", "CPack_DimRemoveDimLines", "dim-hide-dimension-lines.png"),
            new("Hide Arrows", "CPack_DimRemoveArrows", "dim-hide-arrows.png"),
            null,
            new("Reassociate Dimensions", "._DIMREASSOCIATE", "dim-reassociate.png"),
            new("Disassociate Dimensions", "._DIMDISASSOCIATE", "dim-disassociate.png"),
            new("Dimension Inspection", "._DIMINSPECT", "dim-inspect.png"),
        ],
        [
            new("Baseline Dimension", "._DIMBASELINE", "dim-baseline.png"),
            new("Continue Dimension", "._DIMCONTINUE", "dim-continue.png"),
            new("Update Dimensions", "._-DIMSTYLE _Apply", "dim-update.png"),
            new("Space Dimensions", "._DIMSPACE", "dim-space.png"),
            null,
            new("Precision 1/16", "._AIDIMPREC 4", "dim-precision-16.png"),
            new("Precision 1/32", "._AIDIMPREC 5", "dim-precision-32.png"),
            new("Precision 1/64", "._AIDIMPREC 6", "dim-precision-64.png"),
            null,
            new("Align Multileaders", "._MLEADERALIGN", "multileader-align.png"),
            new("Collect Multileaders", "._MLEADERCOLLECT", "multileader-collect.png"),
        ],
    ];

    public static readonly Tool?[][] Inspect =
    [
        [
            new("Measure Distance", "._MEASUREGEOM _Distance", "measure-distance.png"),
            new("Measure Radius", "._MEASUREGEOM _Radius", "measure-radius.png"),
            new("Measure Angle", "._MEASUREGEOM _Angle", "measure-angle.png"),
            new("Measure Volume", "._MEASUREGEOM _Volume", "measure-volume.png"),
        ],
        [
            new("Check Spelling", "._SPELL", "spell-check.png"),
            new("Find and Replace", "._FIND", "find.png"),
        ],
    ];

    public static readonly Tool[] Layouts =
    [
        new("Delete Other Layouts", "CPack_DeleteOtherLayouts", "delete-other-layouts.png"),
        new("Delete All Layouts", "CPack_DeleteAllLayouts", "delete-all-layouts.png"),
    ];

    public static readonly Tool[] Cleanup =
    [
        new("Clean Drawing", "CPack_CleanupDWG", "clean-drawing.png"),
        new("Purge", "CPack_Purge", "purge.png"),
        new("Audit", "CPack_Audit", "audit.png"),
        new("Clean Solids", "CPack_CleanSolids", "clean-solids.png"),
        new("Fix Lineweights", "CPack_LineweightFix", "fix-lineweights.png"),
        new("Fix Isolines", "CPack_IsolineFix", "fix-isolines.png"),
        new("Fix Viewports", "CPack_ViewportPreparation", "fix-viewports.png"),
    ];

    public static readonly Tool[] Selection =
    [
        new("Select 2D Elements", "CPack_Select2DElements", "select-2d.png"),
        new("Select Annotations", "CPack_SelectAnnoElements", "select-annotations.png"),
        new("Select Hatches", "CPack_SelectHatchElements", "select-hatches.png"),
        new("Select Blocks", "CPack_SelectBlockElements", "select-blocks.png"),
    ];
}
