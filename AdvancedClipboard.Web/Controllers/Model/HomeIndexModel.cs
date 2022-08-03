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

    public DetailsModel ClipboardData { get; set; } = new DetailsModel();

    public LaneDetailsModel LaneData { get; set; } = new LaneDetailsModel();

    public IFormFile? File { set; get; }

    public string FileFilter { get; set; } = string.Empty;

    public string SearchText { get; set; } = string.Empty;
  }
}
