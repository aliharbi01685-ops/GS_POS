using System;
using Microsoft.Maui.Controls;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MAUI_InventoryApp_Arabic_Dark.Views
{
    public partial class ConnectionPage : ContentPage
    {
        string configPath;
        public ConnectionPage()
        {
            InitializeComponent();
            configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");
            LoadSaved();
        }

        void LoadSaved()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    var s = File.ReadAllText(configPath);
                    var doc = JsonDocument.Parse(s);
                    EntryServer.Text = doc.RootElement.GetProperty("server").GetString();
                    EntryDatabase.Text = doc.RootElement.GetProperty("database").GetString();
                    EntryUser.Text = doc.RootElement.GetProperty("user").GetString();
                    EntryPassword.Text = doc.RootElement.GetProperty("password").GetString();
                }
            }
            catch { }
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            BtnSave.IsEnabled = false;
            LblStatus.IsVisible = false;
            var cfg = new { server = EntryServer.Text?.Trim(), database = EntryDatabase.Text?.Trim(), user = EntryUser.Text?.Trim(), password = EntryPassword.Text ?? "" };
            // Save locally first
            try
            {
                var json = JsonSerializer.Serialize(cfg);
                File.WriteAllText(configPath, json);
            }
            catch { }

            // Test connection by calling GetOpenAccounts endpoint on provided server
            try
            {
                var apiUrl = $"http://{cfg.server}/api/GetOpenAccounts?server={Uri.EscapeDataString(cfg.server)}&database={Uri.EscapeDataString(cfg.database)}&uid={Uri.EscapeDataString(cfg.user)}&pwd={Uri.EscapeDataString(cfg.password)}";
                using var c = new System.Net.Http.HttpClient();
                c.Timeout = TimeSpan.FromSeconds(8);
                var res = await c.GetAsync(apiUrl);
                if (res.IsSuccessStatusCode)
                {
                    LblStatus.Text = "✅ تم الاتصال بنجاح";
                    LblStatus.IsVisible = true;
                    await Task.Delay(900);
                    await Navigation.PushAsync(new MainPage());
                }
                else
                {
                    LblStatus.Text = "❌ فشل الاتصال، تأكد من العنوان وبيانات الدخول";
                    LblStatus.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                LblStatus.Text = "❌ فشل الاتصال: " + ex.Message;
                LblStatus.IsVisible = true;
            }
            finally
            {
                BtnSave.IsEnabled = true;
            }
        }
    }
}
