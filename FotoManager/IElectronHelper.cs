using System.Threading.Tasks;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace FotoManager
{
    public interface IElectronHelper
    {
        Task<string[]> ShowOpenDialogAsync(BrowserWindow browserWindow, OpenDialogOptions openDialogOptions);

        Task<string> ShowSaveDialogAsync(BrowserWindow browserWindow, SaveDialogOptions saveDialogOptions);

        BrowserWindow GetBrowserWindow();

        void ReloadBrowserWindow();

        void SetProgressBar(double value);
    }
}