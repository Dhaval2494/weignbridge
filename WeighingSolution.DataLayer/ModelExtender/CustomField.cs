using System;
using System.Collections.Generic;

namespace WeighingSolution.DataLayer
{
    public partial class CustomField : NotifyDataErrorInfoBase
    {
        private List<String> _fieldValues;
        public List<String> FieldValues
        {
            get { return _fieldValues; }
            set { _fieldValues = value; OnPropertyChanged(); }
        }

        private string _SelectedfieldValues;
        public string SelectedfieldValues
        {
            get { return _SelectedfieldValues; }
            set { _SelectedfieldValues = value; OnPropertyChanged(); }
        }

        private bool _Isselected;

        public bool Isselected
        {
            get { return _Isselected; }
            set { _Isselected = value; OnPropertyChanged(); }
        }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case "DisplayName":
                    if (IsVisible && String.IsNullOrEmpty(DisplayName))
                        yield return "DisplayName is required.";
                    break;
            }
        }
    }
}