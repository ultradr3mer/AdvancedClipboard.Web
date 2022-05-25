using System.Drawing;

namespace AdvancedClipboard.Wpf.Extensions
{
  public static class ColorExtensions
  {
    #region Methods

    public static double CalculateLuminance(this Color color)
    {
      return (0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B) / byte.MaxValue;
    }

    public static string ToHexString(this Color c) => $"#{c.R:X2}{c.G:X2}{c.B:X2}";

    internal static Color FromHex(string color)
    {
      return ColorTranslator.FromHtml(color);
    }

    #endregion Methods
  }
}