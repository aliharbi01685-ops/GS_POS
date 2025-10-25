using Microsoft.Maui.Controls;

namespace MAUI_InventoryApp_Arabic_Dark
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Views.ConnectionPage());
        }
    }
}
