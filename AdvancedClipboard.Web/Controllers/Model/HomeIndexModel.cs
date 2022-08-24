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

    public List<ClipboardGetData> Pinned { get; set; } = new List<ClipboardGetData>();
    public Dictionary<Guid, string> LaneColors { get; set; } = new Dictionary<Guid, string>();
    public string LaneStyle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FullPath { get; internal set; } = string.Empty;

    internal void Initialize()
    {
      this.LaneColors = this.Lanes.Where(o => o.Id != null).ToDictionary(o => o.Id!.Value, o => o.Color);
      this.LaneColors.Add(Guid.Empty, "transparent");

      this.LaneStyle = $"background-color: #303030;";
      this.Title = "Lanes";
      if (this.CurrentLaneId != null)
      {
        LaneDisplayData? lane = this.Lanes.Single(o => o.Id == this.CurrentLaneId);
        LaneStyle = $"background-color: {lane.Color}55; border-bottom: .45em solid {lane.Color};";
        Title = lane.Name;
      }
    }
  }
}
