using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CadPack;

/// <summary>A stock material: its layer name and AutoCAD color index.</summary>
internal sealed record Material(string Name, short Color)
{
    /// <summary>Icons/material-&lt;slug&gt;.png, where the slug is the name in lower case with runs of other characters as one dash.</summary>
    public string Icon => $"material-{Regex.Replace(Name.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-')}.png";
}

internal sealed record MaterialGroup(string Label, Material[] Items);

/// <summary>The material layers on the ribbon, in ribbon order.</summary>
internal static class Materials
{
    public static readonly MaterialGroup[] Groups =
    [
        new("Raw Ply",
        [
            new("Plywood - Bending (.35)", 30),
            new("Plywood - Raw (.25)", 20),
            new("Plywood - Raw (.50)", 110),
            new("Plywood - Raw (.75)", 41),
        ]),
        new("Raw MDF",
        [
            new("MDF - Raw (.125)", 204),
            new("MDF - Raw (.25)", 196),
            new("MDF - Raw (.3125)", 113),
            new("MDF - Raw (.375)", 21),
            new("MDF - Raw (.50)", 142),
            new("MDF - Raw (.625)", 42),
            new("MDF - Raw (.75)", 151),
            new("MDF - Raw (1.0)", 112),
        ]),
        new("Lam MDF",
        [
            new("MDF Laminated - xxx (.5625) - Grain", 55),
            new("MDF Laminated - xxx (.8125) - Grain", 14),
            new("MDF Laminated - xxx (.875) - Grain", 33),
            new("MDF Laminated - xxx (1.125) - Grain", 170),
            new("MDF Laminated - xxx (.5625) - Solid", 105),
            new("MDF Laminated - xxx (.8125) - Solid", 252),
            new("MDF Laminated - xxx (.875) - Solid", 96),
            new("MDF Laminated - xxx (1.125) - Solid", 174),
        ]),
        new("Post Lam",
        [
            new("Post Laminate - xxx - Grain", 14),
            new("Post Laminate - xxx - Solid", 252),
        ]),
        new("PVC Edge",
        [
            new("PVC Edge - xxx (.02) .5MM", 71),
            new("PVC Edge - xxx (.04) 1MM", 72),
            new("PVC Edge - xxx (.125) 3MM", 41),
        ]),
        new("Countertop",
        [
            new("Countertop - Dark (.50)", 185),
            new("Countertop - Light (.50)", 253),
        ]),
        new("Steel",
        [
            new("Steel Plate (.1875)", 124),
            new("Steel Plate (.125)", 14),
            new("Steel Plate (.25)", 141),
            new("Steel Plate - 11 Gauge", 85),
            new("Steel Plate - 12 Gauge", 225),
            new("Steel Plate - 13 Gauge", 202),
            new("Steel Plate - 14 Gauge", 136),
            new("Steel Plate - 16 Gauge", 145),
            new("Steel Plate - 18 Gauge", 84),
            new("Steel Plate - 20 Gauge", 54),
        ]),
        new("S. Steel",
        [
            new("S. Steel Plate (.1875)", 130),
            new("S. Steel Plate (.125)", 151),
            new("S. Steel Plate (.25)", 17),
            new("S. Steel Plate - 11 Gauge", 83),
            new("S. Steel Plate - 12 Gauge", 30),
            new("S. Steel Plate - 13 Gauge", 201),
            new("S. Steel Plate - 14 Gauge", 136),
            new("S. Steel Plate - 16 Gauge", 170),
            new("S. Steel Plate - 18 Gauge", 155),
            new("S. Steel Plate - 20 Gauge", 52),
        ]),
        new("Tubing",
        [
            new("Steel Tube - 11 Gauge", 193),
            new("Steel Tube - 13 Gauge", 120),
            new("Steel Tube - 14 Gauge", 132),
            new("Steel Tube - 16 Gauge", 140),
        ]),
    ];

    public static Material? Find(string name) =>
        Groups.SelectMany(g => g.Items).FirstOrDefault(m => string.Equals(m.Name, name.Trim(), StringComparison.OrdinalIgnoreCase));
}
