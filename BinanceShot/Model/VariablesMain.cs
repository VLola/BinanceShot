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
        private bool _AutoPlay { get; set; }
        public bool AutoPlay
        {
            get { return _AutoPlay; }
            set
            {
                _AutoPlay = value;
                OnPropertyChanged("AutoPlay");
            }
        }
    }
}
