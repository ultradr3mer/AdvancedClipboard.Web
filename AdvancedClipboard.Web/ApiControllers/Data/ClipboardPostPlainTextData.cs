using System;

namespace AdvancedClipboard.Web.ApiControllers.Data
{
  public class ClipboardPostPlainTextData
  {
    public string Content { get; set; }
    public Guid? LaneGuid { get; set; }
  }
}