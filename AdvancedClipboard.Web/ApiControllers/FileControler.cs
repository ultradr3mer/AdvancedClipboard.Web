using AdvancedClipboard.Server.Repositories;
using AdvancedClipboard.Web.Util;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;

namespace AdvancedClipboard.Web.ApiControllers
{
  /// <summary>
  /// The <see cref="FileController"/> provides functionality to upload and download files.
  /// </summary>
  [Route("api/[controller]")]
  [Authorize]
  [ApiController]
  public class FileController : ControllerBase
  {
    private readonly FileRepository fileRepository;
    private readonly MimeTypeResolver mimeTypeResolver;


    #region Constructors

    public FileController(FileRepository fileRepository, MimeTypeResolver mimeTypeResolver)
    {
      this.fileRepository = fileRepository;
      this.mimeTypeResolver = mimeTypeResolver;
    }

    #endregion Constructors

    #region Methods

    public bool ThumbnailCallback()
    {
      return false;
    }

    /// <summary>
    /// Gets an image thumbnail.
    /// </summary>
    /// <param name="token">The access token of the image to get.</param>
    /// <param name="filename">The filename of the image to get.</param>
    /// <returns>The detailed post information.</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("thumb/{token}/{filename}")]
    public async Task<IActionResult> GetThumbnail(string token, string filename)
    {
      if (!long.TryParse(token, NumberStyles.HexNumber, null, out long actualToken))
      {
        throw new ArgumentException("Invalid Token.", nameof(token));
      }

      string contentType = "image/jpeg";

      (BlobContainerClient container, BlobContainerClient thumbsContainer) = await this.fileRepository.GetAzureContainerPair(actualToken, filename);


      var jpegFilename = Path.GetFileNameWithoutExtension(filename) + ".jpg";
      BlobClient thumbClient = thumbsContainer.GetBlobClient(jpegFilename);
      bool exists = await thumbClient.ExistsAsync();
      if (exists)
      {
        Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadInfo> info = await thumbClient.DownloadAsync();
        return this.File(info.Value.Content, contentType);
      }
      else
      {
        BlobClient blob = container.GetBlobClient(filename);
        bool existsOriginal = await thumbClient.ExistsAsync();
        if (existsOriginal)
        {
          return this.NotFound();
        }

        using var stream = await blob.OpenReadAsync();
        // Example in C#, should be quite alike in ASP.NET
        // Assuming filename as the uploaded file
#pragma warning disable CA1416 // Validate platform compatibility
        using (Image bigImage = new Bitmap(stream))
        {
          float targetWidth = Math.Min((float)bigImage.Width, 270f);
          float targetHeight = bigImage.Height * targetWidth / bigImage.Width;

          if (targetHeight > 1000)
          {
            targetHeight = 1000;
            targetWidth = bigImage.Width * targetHeight / targetWidth;
          }

          // Now create a thumbnail
          using (Image smallImage = bigImage.GetThumbnailImage((int)targetWidth, 
                                                               (int)targetHeight,
                                                               new Image.GetThumbnailImageAbort(ThumbnailCallback), IntPtr.Zero))
          {
            using var streamSave = new MemoryStream();
            smallImage.Save(streamSave, ImageFormat.Jpeg);
            streamSave.Position = 0;
            streamSave.Seek(0, SeekOrigin.Begin);
            await thumbClient.UploadAsync(streamSave);
          }
#pragma warning restore CA1416 // Validate platform compatibility

          Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadInfo> info = await thumbClient.DownloadAsync();
          return this.File(info.Value.Content, contentType);
        }
      }
    }

    /// <summary>
    /// Gets an image.
    /// </summary>
    /// <param name="token">The access token of the image to get.</param>
    /// <param name="filename">The filename of the image to get.</param>
    /// <returns>The detailed post information.</returns>
    [AllowAnonymous]
    [HttpGet]
    [Route("{token}/{filename}")]
    public async Task<IActionResult> Get(string token, string filename)
    {
      if (!long.TryParse(token, NumberStyles.HexNumber, null, out long actualToken))
      {
        throw new ArgumentException("Invalid Token.", nameof(token));
      }

      string contentType = this.mimeTypeResolver.GetMimeType(Path.GetExtension(filename));

      BlobContainerClient azureContainer = await this.fileRepository.GetAzureContainer(actualToken, filename);
      BlobClient blob = azureContainer.GetBlobClient(filename);
      Azure.Response<Azure.Storage.Blobs.Models.BlobDownloadInfo> info = await blob.DownloadAsync();
      return this.File(info.Value.Content, contentType);
    }

    #endregion Methods
  }
}