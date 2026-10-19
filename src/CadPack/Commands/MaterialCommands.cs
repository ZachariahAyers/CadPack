using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CadPack.Commands;

/// <summary>Material layers: one layer per stock material, named and colored by the Materials table.</summary>
public sealed class MaterialCommands
{
    /// <summary>Makes a material's layer current, then moves any objects picked next onto it.</summary>
    [CommandMethod("CPack_Material", CommandFlags.Modal)]
    public static void ApplyMaterial() => Guard.Run(() =>
    {
        var doc = AcadApp.DocumentManager.MdiActiveDocument;
        var editor = doc.Editor;
        var db = doc.Database;

        var input = editor.GetString(new PromptStringOptions("\nMaterial name: ") { AllowSpaces = true });
        if (input.Status != PromptStatus.OK) return;
        var material = Materials.Find(input.StringResult);
        if (material is null)
        {
            editor.WriteMessage($"\nUnknown material: {input.StringResult}\n");
            return;
        }

        ObjectId layerId;
        using (var tr = db.TransactionManager.StartTransaction())
        {
            layerId = Layers.Ensure(tr, db, material.Name, material.Color);
            db.Clayer = layerId;
            tr.Commit();
        }

        var picked = editor.GetSelection(new PromptSelectionOptions
        {
            MessageForAdding = $"\nSelect objects to move to {material.Name}, or Enter for none: ",
            RejectObjectsOnLockedLayers = true,
        });
        if (picked.Status != PromptStatus.OK) return;

        using (var tr = db.TransactionManager.StartTransaction())
        {
            foreach (var id in picked.Value.GetObjectIds())
            {
                ((Entity)tr.GetObject(id, OpenMode.ForWrite)).LayerId = layerId;
            }
            tr.Commit();
        }
    });
}
