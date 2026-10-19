using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CadPack.Commands;

/// <summary>Bulk edits on selected dimensions.</summary>
public sealed class AnnotationCommands
{
    private static readonly SelectionFilter DimensionsOnly =
        new(new[] { new TypedValue((int)DxfCode.Start, "DIMENSION") });

    /// <summary>Adds an O.D., I.D. or TYP. suffix to the measured value, or resets the text.</summary>
    [CommandMethod("CPack_DimValueEdit", CommandFlags.Modal | CommandFlags.Redraw)]
    public static void DimensionSuffix() => Guard.Run(() =>
    {
        var ids = SelectDimensions("\nSelect dimensions to edit: ");
        if (ids is null) return;

        var options = new PromptKeywordOptions("\nDimension suffix") { AllowNone = false };
        options.Keywords.Add("OD");
        options.Keywords.Add("ID");
        options.Keywords.Add("TYP");
        options.Keywords.Add("Reset");
        var choice = Editor.GetKeywords(options);
        if (choice.Status != PromptStatus.OK) return;

        var text = choice.StringResult switch
        {
            "OD" => "<> O.D.",
            "ID" => "<> I.D.",
            "TYP" => "<> TYP.",
            _ => "<>",
        };
        Edit(ids, dim => dim.DimensionText = text);
    });

    [CommandMethod("CPack_DimRemoveExtLines", CommandFlags.Modal | CommandFlags.Redraw)]
    public static void HideExtensionLines() => Guard.Run(() =>
        Edit(SelectDimensions("\nSelect dimensions: "), dim =>
        {
            dim.Dimse1 = true;
            dim.Dimse2 = true;
        }));

    [CommandMethod("CPack_DimRemoveDimLines", CommandFlags.Modal | CommandFlags.Redraw)]
    public static void HideDimensionLines() => Guard.Run(() =>
        Edit(SelectDimensions("\nSelect dimensions: "), dim =>
        {
            dim.Dimsd1 = true;
            dim.Dimsd2 = true;
        }));

    [CommandMethod("CPack_DimRemoveArrows", CommandFlags.Modal | CommandFlags.Redraw)]
    public static void HideArrows() => Guard.Run(() =>
        Edit(SelectDimensions("\nSelect dimensions: "), dim => dim.Dimasz = 0));

    private static Editor Editor => AcadApp.DocumentManager.MdiActiveDocument.Editor;

    private static ObjectId[]? SelectDimensions(string prompt)
    {
        var options = new PromptSelectionOptions
        {
            MessageForAdding = prompt,
            MessageForRemoval = "\nOnly dimensions can be selected.",
            RejectObjectsOnLockedLayers = true,
        };
        var result = Editor.GetSelection(options, DimensionsOnly);
        return result.Status == PromptStatus.OK ? result.Value.GetObjectIds() : null;
    }

    private static void Edit(ObjectId[]? ids, Action<Dimension> edit)
    {
        if (ids is null) return;
        var db = AcadApp.DocumentManager.MdiActiveDocument.Database;
        using var tr = db.TransactionManager.StartTransaction();
        foreach (var id in ids)
        {
            edit((Dimension)tr.GetObject(id, OpenMode.ForWrite));
        }
        tr.Commit();
    }
}
