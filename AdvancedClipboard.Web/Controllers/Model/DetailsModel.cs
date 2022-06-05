using AdvancedClipboard.Web.ApiControllers.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdvancedClipboard.Web.Controllers.Model
{
  public class DetailsModel
  {
    public SelectList Lanes { get; set; } = new SelectList(Enumerable.Empty<LaneGetData>());

    public Guid ContentTypeId { get; set; }

    public string? FileContentUrl { get; set; }

    public Guid Id { get; set; }

    public string? TextContent { get; set; }

    public string? FileName { get; set; }

    public Guid? LaneId { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;
  }
}
