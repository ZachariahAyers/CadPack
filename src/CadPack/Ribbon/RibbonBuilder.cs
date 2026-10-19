using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Autodesk.Windows;

namespace CadPack.Ribbon;

/// <summary>Builds the CadPack ribbon tab from <see cref="Tools"/> and <see cref="Materials"/>.</summary>
internal static class RibbonBuilder
{
    private const string TabId = "CadPack";

    /// <summary>Adds the tab if it is missing and makes it active.</summary>
    public static void Show()
    {
        var ribbon = ComponentManager.Ribbon;
        if (ribbon is null) return;

        var tab = ribbon.FindTab(TabId);
        if (tab is null)
        {
            tab = Build();
            ribbon.Tabs.Add(tab);
        }
        tab.IsActive = true;
    }

    private static RibbonTab Build()
    {
        var tab = new RibbonTab { Title = "CadPack", Id = TabId };
        tab.Panels.Add(Panel("Create", Row(Tools.Generators.Select(Large).Prepend<RibbonItem>(Split("Solid\nBodies", RibbonItemSize.Large, Tools.Solids)))));
        tab.Panels.Add(Panel("Edit", Tools.Edit.Select(SmallRow)));
        tab.Panels.Add(Panel("Annotation", Tools.Annotation.Select(SmallRow)));
        tab.Panels.Add(Panel("Inspect", Tools.Inspect.Select(SmallRow)));
        tab.Panels.Add(Panel("Materials", Row(Materials.Groups.Select(MaterialSplit))));
        tab.Panels.Add(Panel("Cleanup",
            Row(Split("Delete", RibbonItemSize.Standard, Tools.Layouts)),
            Row(Split("Clean", RibbonItemSize.Standard, Tools.Cleanup)),
            Row(Split("Select", RibbonItemSize.Standard, Tools.Selection))));
        return tab;
    }

    private static RibbonPanel Panel(string title, params IEnumerable<RibbonRowPanel> rows)
    {
        var source = new RibbonPanelSource { Title = title };
        foreach (var row in rows)
        {
            if (source.Items.Count > 0) source.Items.Add(new RibbonRowBreak());
            source.Items.Add(row);
        }
        return new RibbonPanel { Source = source };
    }

    private static RibbonRowPanel Row(params IEnumerable<RibbonItem> items)
    {
        var row = new RibbonRowPanel();
        foreach (var item in items) row.Items.Add(item);
        return row;
    }

    private static RibbonRowPanel SmallRow(Tool?[] tools) =>
        Row(tools.Select(t => t is null ? new RibbonSeparator { SeparatorStyle = RibbonSeparatorStyle.Line } : Small(t)));

    private static RibbonItem Small(Tool tool) => Button(tool, RibbonItemSize.Standard, showText: false);

    private static RibbonItem Large(Tool tool) => Button(tool, RibbonItemSize.Large, showText: true);

    private static RibbonButton Button(Tool tool, RibbonItemSize size, bool showText)
    {
        var icon = Icons.Load(tool.Icon);
        return new RibbonButton
        {
            Text = tool.Label,
            ToolTip = tool.Label.Replace('\n', ' '),
            ShowText = showText,
            ShowImage = true,
            Image = icon,
            LargeImage = icon,
            Size = size,
            Orientation = size == RibbonItemSize.Large ? Orientation.Vertical : Orientation.Horizontal,
            CommandHandler = CommandHandler.Instance,
            CommandParameter = tool.Command,
        };
    }

    private static RibbonSplitButton Split(string label, RibbonItemSize size, IEnumerable<Tool> tools)
    {
        var split = SplitButton(label, size);
        foreach (var tool in tools) split.Items.Add(Button(tool, size, showText: true));
        return split;
    }

    private static RibbonItem MaterialSplit(MaterialGroup group)
    {
        var split = SplitButton(group.Label, RibbonItemSize.Large);
        foreach (var material in group.Items)
        {
            split.Items.Add(Button(new Tool(material.Name, $"CPack_Material\n{material.Name}", material.Icon), RibbonItemSize.Large, showText: true));
        }
        return split;
    }

    private static RibbonSplitButton SplitButton(string label, RibbonItemSize size) => new()
    {
        Text = label,
        ShowText = true,
        Size = size,
        Orientation = size == RibbonItemSize.Large ? Orientation.Vertical : Orientation.Horizontal,
        SynchronizeOption = RibbonListButton.RibbonListButtonSynchronizeOption.Image,
    };
}
