using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;

namespace CadPack.Commands;

internal static class Layers
{
    /// <summary>Returns the layer with this name, creating it with the given AutoCAD color index if missing.</summary>
    public static ObjectId Ensure(Transaction tr, Database db, string name, short colorIndex)
    {
        var table = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
        if (table.Has(name)) return table[name];

        var layer = new LayerTableRecord
        {
            Name = name,
            Color = Color.FromColorIndex(ColorMethod.ByAci, colorIndex),
        };
        table.UpgradeOpen();
        var id = table.Add(layer);
        tr.AddNewlyCreatedDBObject(layer, true);
        return id;
    }
}
