using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CadPack.Commands;

/// <summary>Deletes layout tabs.</summary>
public sealed class LayoutCommands
{
    /// <summary>Deletes every paper layout. AutoCAD keeps one empty layout, as it always must.</summary>
    [CommandMethod("CPack_DeleteAllLayouts", CommandFlags.Modal)]
    public static void DeleteAll() => Guard.Run(() =>
    {
        Delete(_ => true);
        LayoutManager.Current.CurrentLayout = "Model";
        Write("\nAll layouts deleted.\n");
    });

    /// <summary>Deletes every paper layout except the current one.</summary>
    [CommandMethod("CPack_DeleteOtherLayouts", CommandFlags.Modal)]
    public static void DeleteOthers() => Guard.Run(() =>
    {
        var current = LayoutManager.Current.CurrentLayout;
        Delete(name => name != current);
        Write("\nOther layouts deleted.\n");
    });

    // Names are collected first; deleting while walking the layout dictionary would invalidate it.
    private static void Delete(Func<string, bool> shouldDelete)
    {
        var db = AcadApp.DocumentManager.MdiActiveDocument.Database;
        string[] names;
        using (var tr = db.TransactionManager.StartTransaction())
        {
            names = PaperLayouts(tr, db).Select(l => l.LayoutName).Where(shouldDelete).ToArray();
            tr.Commit();
        }
        foreach (var name in names)
        {
            LayoutManager.Current.DeleteLayout(name);
        }
    }

    private static void Write(string message) =>
        AcadApp.DocumentManager.MdiActiveDocument.Editor.WriteMessage(message);

    internal static List<Layout> PaperLayouts(Transaction tr, Database db)
    {
        var layouts = new List<Layout>();
        var dictionary = (DBDictionary)tr.GetObject(db.LayoutDictionaryId, OpenMode.ForRead);
        foreach (DictionaryEntry entry in dictionary)
        {
            var layout = (Layout)tr.GetObject((ObjectId)entry.Value!, OpenMode.ForRead);
            if (!layout.ModelType) layouts.Add(layout);
        }
        return layouts;
    }
}
