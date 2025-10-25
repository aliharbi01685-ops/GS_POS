using Microsoft.Maui.Hosting;
using Microsoft.Maui;

namespace MAUI_InventoryApp_Arabic_Dark
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>();
            return builder.Build();
        }
    }
}
