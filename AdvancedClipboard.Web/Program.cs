using AdvancedClipboard.Server.Repositories;
using AdvancedClipboard.Web.ApiControllers;
using AdvancedClipboard.Web.Models;
using AdvancedClipboard.Web.Models.Identity;
using AdvancedClipboard.Web.Repositories;
using AdvancedClipboard.Web.Util;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
var storageConnectionString = builder.Configuration.GetConnectionString("AzureStorage_ConnectionString");
builder.Services.AddAzureClients(builder => builder.AddBlobServiceClient(storageConnectionString));
builder.Services.AddScoped<FileRepository>();
builder.Services.AddScoped<ClipboardRepository>();
builder.Services.AddScoped<LaneRepository>();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
var clientId = builder.Configuration.GetValue<string>("ClientId");
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "Clipboard API"
  });

  options.AddSecurityDefinition("msid", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
  {
    Type = Microsoft.OpenApi.Models.SecuritySchemeType.OAuth2,
    Flows = new Microsoft.OpenApi.Models.OpenApiOAuthFlows
    {
      AuthorizationCode = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
      {
        AuthorizationUrl = new System.Uri("https://localhost:7071/Identity/Account/Login?ReturnUrl=%2Fswagger"),
        TokenUrl = new System.Uri("https://localhost:7071"),
        Scopes = new Dictionary<string, string>
                {
                    { $"api://{clientId}/access", "access" }
                }
      }
    }
  });

  options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "msid" }
            },
            new [] { $"api://{clientId}/access" }
        }
    });

  //options.DocumentFilter<CustomSwaggerFilter>();

});
builder.Services.AddRazorPages();
builder.Services.AddIdentity<ApplicationUser, Role>()
  .AddEntityFrameworkStores<ApplicationDbContext>()
  .AddDefaultTokenProviders()
  .AddDefaultUI();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}
else
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(name: "api",
    pattern: "api/{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
