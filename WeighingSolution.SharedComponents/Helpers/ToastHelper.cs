using System;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace WeighingSolution.Helpers
{
    public class ToastHelper
    {
        private static Notifier notifier = new Notifier(cfg =>
        {
            cfg.PositionProvider = new WindowPositionProvider(
                parentWindow: Application.Current.MainWindow,
                corner: Corner.TopRight,
                offsetX: 10,
                offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(3));

            cfg.Dispatcher = Application.Current.Dispatcher;
        });

        public static void ShowInformation(string Msg)
        {
            try
            {
                notifier.ShowInformation(Msg, new ToastNotifications.Core.MessageOptions { ShowCloseButton = false });
            }
            catch { }

        }
        public static void ShowSuccess(string Msg)
        {
            try
            {
                notifier.ShowSuccess(Msg, new ToastNotifications.Core.MessageOptions { ShowCloseButton = false });
            }
            catch { }
        }
        public static void ShowWarning(string Msg)
        {
            try
            {
                notifier.ShowWarning(Msg, new ToastNotifications.Core.MessageOptions { ShowCloseButton = false });
            }
            catch { }
        }
        public static void ShowError(string Msg)
        {
            try
            {
                notifier.ShowError(Msg, new ToastNotifications.Core.MessageOptions { ShowCloseButton = false });
            }
            catch { }
        }

        public static void InitToaster()
        {
            notifier = new Notifier(cfg =>
           {
               cfg.PositionProvider = new WindowPositionProvider(
                   parentWindow: Application.Current.MainWindow,
                   corner: Corner.TopRight,
                   offsetX: 10,
                   offsetY: 10);

               cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                   notificationLifetime: TimeSpan.FromSeconds(3),
                   maximumNotificationCount: MaximumNotificationCount.FromCount(3));

               cfg.Dispatcher = Application.Current.Dispatcher;
           });
        }
    }
}
