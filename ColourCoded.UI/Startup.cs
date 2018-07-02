using ColourCoded.UI.Shared.WebApiCaller;
using ColourCoded.UI.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using Rotativa.AspNetCore;

namespace ColourCoded.UI
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc(options =>
      {
        options.Filters.Add(new GlobalExceptionFilter());
        options.Filters.Add(new RequireHttpsAttribute());
      });

      services.AddAuthentication(Configuration["CookieSecurityScheme"])
        .AddCookie(Configuration["CookieSecurityScheme"], options =>
        {
          options.AccessDeniedPath = new PathString("/security/access");
          options.LoginPath = new PathString("/security/authenticate");
          options.ExpireTimeSpan = new TimeSpan(0, Convert.ToInt32(Configuration["CookieExpireTimeSpan"]), 0);
        });

      services.AddTransient<IWebApiCaller, WebApiCaller>();
      services.AddTransient<ICookieHelper, CookieHelper>();
      services.AddTransient<IUserAgentHelper, UserAgentHelper>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      var cultureInfo = new CultureInfo("en-US");
      cultureInfo.NumberFormat.CurrencySymbol = "R";

      CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
      CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

      app.UseAuthentication();

      if (env.IsDevelopment())
      {
        app.UseBrowserLink();
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Home/Error");
      }

      app.UseStaticFiles();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
           name: "default",
           template: "{area}/{controller}/{action=Index}/{id?}");
      });

      RotativaConfiguration.Setup(env);
      app.UseStaticFiles();
    }
  }
}
