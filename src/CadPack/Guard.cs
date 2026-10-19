using System;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using AcadException = Autodesk.AutoCAD.Runtime.Exception;

namespace CadPack;

/// <summary>Runs a command body and reports AutoCAD errors on the command line instead of a crash dialog.</summary>
internal static class Guard
{
    public static void Run(Action body)
    {
        try
        {
            body();
        }
        catch (AcadException ex)
        {
            AcadApp.DocumentManager.MdiActiveDocument?.Editor.WriteMessage($"\nCadPack: {ex.Message}\n");
        }
    }
}
