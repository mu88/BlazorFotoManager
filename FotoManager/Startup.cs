using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FotoManagerLogic.API;
using FotoManagerLogic.Business;
using FotoManagerLogic.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FotoManager
{
    [ExcludeFromCodeCoverage]
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
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton<IProjectService, ProjectService>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IFileHandler, JsonFileHandler>();
            services.AddSingleton<IServerImageRepository, ServerImageRepository>();
            services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
            services.AddSingleton<IElectronHelper, ElectronHelper>();
            services.AddSingleton<ITranslator, Translator>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseRequestLocalization(new RequestLocalizationOptions
                                       {
                                           DefaultRequestCulture = new RequestCulture("de"),
                                           SupportedCultures = new[] { new CultureInfo("de") },
                                           SupportedUICultures = new[] { new CultureInfo("de") }
                                       });

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
            });

            Task.Run(async () =>
            {
                var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
                var browserWindowOptions = new BrowserWindowOptions { Icon = Path.Combine(directoryName, @"assets\Icon.ico") };
                var browserWindow = await Electron.WindowManager.CreateWindowAsync(browserWindowOptions);
                browserWindow.SetMenuBarVisibility(false);
                browserWindow.Maximize();

#if DEBUG
                browserWindow.WebContents.OpenDevTools();
#endif

                return browserWindow;
            });
        }
    }
}