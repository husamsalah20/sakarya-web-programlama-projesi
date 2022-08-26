using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using InventoryBeginners.Models;
using InventoryBeginners.Data;
using InventoryBeginners.Interfaces;
using InventoryBeginners.Repositories;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace InventoryBeginners
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            //-------------------------------------------
            services.AddSingleton<LanguageService>();

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {

                        var assemblyName = new AssemblyName(typeof(ShareResource).GetTypeInfo().Assembly.FullName);

                        return factory.Create("ShareResource", assemblyName.Name);

                    };

                });



            services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportedCultures = new List<CultureInfo>
                        {
                            new CultureInfo("en-US"),
                            new CultureInfo("ar-SA"),
                            new CultureInfo("tr-TR"),
                        };



                    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");

                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());

                });
            //-------------------------------------------

            services.AddScoped<IUnit, UnitRepository>();
            
            services.AddScoped<IProduct, ProductRepo>();

            services.AddScoped<ISupplier, SupplierRepo>();

            services.AddScoped<ICategory, CategoryRepo>();
            services.AddScoped<IBrand, BrandRepo>();
            services.AddScoped<IProductProfile, ProductProfileRepo>();
            services.AddScoped<IProductGroup, ProductGroupRepo>();
            //services.AddScoped<IProductAttribute, ProductAttributeRepo>();

            services.AddDbContext<InventoryContext>(options => options.UseSqlServer(Configuration.GetConnectionString("dbconn")));

            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<InventoryContext>();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
               
                app.UseHsts();
            }

          

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();

            app.UseRequestLocalization(locOptions.Value);

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();

            });
        }
    }
}
