using System.Collections.Generic;

namespace AdvancedClipboard.Web.ApiControllers.Data
{
  public class ClipboardContainerGetData
  {
    #region Properties

    public List<LaneGetData> Lanes { get; set; } = new List<LaneGetData>();
    public List<ClipboardGetData> Entries { get; set; } = new List<ClipboardGetData>();

    #endregion Properties
  }
}