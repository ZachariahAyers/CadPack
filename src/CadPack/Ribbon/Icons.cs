using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CadPack.Ribbon;

/// <summary>Loads ribbon icons embedded from Icons/ under their file names.</summary>
internal static class Icons
{
    public static ImageSource Load(string file)
    {
        using var stream = typeof(Icons).Assembly.GetManifestResourceStream(file)
            ?? throw new InvalidOperationException($"Icon not embedded: {file}");
        var image = new BitmapImage();
        image.BeginInit();
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.StreamSource = stream;
        image.EndInit();
        image.Freeze();
        return image;
    }
}
