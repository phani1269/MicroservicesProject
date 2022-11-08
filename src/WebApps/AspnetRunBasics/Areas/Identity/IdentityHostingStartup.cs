using System;
using AspnetRunBasics.Areas.Identity.Data;
using AspnetRunBasics.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(AspnetRunBasics.Areas.Identity.IdentityHostingStartup))]
namespace AspnetRunBasics.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<AspnetRunBasicsContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("AspnetRunBasicsContextConnection"),
                        provideroptions=>provideroptions.EnableRetryOnFailure()));

                services.AddDefaultIdentity<AspnetRunBasicsUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<AspnetRunBasicsContext>();
            });
        }
    }
}