using AdvancedClipboard.Server.Constants;
using AdvancedClipboard.Web.Models;
using System;

namespace AdvancedClipboard.Web.ApiControllers.Data
{
  public class ClipboardGetData
  {
    #region Constructors

    public ClipboardGetData()
    {
    }

    #endregion Constructors

    #region Properties

    public Guid ContentTypeId { get; private set; }
    public string FileContentUrl { get; private set; } = string.Empty;
    public Guid Id { get; private set; }
    public string TextContent { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;

    public string MimeType { get; private set; } = string.Empty;
    public Guid? LaneId { get; private set; }
    public bool IsPinned { get; private set; }

    #endregion Properties

    #region Methods

    public static ClipboardGetData CreateWithFileContent(ClipboardContentEntity cc, FileAccessTokenEntity fileToken)
    {
      var result = CreateBase(cc);
      result.FileContentUrl = FileTokenData.CreateUrl(fileToken);
      result.ContentTypeId = ContentTypes.File;
      result.DisplayName = cc.DisplayFileName!;
      return result;
    }

    private static ClipboardGetData CreateBase(ClipboardContentEntity cc)
    {
      return new ClipboardGetData()
      {
        Id = cc.Id,
        LaneId = cc.LaneId,
        IsPinned = cc.IsPinned
      };
    }

    public static ClipboardGetData CreateWithImageContent(ClipboardContentEntity cc, FileAccessTokenEntity fileToken)
    {
      var result = CreateBase(cc);
      result.FileContentUrl = FileTokenData.CreateUrl(fileToken);
      result.ContentTypeId = ContentTypes.Image;
      result.DisplayName = cc.DisplayFileName!;
      return result;
    }

    public static ClipboardGetData CreateWithPlainTextContent(ClipboardContentEntity cc)
    {
      var result = CreateBase(cc);
      result.ContentTypeId = ContentTypes.PlainText;
      result.TextContent = cc.TextContent!;
      return result;
    }

    internal static ClipboardGetData CreateFromEntity(ClipboardContentEntity cc, FileAccessTokenEntity? fileToken)
    {
      var contentType = cc.ContentType?.Id ?? cc.ContentTypeId;

      if (contentType == ContentTypes.Image)
      {
        return CreateWithImageContent(cc, fileToken!);
      }
      else if (contentType == ContentTypes.PlainText)
      {
        return CreateWithPlainTextContent(cc);
      }
      else if (contentType == ContentTypes.File)
      {
        return CreateWithFileContent(cc, fileToken!);
      }

      throw new Exception("Unexpected Content Type");
    }

    #endregion Methods
  }
}