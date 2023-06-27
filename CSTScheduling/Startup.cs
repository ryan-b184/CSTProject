using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CSTScheduling.Data.Context;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using CSTScheduling.Data.Services;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Syncfusion.Blazor;
using CSTScheduling.Data;

namespace CSTScheduling
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazorise();
            services.AddSyncfusionBlazor();
            services.AddHttpContextAccessor();
          
            // register factory and configure the options
            services.AddDbContextFactory<CstScheduleDbContext>(opt =>
                opt.UseSqlite(Configuration.GetConnectionString("CstScheduleDbContext")));


            services.AddBlazorise().AddBootstrapProviders().AddFontAwesomeIcons();
            services.AddSingleton<CstScheduleDbService>();
            //services.AddScoped<CstScheduleDbService>();   // https://stackoverflow.com/questions/48202403/instance-of-entity-type-cannot-be-tracked-because-another-instance-with-same-key
            //services.AddTransient<CstScheduleDbService>(); //testing injection alt
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDbContextFactory<CstScheduleDbContext> contextFactory)
        {
            var context = contextFactory.CreateDbContext();
            //context.Database.EnsureDeleted();
            if (env.IsDevelopment())
            {
                //WILL DELETE DATABASE ONCE U ARE DOING USING IT
                //context.Database.EnsureDeleted();


                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            context.Database.EnsureCreated();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}