using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace FotoManager;

[ExcludeFromCodeCoverage]
public class ElectronHelper : IElectronHelper
{
    /// <inheritdoc />
    public async Task<string[]> ShowOpenDialogAsync(BrowserWindow browserWindow, OpenDialogOptions openDialogOptions)
        => await Electron.Dialog.ShowOpenDialogAsync(browserWindow, openDialogOptions);

    /// <inheritdoc />
    public async Task<string> ShowSaveDialogAsync(BrowserWindow browserWindow, SaveDialogOptions saveDialogOptions)
        => await Electron.Dialog.ShowSaveDialogAsync(browserWindow, saveDialogOptions);

    /// <inheritdoc />
    public BrowserWindow GetBrowserWindow() => Electron.WindowManager.BrowserWindows.First();

    /// <inheritdoc />
    public void ReloadBrowserWindow()
    {
        GetBrowserWindow().Reload();
    }

    /// <inheritdoc />
    public void SetProgressBar(double value)
    {
        GetBrowserWindow().SetProgressBar(value);
    }
}
