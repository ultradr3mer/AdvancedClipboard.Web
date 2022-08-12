using AdvancedClipboard.Web.ApiControllers.Data;

namespace AdvancedClipboard.Web.Controllers.Model
{
  public class ClipboardCardModel
  {
    public HomeIndexModel IndexModel { get; set; } = new HomeIndexModel();

    public ClipboardGetData Entry { get; set; } = new ClipboardGetData();
  }
}
