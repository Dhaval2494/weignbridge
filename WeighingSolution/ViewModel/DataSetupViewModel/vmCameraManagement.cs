using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;


namespace WeighingSolution.ViewModel
{
    public class vmCameraManagement : NotifyDataErrorInfoBase, IDisposable
    {
        public DataRepository dataRepository;
        public readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;

        public vmCameraManagement(int CameraId)
        {
            Id = CameraId;
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
            SaveCommand = new DelegateCommand(OnSaveExecute, OnSaveCanExecute);
            CloseDetailViewCommand = new DelegateCommand<string>(OnCloseDetailViewExecute);
        }


        #region Command and Properties Declaration
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand<string> CloseDetailViewCommand { get; set; }
        private int _id;
        public int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }
        private Camera _model;
        public Camera Model
        {
            get { return _model; }
            set
            {
                _model = value;
                OnPropertyChanged();
            }
        }
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
        private List<WeightBridge> weightBridge;
        public List<WeightBridge> WeightBridges
        {
            get { return weightBridge; }
            set { weightBridge = value; OnPropertyChanged(); }
        }
        #endregion

        public async Task LoadAsync()
        {
            var camera = Id > 0
              ? await dataRepository.Context.Cameras.SingleAsync(x => x.CameraID == Id)
              : CreateNewCamera();

            WeightBridges = await dataRepository.Context.WeightBridges.AsNoTracking().Where(x => x.IsActive).ToListAsync();
            WeightBridges.Insert(0, new WeightBridge() { WeightBridgeID = 0, ScaleName = "-- Select Scale Name --" });

            Model = camera;
            Model.PropertyChanged -= Model_PropertyChanged;
            Model.PropertyChanged += Model_PropertyChanged;
            SaveCommand.RaiseCanExecuteChanged();
        }
        private Camera CreateNewCamera()
        {
            var camera = new Camera();
            camera.Active = true;
            dataRepository.Context.Cameras.Add(camera);
            return camera;
        }
        protected void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!HasChanges)
            {
                HasChanges = dataRepository.HasChanges();
            }
            if (e.PropertyName == "HasErrors")
            {
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
        private async void OnCloseDetailViewExecute(string obj)
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

        private bool OnSaveCanExecute()
        {
            return Model != null && !Model.HasErrors && HasChanges;
        }

        private async void OnSaveExecute()
        {
            if (Model.IsValid())
            {
                try
                {
                    await dataRepository.SaveAsync();
                    ToastHelper.ShowSuccess("Camera information recorded successfully");
                    RaiseDetailSavedEvent(Model.CameraID, Model);
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
                        ToastHelper.ShowSuccess("Camera information recorded successfully");
                        RaiseDetailSavedEvent(Model.CameraID, Model);
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
        protected virtual void RaiseDetailSavedEvent(int ObjectID, Object ObjectInstance)
        {
            EventAggregator.GetEvent<AfterDetailSavedEvent>().Publish(new AfterDetailSavedEventArgs
            {
                ObjectID = ObjectID,
                ObjectInstance = ObjectInstance,
                ObjectType = ObjectInstance.GetType().Name
            });
        }
        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
    }
}
