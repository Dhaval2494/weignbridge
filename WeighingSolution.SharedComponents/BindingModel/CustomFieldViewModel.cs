using System.Collections.ObjectModel;
using System.ComponentModel;
namespace WeighingSolution.SharedComponents
{
    public class CustomFieldViewModel : INotifyPropertyChanged
    {
        public int CustomFieldID { get; set; }
        public string DisplayName { get; set; }
        public bool IsRequired { get; set; }
        public bool IsVisible { get; set; }
        public bool Editable { get; set; } = true;   // <-- NEW
        public string ColumnName { get; set; }
        private string _customValue;
        public string CustomValue
        {
            get => _customValue;
            set
            {
                if (_customValue != value)
                {
                    _customValue = value;
                    OnPropertyChanged(nameof(CustomValue));
                }
            }
        }
        public ObservableCollection<string> FieldValues { get; set; } = new ObservableCollection<string>();
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}