using AdvancedClipboard.Server.Repositories;
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
builder.Services.AddScoped<MimeTypeResolver>();
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
var clientId = builder.Configuration.GetValue<string>("ClientId");

builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new OpenApiInfo
  {
    Version = "v1",
    Title = "Clipboard API",
  });

  options.AddServer(new OpenApiServer() { Description = "Azure Instance", Url = "https://advancedclipboard2.azurewebsites.net/" });
  options.AddServer(new OpenApiServer() { Description = "Local Instance", Url = "https://localhost:7071/" });

  options.AddSecurityDefinition("cookieAuth", new OpenApiSecurityScheme
  {
    Type = SecuritySchemeType.ApiKey,
    In = ParameterLocation.Cookie,
    Name = ".AspNetCore.Identity.Application"
  });

  options.DocumentFilter<CustomSwaggerFilter>();

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
