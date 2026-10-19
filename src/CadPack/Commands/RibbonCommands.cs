using Autodesk.AutoCAD.Runtime;
using CadPack.Ribbon;

namespace CadPack.Commands;

public sealed class RibbonCommands
{
    /// <summary>Shows the CadPack tab again after a workspace change has removed it.</summary>
    [CommandMethod("CPack_Ribbon", CommandFlags.Transparent)]
    public static void ShowRibbon() => Guard.Run(RibbonBuilder.Show);
}
