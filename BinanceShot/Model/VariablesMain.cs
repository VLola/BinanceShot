using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BinanceShot.Model
{
    public class VariablesMain : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        private decimal _Percent { get; set; } = 0.5m;
        public decimal Percent
        {
            get { return _Percent; }
            set
            {
                if (value != 0m)
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

        private decimal _USDT { get; set; }
        public decimal USDT
        {
            get { return _USDT; }
            set
            {
                if (value != 0m)
                {
                    _USDT = value;
                    OnPropertyChanged("USDT");
                }
            }
        }
        private string _Symbol { get; set; }
        public string Symbol
        {
            get { return _Symbol; }
            set
            {
                _Symbol = value;
                OnPropertyChanged("Symbol");
            }
        }
    }
}
