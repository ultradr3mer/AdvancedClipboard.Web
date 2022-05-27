using AdvancedClipboard.Web.ApiControllers.Data;

namespace AdvancedClipboard.Web.Controllers.Model
{
  public class HomeIndexModel
  {
    public List<LaneDisplayData> Lanes { get; set; } = new List<LaneDisplayData>();

    public List<ClipboardGetData> Entries { get; set; } = new List<ClipboardGetData>();

    public string? ContentToAdd { get; set; }

    public string? ReturnUrl { get; set; }

    public Guid? CurrentLaneId { get; set; }
  }
}
