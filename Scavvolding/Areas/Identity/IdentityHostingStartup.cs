using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scavvolding.Data;

[assembly: HostingStartup(typeof(Scavvolding.Areas.Identity.IdentityHostingStartup))]
namespace Scavvolding.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<ScavvoldingContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("ScavvoldingContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ScavvoldingContext>();
            });
        }
    }
}