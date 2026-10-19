using System;
using System.Windows.Input;
using Autodesk.Windows;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace CadPack.Ribbon;

/// <summary>Sends a button's command line to the active drawing, cancelling any running command first.</summary>
internal sealed class CommandHandler : ICommand
{
    public static readonly CommandHandler Instance = new();

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if (parameter is RibbonCommandItem { CommandParameter: string command })
        {
            AcadApp.DocumentManager.MdiActiveDocument?.SendStringToExecute($"\u0003\u0003{command}\n", true, false, true);
        }
    }
}
