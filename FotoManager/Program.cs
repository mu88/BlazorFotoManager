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
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FotoManager;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
        builder.Services.AddSingleton<IProjectService, ProjectService>();
        builder.Services.AddSingleton<IFileSystem, FileSystem>();
        builder.Services.AddSingleton<IFileHandler, JsonFileHandler>();
        builder.Services.AddSingleton<IServerImageRepository, ServerImageRepository>();
        builder.Services.AddSingleton<IHttpClientFactory, HttpClientFactory>();
        builder.Services.AddSingleton<IElectronHelper, ElectronHelper>();
        builder.Services.AddSingleton<ITranslator, Translator>();

        builder.UseElectron(args, ElectronAppReady);

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture("de"), SupportedCultures = [new CultureInfo("de")], SupportedUICultures = [new CultureInfo("de")]
        });

        app.UseStaticFiles();

        app.UseRouting();

#pragma warning disable ASP0014
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            endpoints.MapControllers();
        });
#pragma warning restore ASP0014

        app.MapRazorPages();

        app.Run();
    }

    private static async Task ElectronAppReady()
    {
        var directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var browserWindowOptions = new BrowserWindowOptions { Icon = Path.Combine(directoryName, "assets/Icon.ico") };
        var browserWindow = await Electron.WindowManager.CreateWindowAsync(browserWindowOptions);
        browserWindow.SetMenuBarVisibility(false);
        browserWindow.Maximize();

#if DEBUG
        browserWindow.WebContents.OpenDevTools();
#endif

        browserWindow.OnReadyToShow += () => browserWindow.Show();
    }
}