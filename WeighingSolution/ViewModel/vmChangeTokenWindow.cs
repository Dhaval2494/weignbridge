using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.Helpers;

namespace UnmannedPlugin.ViewModel
{
    public class vmChangeTokenWindow : WeighingSolution.SharedComponents.NotifyPropertyChanged, IDisposable
    {
        public readonly IEventAggregator EventAggregator = AppEventManager.Instance.EventAggregator;
        private DataRepository unmanneddataRepository;
        private ChildWindow childWindow;
        public vmChangeTokenWindow(ChildWindow cw)
        {
            unmanneddataRepository = new DataRepository(new weighbridgeEntities(Application.Current.Properties["EntityConnectionString"].ToString()));
            // Tokens = new ObservableCollection<Token>(unmanneddataRepository.Context.Tokens.Where(x => (x.TokenType == "Temporary" && x.TokenStatus == "Free") || (x.TokenType == "Permanent" && x.TokenExpireDate >= DateTime.Now && x.TokenStatus == "Allocated")).ToList());
            SaveChangeTokenCommand = new DelegateCommand(OnSaveChangeTokenExecute);
            EventAggregator.GetEvent<ChangetokenEvent>().Subscribe(onChangeToken);
            childWindow = cw;
        }

        public void onChangeToken(ChangetokenEventArgs obj)
        {
            if (obj != null && obj.objTranTokenLog != null)
            {
                Model = unmanneddataRepository.Context.TransactionTokenLogs.FirstOrDefault(x => x.TransactionTokenLogID == obj.objTranTokenLog.TransactionTokenLogID);
                Tokens = new ObservableCollection<Token>(unmanneddataRepository.Context.Tokens.Where(x => x.TokenID != Model.TokenID && x.TokenStatus == "Free" && (x.TokenExpireDate >= DateTime.Now || x.TokenType == "Temporary")).ToList());
                Model.Token.TokenStatus = "Free";// Default selected status of previous Token
            }
        }
        private void OnSaveChangeTokenExecute()
        {
            try
            {
                //Modify Previous TransactionTokenLog-->Iscancel & Cancelreason & (If Previous Status set free,set freetime)
                //Modify Previous TokenTable-->change status(Free or disable)
                if (BeforesaveExecute())
                {
                    Model.IsCancel = true;
                    if (Model.Token.TokenStatus == "Free")
                        Model.FreeTime = DateTime.Now;
                    unmanneddataRepository.Context.SaveChanges();
                    ToastHelper.ShowSuccess("Token change successfully");
                    childWindow.Close();
                    // add new TransactionTokenLog
                    TransactionTokenLog objTrantoken = new TransactionTokenLog();
                    unmanneddataRepository.Context.TransactionTokenLogs.Add(objTrantoken);
                    objTrantoken.ReceiptTicketID = Model.ReceiptTicketID;
                    objTrantoken.IsCancel = false;
                    objTrantoken.TokenID = Convert.ToInt32(NewToken);
                    objTrantoken.AllocateTime = DateTime.Now;
                    objTrantoken.Token = unmanneddataRepository.Context.Tokens.FirstOrDefault(x => x.TokenID == objTrantoken.TokenID);
                    objTrantoken.Token.TokenStatus = "Allocated";
                    unmanneddataRepository.Context.SaveChanges();
                    EventAggregator.GetEvent<AfterChangeTokenEvent>().Publish(new ChangetokenEventArgs { objTranTokenLog = objTrantoken });
                }
            }
            catch (Exception)
            {
            }
        }

        private bool BeforesaveExecute()
        {
            if (string.IsNullOrEmpty(NewToken))
            {
                ToastHelper.ShowWarning("Please select new token");
                return false;
            }
            else if (string.IsNullOrEmpty(Model.CancelReason))
            {
                ToastHelper.ShowWarning("change reason is required");
                return false;
            }
            else
                return true;
        }


        #region Command and Properties Declaration
        public DelegateCommand SaveChangeTokenCommand { get; private set; }

        private ObservableCollection<Token> _Tokens;
        public ObservableCollection<Token> Tokens
        {
            get { return _Tokens; }
            set { _Tokens = value; OnPropertyChanged(); }
        }


        private TransactionTokenLog _Model;

        public TransactionTokenLog Model
        {
            get { return _Model; }
            set { _Model = value; OnPropertyChanged(); }
        }
        private string _NewToken;

        public string NewToken
        {
            get { return _NewToken; }
            set { _NewToken = value; OnPropertyChanged(); }
        }

        #endregion
        public void Dispose()
        {
            unmanneddataRepository.Context.Dispose();
            EventAggregator.GetEvent<ChangetokenEvent>().Unsubscribe(onChangeToken);
        }
    }
}
