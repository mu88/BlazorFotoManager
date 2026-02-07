using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;
using FotoManagerLogic.Business;
using FotoManagerLogic.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
        });
#pragma warning restore ASP0014

        app.MapGet("/api/images", (string path) =>
        {
            var filePath = Encoding.UTF8.GetString(Convert.FromBase64String(path));
            if (!File.Exists(filePath))
            {
                return Results.NotFound();
            }

            var contentType = Path.GetExtension(filePath).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Results.File(stream, contentType);
        });

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