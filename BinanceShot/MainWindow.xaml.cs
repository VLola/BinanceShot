using Binance.Net.Enums;
using Binance.Net.Objects.Models.Futures;
using Binance.Net.Objects.Models.Spot;
using BinanceShot.Binance;
using BinanceShot.ConnectDB;
using BinanceShot.Errors;
using BinanceShot.Model;
using BinanceShot.ViewModel;
using Newtonsoft.Json;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Color = System.Drawing.Color;

namespace BinanceShot
{
    public partial class MainWindow : Window
    {
        public VariablesMain Variables { get; set; } = new VariablesMain();
        public string API_KEY { get; set; } = "";
        public string SECRET_KEY { get; set; } = "";
        public string CLIENT_NAME { get; set; } = "";
        public List<SymbolName> list_sumbols_name = new List<SymbolName>();
        public Socket socket;
        public ScatterPlot prices_plot;
        public class SymbolName
        {
            public decimal Price { get; set; }
            public BinanceFuturesUsdtSymbol BFUS { get; set; }
            public SymbolName(decimal Price, BinanceFuturesUsdtSymbol BFUS)
            {
                this.Price = Price;
                this.BFUS = BFUS;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            ErrorWatcher();
            Clients();
            Chart();
            this.DataContext = this;
        }

