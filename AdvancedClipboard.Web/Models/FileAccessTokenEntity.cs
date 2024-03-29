﻿using System.ComponentModel.DataAnnotations;

namespace AdvancedClipboard.Web.Models
{
  /// <summary>
  /// A file access token.
  /// </summary>
  public class FileAccessTokenEntity
  {
    #region Properties

    /// <summary>
    /// The filename.
    /// </summary>
    public string Filename { get; set; } = string.Empty;

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The token.
    /// </summary>
    public long Token { get; set; }

    /// <summary>
    /// The user that uploaded the file.
    /// </summary>
    public Guid UserId { get; set; }

    #endregion Properties
  }
}
