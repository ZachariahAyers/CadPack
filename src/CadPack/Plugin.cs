using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;
using CadPack;
using CadPack.Commands;
using CadPack.Ribbon;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

// With CommandClass present AutoCAD scans only these types for commands, so every command class is listed.
[assembly: ExtensionApplication(typeof(Plugin))]
[assembly: CommandClass(typeof(AnnotationCommands))]
[assembly: CommandClass(typeof(CleanupCommands))]
[assembly: CommandClass(typeof(LayoutCommands))]
[assembly: CommandClass(typeof(SelectionCommands))]
[assembly: CommandClass(typeof(MaterialCommands))]
[assembly: CommandClass(typeof(RibbonCommands))]

namespace CadPack;

/// <summary>Shows the CadPack ribbon tab when AutoCAD loads the plug-in.</summary>
public sealed class Plugin : IExtensionApplication
{
    public void Initialize() => AcadApp.Idle += OnIdle;

    public void Terminate() => AcadApp.Idle -= OnIdle;

    // The ribbon does not exist yet while Initialize runs; build the tab on the first idle after it does.
    private static void OnIdle(object? sender, EventArgs e)
    {
        if (ComponentManager.Ribbon is null) return;
        AcadApp.Idle -= OnIdle;
        Guard.Run(RibbonBuilder.Show);
    }
}
