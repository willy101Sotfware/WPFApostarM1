using System.ComponentModel;

namespace WPFApostar.Models
{
    public class CheckTypeSerch : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyRaised(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }

        private string _item1;

        public string Item1
        {
            get
            {
                return _item1;
            }
            set
            {
                _item1 = value;
                OnPropertyRaised("Item1");
            }
        }

        private string _item2;

        public string Item2
        {
            get
            {
                return _item2;
            }
            set
            {
                _item2 = value;
                OnPropertyRaised("Item2");
            }
        }
    }
}