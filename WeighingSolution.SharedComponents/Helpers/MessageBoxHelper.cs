using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace WeighingSolution.Helpers
{
    public class MessageBoxHelper
    {
        private static MetroWindow MetroWindow { get { return (MetroWindow)Application.Current.MainWindow; } }
        public static async Task<MessageDialogResult> ShowOkCancelDialogAsync(string TitleText, string MessageText)
        {
            var msgbox_settings = new MetroDialogSettings { AnimateShow = false, AnimateHide = false };
            MessageDialogResult msgbox_result = await MetroWindow.ShowMessageAsync(TitleText, MessageText, MessageDialogStyle.AffirmativeAndNegative, msgbox_settings);
            return msgbox_result;
        }
        public static async Task<MessageDialogResult> ShowYesNoDialog(string TitleText, string MessageText, MessageDialogResult DefaultButtonFocus = MessageDialogResult.Affirmative)
        {
            MetroWindow.MetroDialogOptions.ColorScheme = MetroDialogColorScheme.Accented;
            var msgbox_settings = new MetroDialogSettings { AffirmativeButtonText = "Yes", NegativeButtonText = "No", DefaultButtonFocus = DefaultButtonFocus, AnimateShow = false, AnimateHide = false };
            MessageDialogResult msgbox_result = await MetroWindow.ShowMessageAsync(TitleText, MessageText, MessageDialogStyle.AffirmativeAndNegative, msgbox_settings);
            return msgbox_result;
        }
        public static async Task ShowInfoDialogAsync(string TitleText, string MessageText)
        {
            var msgbox_settings = new MetroDialogSettings { AnimateShow = false, AnimateHide = false };
            await MetroWindow.ShowMessageAsync(TitleText, MessageText, MessageDialogStyle.Affirmative, msgbox_settings);
        }
    }
}
