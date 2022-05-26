using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AdvancedClipboard.Web.Util
{
  public class CustomSwaggerFilter : IDocumentFilter
  {
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
      var nonMobileRoutes = swaggerDoc.Paths
          .Where(x => !x.Key.StartsWith("/api"))
          .ToList();
      nonMobileRoutes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
    }
  }
}
