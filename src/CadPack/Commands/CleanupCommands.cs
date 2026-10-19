using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CadPack.Commands;

/// <summary>Drawing cleanup: purge, audit, display fixes and solid repair.</summary>
public sealed class CleanupCommands
{
    /// <summary>Runs every cleanup step in order.</summary>
    [CommandMethod("CPack_CleanupDWG", CommandFlags.Modal)]
    public static void CleanDrawing() => Guard.Run(() =>
    {
        FixIsolines();
        FixLineweights();
        CleanSolids();
        PrepareViewports();
        Purge();
        Audit();
    });

    [CommandMethod("CPack_Purge", CommandFlags.Modal)]
    public static void Purge() => Guard.Run(() =>
    {
        Editor.Command("._-PURGE", "_All", "*", "_No");
        Editor.Command("._-PURGE", "_Regapps", "*", "_No");
        Editor.WriteMessage("\nDrawing purged.\n");
    });

    [CommandMethod("CPack_Audit", CommandFlags.Modal)]
    public static void Audit() => Guard.Run(() =>
    {
        Editor.Command("._AUDIT", "_Yes");
        Editor.WriteMessage("\nDrawing audited and errors fixed.\n");
    });

    /// <summary>Resets ISOLINES to the AutoCAD default of 4.</summary>
    [CommandMethod("CPack_IsolineFix", CommandFlags.Modal)]
    public static void FixIsolines() => Guard.Run(() =>
    {
        Editor.Command("._ISOLINES", "4");
        Editor.Command("._REGENALL");
        Editor.WriteMessage("\nIsolines set to 4.\n");
    });

    /// <summary>Sets every layer's lineweight to Default.</summary>
    [CommandMethod("CPack_LineweightFix", CommandFlags.Modal)]
    public static void FixLineweights() => Guard.Run(() =>
    {
        Editor.Command("._-LAYER", "_LWeight", "_Default", "*", "");
        Editor.WriteMessage("\nLayer lineweights set to Default.\n");
    });

    /// <summary>Puts every layout viewport on DEFPOINTS, 2D Wireframe, hidden shade plot, locked and on.</summary>
    [CommandMethod("CPack_ViewportPreparation", CommandFlags.Modal)]
    public static void PrepareViewports() => Guard.Run(() =>
    {
        var db = AcadApp.DocumentManager.MdiActiveDocument.Database;
        var layouts = LayoutManager.Current;
        var previous = layouts.CurrentLayout;

        try
        {
            using var tr = db.TransactionManager.StartTransaction();
            var defpoints = Layers.Ensure(tr, db, "DEFPOINTS", 7);
            var styles = (DBDictionary)tr.GetObject(db.VisualStyleDictionaryId, OpenMode.ForRead);
            var wireframe = styles.GetAt("2DWireframe");

            // Each layout is made current while its viewports are edited.
            foreach (var layout in LayoutCommands.PaperLayouts(tr, db))
            {
                layouts.CurrentLayout = layout.LayoutName;
                var space = (BlockTableRecord)tr.GetObject(layout.BlockTableRecordId, OpenMode.ForRead);
                foreach (ObjectId id in space)
                {
                    if (tr.GetObject(id, OpenMode.ForRead) is not Viewport vp) continue;
                    vp.UpgradeOpen();
                    vp.LayerId = defpoints;
                    vp.VisualStyleId = wireframe;
                    vp.ShadePlot = ShadePlotType.Hidden;
                    vp.Locked = true;
                    vp.On = true;
                    vp.UpdateDisplay();
                }
            }
            tr.Commit();
        }
        finally
        {
            layouts.CurrentLayout = previous;
        }
        Editor.WriteMessage("\nViewports updated.\n");
    });

    /// <summary>Runs CleanBody on every 3D solid in the drawing.</summary>
    [CommandMethod("CPack_CleanSolids", CommandFlags.Modal)]
    public static void CleanSolids() => Guard.Run(() =>
    {
        var result = Editor.SelectAll(new SelectionFilter(new[] { new TypedValue((int)DxfCode.Start, "3DSOLID") }));
        if (result.Status != PromptStatus.OK)
        {
            Editor.WriteMessage("\nNo 3D solids in the drawing. Solids inside blocks are not cleaned.\n");
            return;
        }

        var db = AcadApp.DocumentManager.MdiActiveDocument.Database;
        using var tr = db.TransactionManager.StartTransaction();
        foreach (var id in result.Value.GetObjectIds())
        {
            ((Solid3d)tr.GetObject(id, OpenMode.ForWrite, false, true)).CleanBody();
        }
        tr.Commit();
        Editor.WriteMessage($"\nCleaned {result.Value.Count} solids.\n");
    });

    private static Editor Editor => AcadApp.DocumentManager.MdiActiveDocument.Editor;
}
