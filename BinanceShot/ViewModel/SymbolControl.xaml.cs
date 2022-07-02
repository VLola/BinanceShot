using BinanceShot.Binance;
using BinanceShot.Model;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BinanceShot.ViewModel
{
    /// <summary>
    /// Логика взаимодействия для SymbolControl.xaml
    /// </summary>
    public partial class SymbolControl : UserControl
    {

        public DispatcherTimer timer = new DispatcherTimer();
        public Symbol symbol { get; set; } 
        public Socket socket { get; set; } = new Socket("Si5U4TSmpX4ByMDQEiWu9aGnHaX7o66Hw1erDl5tsfOKw1sjXTpUrP0JhonXrGJR", "ddKGxVke1y7Y0WRMBeuMeKAfqNdU7aBC8eOeHXHMY6CqYGzl0MPfuM60UkX7Dnoa");
        public SymbolControl(string symbol_name, ScottPlot.WpfPlot plt)
        {
            InitializeComponent();
            this.DataContext = this;
            symbol = new Symbol(plt);
            symbol.SymbolName = symbol_name;
            symbol.PropertyChanged += Symbol_PropertyChanged;
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            symbol.PastPrice = symbol.Price;
        }

        private void Symbol_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Start")
            {
                if (symbol.Start)
                {
                    SubscribeToAggregatedTradeUpdatesAsync();
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    StopAsync();
                }
            }
        }

        private async void StopAsync()
        {
            await Task.Run(() => {
                Stop();
            });
        }
        private void Stop()
        {
            socket.socketClient.UnsubscribeAllAsync();
        }
        public async void SubscribeToAggregatedTradeUpdatesAsync()
        {
            await socket.futuresSocket.SubscribeToAggregatedTradeUpdatesAsync(symbol.SymbolName, (Message => {
                Dispatcher.Invoke(new Action(() =>
                {
                    symbol.UpdateTime = Message.Data.TradeTime;
                    symbol.BuyerIsMaker = Message.Data.BuyerIsMaker;
                    symbol.Price = Message.Data.Price;
                }));
            }));
        }
    }
}
