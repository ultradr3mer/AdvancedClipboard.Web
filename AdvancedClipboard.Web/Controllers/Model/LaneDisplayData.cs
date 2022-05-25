using AdvancedClipboard.Web.ApiControllers.Data;
using AdvancedClipboard.Wpf.Extensions;
using Mapster;

namespace AdvancedClipboard.Web.Controllers.Model
{
  public class LaneDisplayData : LaneGetData
  {
    public LaneDisplayData(LaneGetData data)
    {
      TypeAdapter.Adapt(data, this);

      var color = ColorExtensions.FromHex(data.Color);
      double luminance = color.CalculateLuminance();
      this.TextColor = luminance > 0.6 ? "black" : "white";
    }

    public string TextColor { get; set; }
  }
}
