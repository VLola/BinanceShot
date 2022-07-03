using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace BinanceShot.Model
{
    public class Symbol : INotifyPropertyChanged
    {
        public ScatterPlot long_price;
        public ScatterPlot short_price;
        public ScatterPlot long_open_order;
        public ScatterPlot short_open_order;
        public ScatterPlot close_order;
        public ScottPlot.WpfPlot plt;
        public Symbol(ScottPlot.WpfPlot plt)
        {
            this.plt = plt;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private string _SymbolName { get; set; }
        public string SymbolName
        {
            get { return _SymbolName; }
            set
            {
                _SymbolName = value;
                OnPropertyChanged("SymbolName");
            }
        }
        private DateTime _UpdateTime { get; set; }
        public DateTime UpdateTime
        {
            get { return _UpdateTime; }
            set
            {
                _UpdateTime = value;
                OnPropertyChanged("UpdateTime");
            }
        }
        private bool _Start { get; set; }
        public bool Start
        {
            get { return _Start; }
            set
            {
                _Start = value;
                OnPropertyChanged("Start");
            }
        }
        private bool _BuyerIsMaker { get; set; }
        public bool BuyerIsMaker
        {
            get { return _BuyerIsMaker; }
            set
            {
                _BuyerIsMaker = value;
                OnPropertyChanged("BuyerIsMaker");
            }
        }
        private bool _Select { get; set; }
        public bool Select
        {
            get { return _Select; }
            set
            {
                _Select = value;
                OnPropertyChanged("Select");
                if (value) plt.Plot.Clear();
            }
        }
        List<double> price_buy_x = new List<double>();
        List<double> price_buy_y = new List<double>();
        List<double> price_sell_x = new List<double>();
        List<double> price_sell_y = new List<double>();
        List<double> price_open_long_order_x = new List<double>();
        List<double> price_open_long_order_y = new List<double>();
        List<double> price_open_short_order_x = new List<double>();
        List<double> price_open_short_order_y = new List<double>();
        List<double> price_close_order_x = new List<double>();
        List<double> price_close_order_y = new List<double>();
        private decimal _Price { get; set; }
        public decimal Price
        {
            get { return _Price; }
            set
            {
                _Price = value;
                OnPropertyChanged("Price");
                if (!_BuyerIsMaker)
                {
                    price_buy_x.Add(_UpdateTime.ToOADate());
                    price_buy_y.Add(Double.Parse(value.ToString()));
                }
                else
                {
                    price_sell_x.Add(_UpdateTime.ToOADate());
                    price_sell_y.Add(Double.Parse(value.ToString()));
                }
                if (!isBet && value >= _PriceActivateShort && _PriceActivateShort > 0m)
                {
                    CountShort = _CountShort + 1;
                    isBet = true;
                    isShort = true;
                    price_open_short_order_x.Add(_UpdateTime.ToOADate());
                    price_open_short_order_y.Add(Double.Parse(value.ToString()));
                }
                else if (!isBet && value <= _PriceActivateLong && _PriceActivateLong > 0m)
                {
                    CountLong = _CountLong + 1;
                    isBet = true;
                    isLong = true;
                    price_open_long_order_x.Add(_UpdateTime.ToOADate());
                    price_open_long_order_y.Add(Double.Parse(value.ToString()));
                }
                else if (isBet && isLong && value >= PriceTakeProfitLong)
                {
                    isLong = false;
                    isBet = false;
                    Positive = _Positive + 1;
                    price_close_order_x.Add(_UpdateTime.ToOADate());
                    price_close_order_y.Add(Double.Parse(value.ToString()));
                }
                else if (isBet && isLong && value <= PriceStopLossLong)
                {
                    isLong = false;
                    isBet = false;
                    price_close_order_x.Add(_UpdateTime.ToOADate());
                    price_close_order_y.Add(Double.Parse(value.ToString()));
                }
                else if (isBet && isShort && value <= PriceTakeProfitShort)
                {
                    isShort = false;
                    isBet = false;
                    Positive = _Positive + 1;
                    price_close_order_x.Add(_UpdateTime.ToOADate());
                    price_close_order_y.Add(Double.Parse(value.ToString()));
                }
                else if (isBet && isShort && value >= PriceStopLossShort)
                {
                    isShort = false;
                    isBet = false;
                    price_close_order_x.Add(_UpdateTime.ToOADate());
                    price_close_order_y.Add(Double.Parse(value.ToString()));
                }
                // Chart
                
                if (_Select)
                {
                    if (price_buy_x.Count > 0)
                    {
                        plt.Plot.Remove(long_price);
                        long_price = plt.Plot.AddScatter(price_buy_x.ToArray(), price_buy_y.ToArray(), color: Color.LightGreen, lineWidth: 0, markerSize: 5, label: _SymbolName);
                        long_price.YAxisIndex = 1;
                    }
                    if (price_sell_x.Count > 0)
                    {
                        plt.Plot.Remove(short_price);
                        short_price = plt.Plot.AddScatter(price_sell_x.ToArray(), price_sell_y.ToArray(), color: Color.Pink, lineWidth: 0, markerSize: 5);
                        short_price.YAxisIndex = 1;
                    }
                    if (price_open_long_order_x.Count > 0)
                    {
                        plt.Plot.Remove(long_open_order);
                        long_open_order = plt.Plot.AddScatter(price_open_long_order_x.ToArray(), price_open_long_order_y.ToArray(), color: Color.Green, lineWidth: 0, markerSize: 8);
                        long_open_order.YAxisIndex = 1;
                    }
                    if (price_open_short_order_x.Count > 0)
                    {
                        plt.Plot.Remove(short_open_order);
                        short_open_order = plt.Plot.AddScatter(price_open_short_order_x.ToArray(), price_open_short_order_y.ToArray(), color: Color.Red, lineWidth: 0, markerSize: 8);
                        short_open_order.YAxisIndex = 1;
                    }
                    if (price_close_order_x.Count > 0)
                    {
                        plt.Plot.Remove(close_order);
                        close_order = plt.Plot.AddScatter(price_close_order_x.ToArray(), price_close_order_y.ToArray(), color: Color.Red, lineWidth: 0, markerSize: 10, markerShape: ScottPlot.MarkerShape.eks);
                        close_order.YAxisIndex = 1;
                    }
                    if(_AutoPlay) plt.Plot.SetAxisLimits(xMin: _UpdateTime.AddMinutes(-1).ToOADate(), xMax: _UpdateTime.AddSeconds(10).ToOADate(), yAxisIndex: 1);
                    plt.Render();
                }
            }
        }
        public bool _AutoPlay { get; set; } = true;
        public bool AutoPlay
        {
            get { return _AutoPlay; }
            set
            {
                _AutoPlay = value;
                OnPropertyChanged("AutoPlay");
            }
        }
        private bool isBet { get; set; }
        private bool isLong { get; set; }
        private bool isShort { get; set; }
        private decimal _PastPrice { get; set; }
        public decimal PastPrice
        {
            get { return _PastPrice; }
            set
            {
                _PastPrice = value;
                OnPropertyChanged("PastPrice");
                PriceActivateLong = (value - (value * _Percent * 0.01m));
                PriceActivateShort = (value + (value * _Percent * 0.01m));
                if (!isBet)
                {
                    PriceStopLossLong = (_PriceActivateLong - (PriceActivateLong * _PercentStopLoss * 0.01m));
                    PriceTakeProfitLong = (_PriceActivateLong + (PriceActivateLong * _PercentTakeProfit * 0.01m));
                    PriceStopLossShort = (_PriceActivateShort + (_PriceActivateShort * _PercentStopLoss * 0.01m));
                    PriceTakeProfitShort = (_PriceActivateShort - (_PriceActivateShort * _PercentTakeProfit * 0.01m));
                }
            }
        }
        private decimal _PriceActivateLong { get; set; }
        public decimal PriceActivateLong
        {
            get { return _PriceActivateLong; }
            set
            {
                _PriceActivateLong = value;
                OnPropertyChanged("PriceActivateLong");
            }
        }
        private decimal _PriceActivateShort { get; set; }
        public decimal PriceActivateShort
        {
            get { return _PriceActivateShort; }
            set
            {
                _PriceActivateShort = value;
                OnPropertyChanged("PriceActivateShort");
            }
        }
        private decimal _Percent { get; set; } = 0.5m;
        public decimal Percent
        {
            get { return _Percent; }
            set
            {
                if(value != 0m)
                {
                    _Percent = value;
                    OnPropertyChanged("Percent");
                }
            }
        }
        private decimal _PercentStopLoss { get; set; } = 0.45m;
        public decimal PercentStopLoss
        {
            get { return _PercentStopLoss; }
            set
            {
                if (value != 0m)
                {
                    _PercentStopLoss = value;
                    OnPropertyChanged("PercentStopLoss");
                }
            }
        }
        private decimal _PercentTakeProfit { get; set; } = 0.15m;
        public decimal PercentTakeProfit
        {
            get { return _PercentTakeProfit; }
            set
            {
                if (value != 0m)
                {
                    _PercentTakeProfit = value;
                    OnPropertyChanged("PercentTakeProfit");
                }
            }
        }

        private decimal _PriceStopLossLong { get; set; }
        public decimal PriceStopLossLong
        {
            get { return _PriceStopLossLong; }
            set
            {
                _PriceStopLossLong = value;
                OnPropertyChanged("PriceStopLossLong");
            }
        }

        private decimal _PriceTakeProfitLong { get; set; }
        public decimal PriceTakeProfitLong
        {
            get { return _PriceTakeProfitLong; }
            set
            {
                _PriceTakeProfitLong = value;
                OnPropertyChanged("PriceTakeProfitLong");
            }
        }
        private decimal _PriceStopLossShort { get; set; }
        public decimal PriceStopLossShort
        {
            get { return _PriceStopLossShort; }
            set
            {
                _PriceStopLossShort = value;
                OnPropertyChanged("PriceStopLossShort");
            }
        }

        private decimal _PriceTakeProfitShort { get; set; }
        public decimal PriceTakeProfitShort
        {
            get { return _PriceTakeProfitShort; }
            set
            {
                _PriceTakeProfitShort = value;
                OnPropertyChanged("PriceTakeProfitShort");
            }
        }

        private int _CountLong { get; set; } = 0;
        public int CountLong
        {
            get { return _CountLong; }
            set
            {
                _CountLong = value;
                OnPropertyChanged("CountLong");
            }
        }
        private int _CountShort { get; set; } = 0;
        public int CountShort
        {
            get { return _CountShort; }
            set
            {
                _CountShort = value;
                OnPropertyChanged("CountShort");
            }
        }
        private int _Positive { get; set; } = 0;
        public int Positive
        {
            get { return _Positive; }
            set
            {
                _Positive = value;
                OnPropertyChanged("Positive");
            }
        }
    }
}
