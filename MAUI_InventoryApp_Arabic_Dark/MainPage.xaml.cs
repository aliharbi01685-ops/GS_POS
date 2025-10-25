using System;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace MAUI_InventoryApp_Arabic_Dark
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<dynamic> Items { get; set; } = new ObservableCollection<dynamic>();
        string configPath;

        public MainPage()
        {
            InitializeComponent();
            ResultsCollection.ItemsSource = Items;
            configPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.json");
        }

        private async void BtnInventory_Clicked(object sender, EventArgs e)
        {
            var to = DateTime.Now;
            var from = to.AddDays(-7);
            await LoadInventory(from, to);
        }

        private async void BtnSales_Clicked(object sender, EventArgs e)
        {
            string stor = await DisplayPromptAsync("اسم الفرع", "ادخل اسم الفرع:");
            if (!string.IsNullOrEmpty(stor))
            {
                await LoadSales(stor);
            }
        }

        private async void BtnOpen_Clicked(object sender, EventArgs e)
        {
            await LoadOpenAccounts();
        }

        private async Task<(string server,string database,string user,string password)> ReadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    var s = File.ReadAllText(configPath);
                    var doc = JsonDocument.Parse(s);
                    var server = doc.RootElement.GetProperty("server").GetString();
                    var database = doc.RootElement.GetProperty("database").GetString();
                    var user = doc.RootElement.GetProperty("user").GetString();
                    var password = doc.RootElement.GetProperty("password").GetString();
                    return (server,database,user,password);
                }
            }
            catch { }
            return (null,null,null,null);
        }

        private async Task LoadInventory(DateTime f, DateTime t)
        {
            Items.Clear();
            var cfg = await ReadConfig();
            if (string.IsNullOrEmpty(cfg.server)) { await DisplayAlert("خطأ","بيانات الاتصال غير محفوظة","حسناً"); return; }
            var url = $"http://{cfg.server}/api/GetInventory?fromDate={f:yyyy-MM-dd}&toDate={t:yyyy-MM-dd}&server={Uri.EscapeDataString(cfg.server)}&database={Uri.EscapeDataString(cfg.database)}&uid={Uri.EscapeDataString(cfg.user)}&pwd={Uri.EscapeDataString(cfg.password)}";
            try
            {
                using var c = new System.Net.Http.HttpClient();
                c.Timeout = TimeSpan.FromSeconds(12);
                var s = await c.GetStringAsync(url);
                var list = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(s);
                foreach (var it in list)
                {
                    var stor = it.GetProperty("Stor_name").GetString();
                    var user = it.GetProperty("User_Name").GetString();
                    var cash = it.GetProperty("TOTALS_Cash").GetDecimal();
                    var credit = it.GetProperty("TOTALS_Credit").GetDecimal();
                    var date = it.GetProperty("EDATE").ToString();
                    Items.Add(new { Title = $"{stor} - {user}", Subtitle = $"كاش: {cash}، ائتمان: {credit}، التاريخ: {date}" });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("خطأ","فشل جلب البيانات: "+ex.Message,"حسناً");
            }
        }

        private async Task LoadSales(string stor)
        {
            Items.Clear();
            var cfg = await ReadConfig();
            if (string.IsNullOrEmpty(cfg.server)) { await DisplayAlert("خطأ","بيانات الاتصال غير محفوظة","حسناً"); return; }
            var url = $"http://{cfg.server}/api/GetSalesByBranch?stor_name={Uri.EscapeDataString(stor)}&server={Uri.EscapeDataString(cfg.server)}&database={Uri.EscapeDataString(cfg.database)}&uid={Uri.EscapeDataString(cfg.user)}&pwd={Uri.EscapeDataString(cfg.password)}";
            try
            {
                using var c = new System.Net.Http.HttpClient();
                c.Timeout = TimeSpan.FromSeconds(12);
                var s = await c.GetStringAsync(url);
                var list = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(s);
                foreach (var it in list)
                {
                    var storname = it.GetProperty("Stor_name").GetString();
                    var total = it.GetProperty("Total").GetDecimal();
                    var date = it.GetProperty("Odate").ToString();
                    Items.Add(new { Title = $"{storname}", Subtitle = $"الإجمالي: {total}، التاريخ: {date}" });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("خطأ","فشل جلب البيانات: "+ex.Message,"حسناً");
            }
        }

        private async Task LoadOpenAccounts()
        {
            Items.Clear();
            var cfg = await ReadConfig();
            if (string.IsNullOrEmpty(cfg.server)) { await DisplayAlert("خطأ","بيانات الاتصال غير محفوظة","حسناً"); return; }
            var url = $"http://{cfg.server}/api/GetOpenAccounts?server={Uri.EscapeDataString(cfg.server)}&database={Uri.EscapeDataString(cfg.database)}&uid={Uri.EscapeDataString(cfg.user)}&pwd={Uri.EscapeDataString(cfg.password)}";
            try
            {
                using var c = new System.Net.Http.HttpClient();
                c.Timeout = TimeSpan.FromSeconds(12);
                var s = await c.GetStringAsync(url);
                var list = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement[]>(s);
                foreach (var it in list)
                {
                    var stor = it.GetProperty("Stor_name").GetString();
                    var user = it.GetProperty("User_Name").GetString();
                    var cash = it.GetProperty("TOTALS_Cash").GetDecimal();
                    var deposit = it.GetProperty("TOTALS_Deposit").GetDecimal();
                    var withdraw = it.GetProperty("TOTALS_Withdraw").GetDecimal();
                    var status = it.GetProperty("Status").GetString();
                    Items.Add(new { Title = $"{stor} - {user}", Subtitle = $"كاش: {cash}، وديعة: {deposit}، سحب: {withdraw}، الحالة: {status}" });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("خطأ","فشل جلب البيانات: "+ex.Message,"حسناً");
            }
        }
    }
}
