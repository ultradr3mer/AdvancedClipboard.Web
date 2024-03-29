﻿namespace AdvancedClipboard.Web.Controllers.Model
{
  public class LaneDetailsModel
  {
    public Guid Id { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool HasItems { get; set; }
  }
}
