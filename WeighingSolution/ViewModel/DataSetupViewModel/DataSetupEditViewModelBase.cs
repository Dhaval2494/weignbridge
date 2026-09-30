using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    public abstract class DataSetupEditViewModelBase<T> : NotifyPropertyChanged, IDisposable
        where T : NotifyDataErrorInfoBase
    {
        public DataRepository dataRepository;
        public readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        public DataSetupEditViewModelBase()
        {
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
            SaveFalg = false;
        }

        public bool SaveFalg { get; set; }

        #region Command and Properties Declaration
        public ICommand SaveCommand { get; private set; }
        public ICommand CloseDetailViewCommand { get; set; }

        //Used to reload entity in case of DB Concurrency issue
        private int _id;
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }
        //Used to track changes to the Model
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
                    ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
                }
            }
        }

        private T _model;
        public T Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Abstract Methods (required to be implemented by child classes)
        public abstract Task LoadAsync();
        protected abstract void AfterSaveExecute();
        #endregion

        #region Methods
        protected virtual bool OnSaveCanExecute()
        {
            return Model != null && !Model.HasErrors && HasChanges;
        }
        protected virtual void RaiseDetailSavedEvent(int ObjectID, Object ObjectInstance)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs
            {
                ObjectID = ObjectID,
                ObjectInstance = ObjectInstance,
                ObjectType = ObjectInstance.GetType().Name
            });
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
            EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Publish(new AfterDetailSaveCancelledEventArgs { ObjectInstance = Model });
        }
        protected virtual bool BeforeSaveExecute()
        {
            return true;
        }
        protected async virtual void OnSaveExecute()
        {
            if (SaveFalg)
                return;

            if (Model.IsValid())
            {
                try
                {
                    if (BeforeSaveExecute())
                    {
                        SaveFalg = true;
                        await dataRepository.SaveAsync();
                        AfterSaveExecute();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var databaseValues = ex.Entries.Single().GetDatabaseValues();
                    if (databaseValues == null)
                    {
                        MessageBoxHelper.ShowInfoDialogAsync("Save Failed!", "The entity has been deleted by another user").Wait();
                        return;
                    }

                    var result = MessageBoxHelper.ShowOkCancelDialogAsync("Save Failed!", "The entity has been changed in "
                     + "the meantime by someone else. Click OK to save your changes anyway, click Cancel "
                     + "to reload the entity from the database.").Result;

                    if (result == MessageDialogResult.Affirmative)
                    {
                        // Update the original values with database-values
                        var entry = ex.Entries.Single();
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                        dataRepository.SaveAsync().Wait();
                        AfterSaveExecute();
                    }
                    else
                    {
                        // Reload entity from database
                        ex.Entries.Single().ReloadAsync().Wait();
                        LoadAsync().Wait();
                    }
                };
            }
        }
        protected void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = dataRepository.HasChanges();
            }
            if (e.PropertyName == "HasErrors")
            {
                ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
        public void InitializeModel(T model)
        {
            Model = model;
            Model.PropertyChanged -= Model_PropertyChanged;
            Model.PropertyChanged += Model_PropertyChanged;
            ((DelegateCommand)SaveCommand).RaiseCanExecuteChanged();
        }
        public T CreateNewModel()
        {
            Type type = typeof(T);
            var model = (T)Activator.CreateInstance(type);
            dataRepository.Context.Set<T>().Add(model);
            return model;
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
        #endregion
    }
}
