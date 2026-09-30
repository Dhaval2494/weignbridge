using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.SharedComponents;

namespace WeighingSolution.ViewModel
{
    class VMTransactionReceiptPopup : NotifyPropertyChanged, IDisposable
    {
        public readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        public DataRepository dataRepository;
        public VMTransactionReceiptPopup()
        {
            dataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
        }

        #region Properties Declaration

        private ObservableCollection<CameraImage> _lstCamImage;

        public ObservableCollection<CameraImage> lstCamImage
        {
            get { return _lstCamImage; }
            set { _lstCamImage = value; OnPropertyChanged(); }
        }
        #endregion
        public void ViewTranscationImage(TransactionDataBindingModel ObjTD)
        {
            lstCamImage = new ObservableCollection<CameraImage>();
            string TranDataStatus = ObjTD.Status;
            if (ObjTD.TransactionDetails != null && ObjTD.TransactionDetails.Count > 0)
            {
                foreach (TransactionDetailBindingModel item in ObjTD.TransactionDetails.Where(x => (x.GrossWeight != null && x.GrossWeight != 0 && x.TareWeight != null && x.TareWeight != 0) || (ObjTD.Status == TranDataStatus)))
                {
                    List<CameraImage> lstCamera = dataRepository.Context.CameraImages.Where(x => x.TransactionDetailID == item.TransactionDetailID).ToList();
                    if (lstCamera != null && lstCamera.Count > 0)
                    {
                        for (int i = 0; i < lstCamera.Count; i++)
                        {
                            CameraImage ImageItem = lstCamera[i];
                            ImageItem.TicketID = ObjTD.TicketID;
                            lstCamImage.Add(ImageItem);
                        }
                        OnPropertyChanged("lstCamImage");
                    }
                }
            }
        }

        public void Dispose()
        {
            dataRepository.Context.Dispose();
        }
    }


}