        private void USDT_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Variables.USDT > 5.1m)
            {
                foreach (SymbolControl it in Symbols.Children)
                {
                    it.symbol.USDT = Variables.USDT;
                }
            }
        }
        private void Percent_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Variables.Percent > 0m)
            {
                foreach (SymbolControl it in Symbols.Children)
                {
                    it.symbol.Percent = Variables.Percent;
                }
            }
        }
        private void PercentTakeProfit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Variables.PercentTakeProfit > 0m)
            {
                foreach (SymbolControl it in Symbols.Children)
                {
                    it.symbol.PercentTakeProfit = Variables.PercentTakeProfit;
                }
            }
        }
        private void PercentStopLoss_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Variables.PercentStopLoss > 0m)
            {
                foreach (SymbolControl it in Symbols.Children)
                {
                    it.symbol.PercentStopLoss = Variables.PercentStopLoss;
                }
            }
        }
        private void DetailSymbol_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string name = (string)button.Content;
            foreach (SymbolControl it in Symbols.Children)
            {
                it.symbol.Select = false;
            }
            foreach (SymbolControl it in Symbols.Children)
            {
                if (it.symbol.SymbolName == name)
                {
                    it.symbol.Select = true;
                }
            }
        }
        private void SellectAll_Click(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            if (box.IsChecked == true)
            {
                foreach (SymbolControl it in Symbols.Children)
                {
                    if (!it.symbol.Start) it.symbol.Start = true;
                }
            }
            else
            {
                foreach (SymbolControl it in Symbols.Children)
                {
                    if (it.symbol.Start) it.symbol.Start = false;
                }
            }
        }
        #region - List Sumbols -
        private void GetSumbolName()
        {
            List<BinancePrice> list = ListPriceSymbols();
            foreach (var it in ListSymbols().OrderBy(q => q.Name).ToList())
            {
                foreach (var item in list) if (item.Symbol == it.Name) list_sumbols_name.Add(new SymbolName(item.Price, it));
            }

            int i = 0;
            foreach (var it in list_sumbols_name)
            {
                Symbols.RowDefinitions.Add(new RowDefinition());
                SymbolControl control = new SymbolControl(it.BFUS.Name, it.BFUS.LotSizeFilter.StepSize, it.BFUS.LotSizeFilter.MinQuantity, it.Price, plt);
                control.DetailSymbol.Click += DetailSymbol_Click;
                Grid.SetRow(control, i);
                Symbols.Children.Add(control);
                i++;
            }
        }

        public List<BinanceFuturesUsdtSymbol> ListSymbols()
        {
            try
            {
                var result = socket.futures.ExchangeData.GetExchangeInfoAsync().Result;
                if (!result.Success) ErrorText.Add("Error ListSymbols");
                return result.Data.Symbols.ToList();
            }
            catch (Exception e)
            {
                ErrorText.Add($"ListSymbols {e.Message}");
                return ListSymbols();
            }
        }
        public List<BinancePrice> ListPriceSymbols()
        {
            try
            {
                var result = socket.futures.ExchangeData.GetPricesAsync().Result;
                if (!result.Success) ErrorText.Add("Error ListPriceSymbols GetPricesAsync");
                return result.Data.ToList();
            }
            catch (Exception e)
            {
                ErrorText.Add($"ListPriceSymbols {e.Message}");
                return ListPriceSymbols();
            }
        }
        #endregion

        #region - Login -
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CLIENT_NAME != "" && API_KEY != "" && SECRET_KEY != "")
                {
                    if (ConnectTrial.Check(CLIENT_NAME))
                    {
                        string path = System.IO.Path.Combine(Environment.CurrentDirectory, "clients");
                        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                        if (!File.Exists(path + "/" + CLIENT_NAME))
                        {

                            Client client = new Client(CLIENT_NAME, API_KEY, SECRET_KEY);
                            string json = JsonConvert.SerializeObject(client);
                            File.WriteAllText(path + "/" + CLIENT_NAME, json);
                            Clients();
                            CLIENT_NAME = "";
                            API_KEY = "";
                            SECRET_KEY = "";
                        }
                    }
                    else ErrorText.Add("Сlient name not found!");
                }
            }
            catch (Exception c)
            {
                ErrorText.Add($"Button_Save {c.Message}");
            }
        }
        private void Clients()
        {
            try
            {
                string path = System.IO.Path.Combine(Environment.CurrentDirectory, "clients");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                List<string> filesDir = (from a in Directory.GetFiles(path) select System.IO.Path.GetFileNameWithoutExtension(a)).ToList();
                if (filesDir.Count > 0)
                {
                    ClientList file_list = new ClientList(filesDir);
                    BOX_NAME.ItemsSource = file_list.BoxNameContent;
                    BOX_NAME.SelectedItem = file_list.BoxNameContent[0];
                }
            }
            catch (Exception e)
            {
                ErrorText.Add($"Clients {e.Message}");
            }
        }
        private void Button_Login(object sender, RoutedEventArgs e)
        {
            try
            {
                if (API_KEY != "" && SECRET_KEY != "" && CLIENT_NAME != "")
                {
                    if (ConnectTrial.Check(CLIENT_NAME))
                    {
                        socket = new Socket(API_KEY, SECRET_KEY);
                        Login_Click();
                        CLIENT_NAME = "";
                        API_KEY = "";
                        SECRET_KEY = "";
                    }
                    else ErrorText.Add("Сlient name not found!");
                }
                else if (BOX_NAME.Text != "")
                {
                    string path = System.IO.Path.Combine(Environment.CurrentDirectory, "clients");
                    string json = File.ReadAllText(path + "\\" + BOX_NAME.Text);
                    Client client = JsonConvert.DeserializeObject<Client>(json);
                    if (ConnectTrial.Check(client.ClientName))
                    {
                        socket = new Socket(client.ApiKey, client.SecretKey);
                        Login_Click();
                    }
                    else ErrorText.Add("Сlient name not found!");
                }
            }
            catch (Exception c)
            {
                ErrorText.Add($"Button_Login {c.Message}");
            }
        }
        private void Login_Click()
        {
            LOGIN_GRID.Visibility = Visibility.Hidden;
            EXIT_GRID.Visibility = Visibility.Visible;
            GetSumbolName();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            EXIT_GRID.Visibility = Visibility.Hidden;
            LOGIN_GRID.Visibility = Visibility.Visible;
            socket = null;
            list_sumbols_name.Clear();
        }
        #endregion

        #region - Error -
        // ------------------------------------------------------- Start Error Text Block --------------------------------------
        private void ErrorWatcher()
        {
            try
            {
                FileSystemWatcher error_watcher = new FileSystemWatcher();
                error_watcher.Path = ErrorText.Directory();
                error_watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                error_watcher.Changed += new FileSystemEventHandler(OnChanged);
                error_watcher.Filter = ErrorText.Patch();
                error_watcher.EnableRaisingEvents = true;
            }
            catch (Exception e)
            {
                ErrorText.Add($"ErrorWatcher {e.Message}");
            }
        }
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            Dispatcher.Invoke(new Action(() => { ERROR_LOG.Text = File.ReadAllText(ErrorText.FullPatch()); }));
        }
        private void Button_ClearErrors(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(ErrorText.FullPatch(), "");
        }
        // ------------------------------------------------------- End Error Text Block ----------------------------------------
        #endregion

        #region - Chart -
        private void Chart()
        {
            plt.Plot.Layout(padding: 12);
            plt.Plot.Style(figureBackground: Color.Black, dataBackground: Color.Black);
            plt.Plot.Frameless();
            plt.Plot.XAxis.TickLabelStyle(color: Color.White);
            plt.Plot.XAxis.TickMarkColor(ColorTranslator.FromHtml("#333333"));
            plt.Plot.XAxis.MajorGrid(color: ColorTranslator.FromHtml("#333333"));

            plt.Plot.YAxis.Ticks(false);
            plt.Plot.YAxis.Grid(false);
            plt.Plot.YAxis2.Ticks(true);
            plt.Plot.YAxis2.Grid(true);
            plt.Plot.YAxis2.TickLabelStyle(color: ColorTranslator.FromHtml("#00FF00"));
            plt.Plot.YAxis2.TickMarkColor(ColorTranslator.FromHtml("#333333"));
            plt.Plot.YAxis2.MajorGrid(color: ColorTranslator.FromHtml("#333333"));

            var legend = plt.Plot.Legend();
            legend.FillColor = Color.Transparent;
            legend.OutlineColor = Color.Transparent;
            legend.Font.Color = Color.White;
            legend.Font.Bold = true;
        }
        #endregion

    }
}
