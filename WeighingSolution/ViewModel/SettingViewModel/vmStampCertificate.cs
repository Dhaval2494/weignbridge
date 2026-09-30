using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Windows.Controls;
using WeighingSolution.DataLayer;
using WeighingSolution.Helpers;

namespace WeighingSolution.ViewModel.SettingViewModel
{
    public class vmStampCertificate : NotifyDataErrorInfoBase
    {
        private Page parentpage;

        #region CTOR
        public vmStampCertificate(Page page)
        {
            parentpage = page;

            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);

        }
        public DelegateCommand SaveSettingCommand { get; private set; }
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }

        private bool _hasChanges;
        public bool HasChanges
        {
            get { return _hasChanges; }
            set
            {
                if (_hasChanges != value)
                {
                    _hasChanges = value;
                    OnPropertyChanged();
                    ((DelegateCommand)SaveSettingCommand).RaiseCanExecuteChanged();
                }
            }
        }

        protected async virtual void OnCloseDetailViewExecute(string NavigateURL)
        {
            if (HasChanges)
            {
                var result = await MessageBoxHelper.ShowOkCancelDialogAsync("Are you sure?", "You've made changes. Close this item?");
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
            }
            Uri uri = new Uri(NavigateURL, UriKind.Relative);
            parentpage.NavigationService.Navigate(uri);
        }
        #endregion
    }
}
