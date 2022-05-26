using AdvancedClipboard.Web.ApiControllers.Data;

namespace AdvancedClipboard.Web.Controllers.Model
{
  public class HomeIndexModel
  {
    public List<LaneDisplayData> Lanes { get; internal set; }
    public List<ClipboardGetData> Entries { get; internal set; }

    public string ContentToAdd { get; set; }

    public string ReturnUrl { get; set; }
  }
}
