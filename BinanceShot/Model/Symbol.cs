using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BinanceShot.Model
{
    public class Symbol : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public List<HistoryTrade> Prices = new List<HistoryTrade>();
        public List<HistoryTrade> TradesLong = new List<HistoryTrade>();
        public List<HistoryTrade> TradesShort = new List<HistoryTrade>();
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
        private decimal _Price { get; set; }
        public decimal Price
        {
            get { return _Price; }
            set
            {
                _Price = value;
                OnPropertyChanged("Price");
                Prices.Add(new HistoryTrade(value, _UpdateTime));
                if (!isBet && value >= _PriceActivateShort && _PriceActivateShort > 0m)
                {
                    CountShort = _CountShort + 1;
                    isBet = true;
                    isShort = true;
                    TradesShort.Add(new HistoryTrade(value, _UpdateTime, true));
                }
                else if (!isBet && value <= _PriceActivateLong && _PriceActivateLong > 0m)
                {
                    CountLong = _CountLong + 1;
                    isBet = true;
                    isLong = true;
                    TradesLong.Add(new HistoryTrade(value, _UpdateTime, true));
                }
                else if (isBet && isLong && value >= PriceTakeProfitLong)
                {
                    isLong = false;
                    isBet = false;
                    Positive = _Positive + 1;
                    TradesLong.Add(new HistoryTrade(value, _UpdateTime));
                }
                else if (isBet && isLong && value <= PriceStopLossLong)
                {
                    isLong = false;
                    isBet = false;
                    TradesLong.Add(new HistoryTrade(value, _UpdateTime));
                }
                else if (isBet && isShort && value <= PriceTakeProfitShort)
                {
                    isShort = false;
                    isBet = false;
                    Positive = _Positive + 1;
                    TradesShort.Add(new HistoryTrade(value, _UpdateTime));
                }
                else if (isBet && isShort && value >= PriceStopLossShort)
                {
                    isShort = false;
                    isBet = false;
                    TradesShort.Add(new HistoryTrade(value, _UpdateTime));
                }
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
                PriceActivateLong = (value - (value * _Percent));
                PriceStopLossLong = (_PriceActivateLong - (PriceActivateLong * _PercentStopLoss));
                PriceTakeProfitLong = (_PriceActivateLong + (PriceActivateLong * _PercentTakeProfit));
                PriceActivateShort = (value + (value * _Percent));
                PriceStopLossShort = (_PriceActivateShort + (_PriceActivateShort * _PercentStopLoss));
                PriceTakeProfitShort = (_PriceActivateShort - (_PriceActivateShort * _PercentTakeProfit));
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
        private decimal _Percent { get; set; } = 0.001m;
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
        private decimal _PercentStopLoss { get; set; } = 0.001m;
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
        private decimal _PercentTakeProfit { get; set; } = 0.001m;
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
    public class HistoryTrade
    {
        public decimal Price { get; set; }
        public DateTime UpdateTime { get; set; }
        public bool isBuy { get; set; }
        public HistoryTrade(decimal Price, DateTime UpdateTime, bool? isBuy = false)
        {
            this.Price = Price;
            this.UpdateTime = UpdateTime;
            this.isBuy = (bool)isBuy;
        }
    }
}
