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
            if (e.PropertyName == "Price")
            {

                if (symbol.Select == true)
                {
                    if (symbol.price_buy_x.Count > 0)
                    {
                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.Remove(long_price);
                            long_price = plt.Plot.AddScatter(symbol.price_buy_x.ToArray(), symbol.price_buy_y.ToArray(), color: Color.Green, lineWidth: 0, markerSize: 5);
                            long_price.YAxisIndex = 1;
                        });
                    }
                    if (symbol.price_sell_x.Count > 0)
                    {
                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.Remove(short_price);
                            short_price = plt.Plot.AddScatter(symbol.price_sell_x.ToArray(), symbol.price_sell_y.ToArray(), color: Color.Red, lineWidth: 0, markerSize: 5);
                            short_price.YAxisIndex = 1;
                        });
                    }
                    if (symbol.price_open_long_order_x.Count > 0)
                    {
                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.Remove(long_open_order);
                            long_open_order = plt.Plot.AddScatter(symbol.price_open_long_order_x.ToArray(), symbol.price_open_long_order_y.ToArray(), color: Color.Green, lineWidth: 0, markerSize: 8);
                            long_open_order.YAxisIndex = 1;
                        });
                    }
                    if (symbol.price_open_short_order_x.Count > 0)
                    {
                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.Remove(short_open_order);
                            short_open_order = plt.Plot.AddScatter(symbol.price_open_short_order_x.ToArray(), symbol.price_open_short_order_y.ToArray(), color: Color.Red, lineWidth: 0, markerSize: 8);
                            short_open_order.YAxisIndex = 1;
                        });
                    }
                    if (symbol.price_close_order_x.Count > 0)
                    {

                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.Remove(close_order);
                            close_order = plt.Plot.AddScatter(symbol.price_close_order_x.ToArray(), symbol.price_close_order_y.ToArray(), color: Color.Red, lineWidth: 0, markerSize: 10, markerShape: ScottPlot.MarkerShape.eks);
                            close_order.YAxisIndex = 1;
                        });
                    }
                    if (symbol.AutoPlay)
                    {
                        plt.Dispatcher.Invoke(() =>
                        {
                            plt.Plot.SetAxisLimits(xMin: symbol.UpdateTime.AddMinutes(-1).ToOADate(), xMax: symbol.UpdateTime.AddSeconds(10).ToOADate(), yAxisIndex: 1);
                            //plt.Plot.AxisAuto();
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
