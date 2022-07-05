using Binance.Net.Enums;
using BinanceShot.Binance;
using BinanceShot.Model;
using ScottPlot.Plottable;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace BinanceShot.ViewModel
{
    public partial class SymbolControl : UserControl
    {

        public DispatcherTimer timer = new DispatcherTimer();
        public Symbol symbol { get; set; } = new Symbol();

        public ScatterPlot long_price { get; set; }
        public ScatterPlot short_price { get; set; }
        public ScatterPlot long_open_order { get; set; }
        public ScatterPlot short_open_order { get; set; }
        public ScatterPlot close_order { get; set; }
        public ScottPlot.WpfPlot plt { get; set; }
        public Socket socket { get; set; } = new Socket("Si5U4TSmpX4ByMDQEiWu9aGnHaX7o66Hw1erDl5tsfOKw1sjXTpUrP0JhonXrGJR", "ddKGxVke1y7Y0WRMBeuMeKAfqNdU7aBC8eOeHXHMY6CqYGzl0MPfuM60UkX7Dnoa");
        public SymbolControl(string symbol_name, decimal step_size, decimal min_quantity, decimal price, ScottPlot.WpfPlot plt)
        {
            InitializeComponent();
            this.plt = plt;
            this.DataContext = this;
            symbol.SymbolName = symbol_name;
            symbol.Price = price;
            symbol.StepSize = step_size;
            symbol.MinQuantity = min_quantity;
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
                    SubscribeToTickerUpdatesAsync();
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    StopAsync();
                }
            }
            if (symbol.Select)
            {
                if (e.PropertyName == "PriceOpenLongOrder")
                {
                    plt.Dispatcher.Invoke(() =>
                    {
                        plt.Plot.AddPoint(symbol.TimeOpenLongOrder, symbol.PriceOpenLongOrder, color: Color.Green, size: 10);
                    });
                }
                if (e.PropertyName == "PriceOpenShortOrder")
                {
                    plt.Dispatcher.Invoke(() =>
                    {
                        plt.Plot.AddPoint(symbol.TimeOpenShortOrder, symbol.PriceOpenShortOrder, color: Color.Red, size: 10);
                    });
                }
                if (e.PropertyName == "PriceOpenShortOrder")
                {
                    plt.Dispatcher.Invoke(() =>
                    {
                        plt.Plot.AddPoint(symbol.TimeCloseOrder, symbol.PriceCloseOrder, color: Color.Red, size: 10, shape: ScottPlot.MarkerShape.eks);
                    });
                }
                if (e.PropertyName == "Price")
                {
                    if (symbol.BuyerIsMaker)
                    {
                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.AddPoint(symbol.UpdateTimeDouble, symbol.PriceDouble, color: Color.Green, size: 5);
                        });
                    }
                    else
                    {
                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.AddPoint(symbol.UpdateTimeDouble, symbol.PriceDouble, color: Color.Red, size: 5);
                        });
                    }
                    if (symbol.AutoPlay)
                    {
                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.SetAxisLimits(xMin: symbol.UpdateTime.AddMinutes(-1).ToOADate(), xMax: symbol.UpdateTime.AddSeconds(10).ToOADate());
                        });
                    }
                    plt.Dispatcher.Invoke(() =>
                    {
                        plt.Render();
                    });
                }
            }
            
            if (e.PropertyName == "Select")
            {
                plt.Dispatcher.Invoke(() =>
                {
                    plt.Plot.Clear();
                });
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
                symbol.UpdateTime = Message.Data.TradeTime;
                symbol.BuyerIsMaker = Message.Data.BuyerIsMaker;
                symbol.Price = Message.Data.Price;
            }));
        }
        public async void SubscribeToTickerUpdatesAsync()
        {
            await socket.futuresSocket.SubscribeToTickerUpdatesAsync(symbol.SymbolName, (Message => {
                symbol.Volume = Message.Data.Volume;
            }));
        }
    }
}
