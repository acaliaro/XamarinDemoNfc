using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DemoNfc.ViewModel
{
    public class ResultPopupViewModel : INotifyPropertyChanged
    {

        public ObservableCollection<string> Tags { get; set; }
        public int Seconds { get; set; } = 8;
        public ResultPopupViewModel(System.Collections.Generic.List<string> arg)
        {
            Tags = new ObservableCollection<string>(arg);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
