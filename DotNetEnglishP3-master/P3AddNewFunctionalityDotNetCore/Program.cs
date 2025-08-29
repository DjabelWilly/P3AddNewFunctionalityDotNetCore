using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using P3AddNewFunctionalityDotNetCore;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.ModelBinders;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddLocalization(opts => { opts.ResourcesPath = "Resources"; });
            builder.Services.AddSingleton<ICart, Cart>();
            builder.Services.AddSingleton<ILanguageService, LanguageService>();
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddTransient<IProductRepository, ProductRepository>();
            builder.Services.AddTransient<IOrderService, OrderService>();
            builder.Services.AddTransient<IOrderRepository, OrderRepository>();
            builder.Services.AddMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix, opts => { opts.ResourcesPath = "Resources"; })
                .AddDataAnnotationsLocalization();

            builder.Services.AddDbContext<P3Referential>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("P3Referential")));

            builder.Services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("P3Identity")));

            builder.Services.AddDefaultIdentity<IdentityUser>()
                    .AddEntityFrameworkStores<AppIdentityDbContext>()
                    .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
            });

            // -------------------
            // Définition de la culture par défaut du thread
            // -------------------
            var defaultCulture = new CultureInfo("fr-FR"); // "fr-FR" pour la virgule décimale
            CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
            CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;
            // -------------------

            builder.Services.AddControllersWithViews(options =>
            {
                // Ajoute le provider du Modelbinder custom en priorité (index 0)
                options.ModelBinderProviders.Insert(0, new InvariantDoubleModelBinderProvider());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            else
            {
                app.SeedDatabase(app.Configuration);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            var supportedCultures = new[] { "en-GB", "en-US", "en", "fr-FR", "fr", "es-ES", "es" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("fr-FR")
                .AddSupportedCultures(supportedCultures.ToArray())
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Product}/{action=Index}/{id?}");

            await IdentitySeedData.EnsurePopulated(app);

            app.Run();
        }
    }
}
