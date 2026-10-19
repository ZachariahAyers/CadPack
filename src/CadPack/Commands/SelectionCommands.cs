using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CadPack.Commands;

/// <summary>Selects every object of one kind so it can be deleted or moved in one step.</summary>
public sealed class SelectionCommands
{
    [CommandMethod("CPack_Select2DElements")]
    public static void Select2D() => Guard.Run(() => SelectAll("*LINE,CIRCLE,ARC,ELLIPSE,POINT,RAY"));

    [CommandMethod("CPack_SelectAnnoElements")]
    public static void SelectAnnotations() => Guard.Run(() => SelectAll("*DIMENSION,*TEXT,*LEADER,*TABLE"));

    [CommandMethod("CPack_SelectBlockElements")]
    public static void SelectBlocks() => Guard.Run(() => SelectAll("INSERT"));

    [CommandMethod("CPack_SelectHatchElements")]
    public static void SelectHatches() => Guard.Run(() => SelectAll("HATCH"));

    // Leaves the matches as the pickfirst selection.
    private static void SelectAll(string entityTypes)
    {
        var editor = AcadApp.DocumentManager.MdiActiveDocument.Editor;
        var result = editor.SelectAll(new SelectionFilter(new[] { new TypedValue((int)DxfCode.Start, entityTypes) }));
        if (result.Status == PromptStatus.OK)
        {
            editor.SetImpliedSelection(result.Value);
        }
        else
        {
            editor.WriteMessage("\nNone in the drawing.\n");
        }
    }
}
