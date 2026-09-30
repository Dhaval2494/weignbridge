using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace InstallerUI.ViewModel
{
    public class ViewModelBase : NotifyPropertyChanged
    {
        private readonly BootstrapperApplication _bootstrapper;
        public BootstrapperApplication Bootstrapper
        {
            get { return _bootstrapper; }
        }
        private readonly MainViewModel _mainViewModel;
        public MainViewModel MainViewModel
        {
            get { return _mainViewModel; }
        }

        public ViewModelBase(MainViewModel mainviewmodel)
        {
            _mainViewModel = mainviewmodel;
            _bootstrapper = mainviewmodel.Bootstrapper;
        }
        public string ReadFormattedVariableFromBundle(string variableName)
        {
            string result = string.Empty;
            try
            {
                string text = string.Format("Getting variable '{0}' from the bundle.", variableName);
                this._bootstrapper.Engine.Log(LogLevel.Verbose, text);
                string _value = this._bootstrapper.Engine.StringVariables[variableName];
                if (!string.IsNullOrEmpty(_value))
                {
                    _value = Bootstrapper.Engine.FormatString(_value);
                    _value = _value.Trim('"');
                }
                return _value;
            }
            catch (System.Exception ex)
            {
                string text2 = string.Format("The variable '{0}' does not exist in the bundle.\n{1}", variableName, ex.Message);
                this._bootstrapper.Engine.Log(LogLevel.Error, text2);
            }
            return result;
        }
        public string ReadVariableFromBundle(string variableName)
        {
            string result = string.Empty;
            try
            {
                string text = string.Format("Getting variable '{0}' from the bundle.", variableName);
                this._bootstrapper.Engine.Log(LogLevel.Verbose, text);
                return this._bootstrapper.Engine.StringVariables[variableName];
            }
            catch (System.Exception ex)
            {
                string text2 = string.Format("The variable '{0}' does not exist in the bundle.\n{1}", variableName, ex.Message);
                this._bootstrapper.Engine.Log(LogLevel.Error, text2);
            }
            return result;
        }
        public void WriteVariableToBundle(string variableName, string variableValue)
        {
            try
            {
                string text = string.Format("Writing variable '{0}' with the value '{1}' to the bundle.", variableName, variableValue);
                this._bootstrapper.Engine.Log(LogLevel.Verbose, text);
                this._bootstrapper.Engine.StringVariables[variableName] = variableValue;
            }
            catch (System.Exception ex)
            {
                string text2 = string.Format("Error writing variable '{0}' with the value {1} to bundle.\n{2}", variableName, variableValue, ex.Message);
                this._bootstrapper.Engine.Log(LogLevel.Error, text2);
            }
        }
    }
}