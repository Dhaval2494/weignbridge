using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WeighingSolution.DataLayer;
using WeighingSolution.Events;
using WeighingSolution.ViewModel;

namespace WeighingSolution.View
{
    /// <summary>
    /// Interaction logic for WeighBridgeManagement_Page.xaml
    /// </summary>
    public partial class WeighBridgeManagement_Page : Page
    {
        public int IntWeighBridgeID = 0;
        public bool HasSingleWB = false;
        vmWeightBridgeManagement viewModel;
        private void SetupPage()
        {
            this.Unloaded += CurrentPage_Unloaded;
            this.Loaded += CurrentPage_Loaded;
            viewModel = new vmWeightBridgeManagement(IntWeighBridgeID);
            this.DataContext = viewModel;

            if (Common.ValidatePageRequest(PageEntity.WeighBridge, PageAccess.Update))
                WeightBridgeManagementSave.Visibility = Visibility.Visible;

            gbPLCPort.Visibility = Constant.hasUnmannedPlugin ? Visibility.Visible : Visibility.Collapsed;
            
        }
        private void GoBack(object obj)
        {
            if (HasSingleWB)
            {
                Uri uri = new Uri("/View/DatasetupView/DataSetup_Page.Xaml", UriKind.Relative);
                this.NavigationService.Navigate(uri);
            }
            else
            {
                Uri uri = new Uri("/View/DatasetupView/WeighBridgeManagementList_Page.Xaml", UriKind.Relative);
                this.NavigationService.Navigate(uri);
            }
        }
        private async void CurrentPage_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadAsync();

            if (string.IsNullOrEmpty(viewModel.Model.Region))
            {
                viewModel.Model.Region = "india";
            }

            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Subscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Subscribe(GoBack);
        }

        void CurrentPage_Unloaded(object sender, RoutedEventArgs e)
        {
            MainWindow.weighCommand = false;
            viewModel.EventAggregator.GetEvent<AfterDetailSaveCancelledEvent>().Unsubscribe(GoBack);
            viewModel.EventAggregator.GetEvent<AfterDetailSavedEvent>().Unsubscribe(GoBack);
            viewModel.Model.ReleaseReaders();
            viewModel.Dispose();
        }
        public WeighBridgeManagement_Page()
        {
            InitializeComponent();
            HasSingleWB = true;
            using (weighbridgeEntities ctx = new weighbridgeEntities(Constant.EntityConnectionString))
            {
                WeightBridge objWB = ctx.WeightBridges.FirstOrDefault(x => x.IsActive);
                if (objWB != null)
                    IntWeighBridgeID = objWB.WeightBridgeID;
            }
            SetupPage();
        }
        public WeighBridgeManagement_Page(int weighbridgeid)
        {
            InitializeComponent();
            IntWeighBridgeID = weighbridgeid;
            SetupPage();
        }

        private void weightstart_Click(object sender, RoutedEventArgs e)
        {
            if (viewModel.FiveDigitWeight.ToString().Length == 5 && !string.IsNullOrEmpty(viewModel.Model.RawData))
            {
                string firstrawstring = AutoSplitFrames(viewModel.Model.RawData);

                string finalWeight = DetectAndSavePattern(firstrawstring, viewModel.FiveDigitWeight.ToString());
                viewModel.WeightShow = finalWeight;
            }
        }


        private string leftover = "";

        public string AutoSplitFrames(string rawInput)
        {
            string buffer = leftover + rawInput;
            leftover = "";

            if (buffer.Length < 2)
            {
                leftover = buffer;
                return string.Empty;
            }

            // STEP 1: find the repeating prefix (1 to 5 characters)
            string prefix = null;

            for (int len = 5; len >= 1; len--)
            {
                if (buffer.Length <= len) continue;

                string test = buffer.Substring(0, len);
                int second = buffer.IndexOf(test, len, StringComparison.Ordinal);

                if (second > 0)
                {
                    prefix = test;
                    break;
                }
            }

            // If no prefix found → store and wait for more data
            if (prefix == null)
            {
                leftover = buffer;
                return string.Empty;
            }

            // STEP 2: split by prefix occurrences
            List<string> frames = new List<string>();
            int index = 0;

            while (true)
            {
                int next = buffer.IndexOf(prefix, index + prefix.Length, StringComparison.Ordinal);

                if (next == -1)
                {
                    // last incomplete frame → keep as leftover
                    leftover = buffer.Substring(index);
                    break;
                }

                // complete frame found
                string frame = buffer.Substring(index, next - index);
                frames.Add(frame);

                index = next;
            }

            return frames[0];
        }

        public string DetectAndSavePattern(string rawFrame, string knownWeight)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rawFrame) || string.IsNullOrWhiteSpace(knownWeight))
                    return null;

                // Use the original rawFrame for counting/indexing so STX/ETX remain present

                string original = rawFrame.Replace("\r", "").Replace("\n", "");

                // Also create a digits-only version for fallback searching
                string digitsOnly = new string(original.Where(char.IsDigit).ToArray());

                // Try to locate the known weight in the full original string first
                int weightIndex = original.IndexOf(knownWeight, StringComparison.Ordinal);

                if (weightIndex == -1)
                {
                    // if not found, try digits-only
                    int digitsIndex = digitsOnly.IndexOf(knownWeight, StringComparison.Ordinal);
                    if (digitsIndex == -1)
                        return null; // cannot detect pattern

                    int digitCount = 0;
                    int mappedIndex = -1;
                    for (int i = 0; i < original.Length; i++)
                    {
                        if (char.IsDigit(original[i]))
                        {
                            if (digitCount == digitsIndex)
                            {
                                mappedIndex = i;
                                break;
                            }
                            digitCount++;
                        }
                    }

                    if (mappedIndex == -1)
                        return null; // mapping failed for some reason
                    weightIndex = mappedIndex;
                }

                // Determine first character and second occurrence index (if any).
                // Use original so control chars are counted.
                if (original.Length == 0)
                    return null;

                // Extract exact weight from original using mapped index
                if (weightIndex + knownWeight.Length > original.Length)
                    return null; // defensive check

                string extracted = original.Substring(weightIndex, knownWeight.Length);

                // Save into viewModel (preserve original string and counts)
                viewModel.Model.TotalStringLength = (short?)original.Length;         // full cleaned length or first part length as per your original logic
                viewModel.Model.StartStringChar = original.First().ToString();
                viewModel.Model.StringCharLength = original.Length.ToString();
                viewModel.Model.Format = "######.##";
                viewModel.Model.StringStartingValue = string.Empty;              // entire raw string (preserve control chars)
                viewModel.Model.IsDecimalInString = knownWeight.Contains(".");
                viewModel.Model.DecimalPointLocation = (short?)(knownWeight.Contains(".")
                    ? knownWeight.Split('.')[1].Length
                    : 0);
                viewModel.Model.IsReverseString = false;
                viewModel.Model.WEIGHTSTARTFROM = weightIndex.ToString();   // detected start index in the original string
                viewModel.Model.WEIGHTLENGTH = knownWeight.Length.ToString();

                CommandManager.InvalidateRequerySuggested();

                return extracted;
            }
            catch (Exception ex)
            {
                // consider logging ex somewhere instead of swallowing silently
                return string.Empty;
            }
        }
    }
}