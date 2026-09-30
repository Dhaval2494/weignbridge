using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WeighingSolution.DataLayer
{
    public class NotifyDataErrorInfoBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Dictionary<string, List<string>> _errorsByPropertyName
         = new Dictionary<string, List<string>>();

        public bool HasErrors { get { return _errorsByPropertyName.Any(); } }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;

            return _errorsByPropertyName.ContainsKey(propertyName)
              ? _errorsByPropertyName[propertyName]
              : null;
        }

        protected virtual void OnErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged("HasErrors");
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName[propertyName] = new List<string>();
            }
            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        protected void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
        protected virtual void SetValue<TValue>(TValue value,
          [CallerMemberName] string propertyName = null)
        {
            this.GetType().GetProperty(propertyName).SetValue(this, value);
            OnPropertyChanged(propertyName);
            ValidatePropertyInternal(propertyName, value);
        }
        public bool IsValid()
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (!property.PropertyType.Name.Contains("ObservableCollection"))
                {
                    ValidatePropertyInternal(property.Name, property.GetValue(this));
                    ValidateBusinessLogic(property.Name);
                }
            }
            return !HasErrors;
        }

        public void ValidatePropertyInternal(string propertyName, object currentValue)
        {
            ClearErrors(propertyName);

            ValidateDataAnnotations(propertyName, currentValue);

            ValidateCustomErrors(propertyName);
        }

        private void ValidateDataAnnotations(string propertyName, object currentValue)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this) { MemberName = propertyName };
            Validator.TryValidateProperty(currentValue, context, results);

            foreach (var result in results)
            {
                AddError(propertyName, result.ErrorMessage);
            }
        }

        private void ValidateCustomErrors(string propertyName)
        {
            var errors = ValidateProperty(propertyName);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    AddError(propertyName, error);
                }
            }
        }

        private void ValidateBusinessLogic(string propertyName)
        {
            var errors = VerifyBusinessLogic(propertyName);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    AddError(propertyName, error);
                }
            }
        }

        protected virtual IEnumerable<string> ValidateProperty(string propertyName)
        {
            return null;
        }

        protected virtual IEnumerable<string> VerifyBusinessLogic(string propertyName)
        {
            return null;
        }
    }
}
