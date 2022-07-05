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
        private bool _BuyerIsMaker { get; set; } = false;
        public bool BuyerIsMaker
        {
            get { return _BuyerIsMaker; }
            set
            {
                _BuyerIsMaker = value;
                OnPropertyChanged("BuyerIsMaker");
            }
        }
        private bool _Select { get; set; } = false;
        public bool Select
        {
            get { return _Select; }
            set
            {
                _Select = value;
                OnPropertyChanged("Select");
            }
        }
        public List<double> price_buy_x = new List<double>();
        public List<double> price_buy_y = new List<double>();
        public List<double> price_sell_x = new List<double>();
        public List<double> price_sell_y = new List<double>();
        public List<double> price_open_long_order_x = new List<double>();
        public List<double> price_open_long_order_y = new List<double>();
        public List<double> price_open_short_order_x = new List<double>();
        public List<double> price_open_short_order_y = new List<double>();
        public List<double> price_close_order_x = new List<double>();
        public List<double> price_close_order_y = new List<double>();
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
                    // Open
                    isBet = true;
                    isShort = true;
                    CountShort = _CountShort + 1;
                    price_open_short_order_x.Add(_UpdateTime.ToOADate());
                    price_open_short_order_y.Add(Double.Parse(value.ToString()));
                }
                else if (!isBet && value <= _PriceActivateLong && _PriceActivateLong > 0m)
                {
                    // Open
                    isBet = true;
                    isLong = true;
                    CountLong = _CountLong + 1;
                    price_open_long_order_x.Add(_UpdateTime.ToOADate());
                    price_open_long_order_y.Add(Double.Parse(value.ToString()));
                }
                else if (isBet && isLong && value >= PriceTakeProfitLong)
                {
                    Positive = _Positive + 1;
                    price_close_order_x.Add(_UpdateTime.ToOADate());
                    price_close_order_y.Add(Double.Parse(value.ToString()));
                    // Close
                    isLong = false;
                    isBet = false;
                }
                else if (isBet && isLong && value <= PriceStopLossLong)
                {
                    price_close_order_x.Add(_UpdateTime.ToOADate());
                    price_close_order_y.Add(Double.Parse(value.ToString()));
                    // Close
                    isLong = false;
                    isBet = false;
                }
                else if (isBet && isShort && value <= PriceTakeProfitShort)
                {
                    Positive = _Positive + 1;
                    price_close_order_x.Add(_UpdateTime.ToOADate());
                    price_close_order_y.Add(Double.Parse(value.ToString()));
                    // Close
                    isShort = false;
                    isBet = false;
                }
                else if (isBet && isShort && value >= PriceStopLossShort)
                {
                    price_close_order_x.Add(_UpdateTime.ToOADate());
                    price_close_order_y.Add(Double.Parse(value.ToString()));
                    // Close
                    isShort = false;
                    isBet = false;
                }
                // Chart
                
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
        private bool _isBet { get; set; }
        public bool isBet
        {
            get { return _isBet; }
            set
            {
                _isBet = value;
                OnPropertyChanged("isBet");
            }
        }
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
        private decimal _Volume { get; set; }
        public decimal Volume
        {
            get { return _Volume; }
            set
            {
                if (_Price > 0m)
                {
                    _Volume = Math.Round(value * _Price, 0);
                    OnPropertyChanged("Volume");
                }
            }
        }
        private decimal _Quantity { get; set; }
        public decimal Quantity
        {
            get { return _Quantity; }
            set
            {
                if (RoundQuantity(value) > _MinQuantity) {
                    _Quantity = RoundQuantity(value);
                    OnPropertyChanged("Quantity");
                }
            }
        }
        private decimal _USDT { get; set; } = 5.1m;
        public decimal USDT
        {
            get { return _USDT; }
            set
            {
                if (value >= _MIN_USDT) {
                    _USDT = value;
                    OnPropertyChanged("USDT");
                    Quantity = RoundQuantity(value / _Price);
                }
            }
        }
        private decimal _MIN_USDT { get; set; } = 5.1m;
        public decimal MIN_USDT
        {
            get { return _MIN_USDT; }
            set
            {
                if(value >= 5.1m) {
                    _MIN_USDT = value;
                    OnPropertyChanged("MIN_USDT");
                    USDT = value;
                }
            }
        }
        private decimal _StepSize { get; set; }
        public decimal StepSize
        {
            get { return _StepSize; }
            set
            {
                _StepSize = value;
                OnPropertyChanged("StepSize");
            }
        }
        private decimal _MinQuantity { get; set; }
        public decimal MinQuantity
        {
            get { return _MinQuantity; }
            set
            {
                _MinQuantity = value;
                OnPropertyChanged("MinQuantity");
                Quantity = value + _StepSize;
                MIN_USDT = _Quantity * _Price;
            }
        }
        private decimal RoundQuantity(decimal quantity)
        {
            decimal quantity_final = 0m;
            if (_StepSize == 0.001m) quantity_final = Math.Round(quantity, 3);
            else if (_StepSize == 0.01m) quantity_final = Math.Round(quantity, 2);
            else if (_StepSize == 0.1m) quantity_final = Math.Round(quantity, 1);
            else if (_StepSize == 1m) quantity_final = Math.Round(quantity, 0);
            return quantity_final;
        }
    }
}
