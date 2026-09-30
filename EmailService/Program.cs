using System.ServiceProcess;

namespace EmailService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
#if DEBUG
            //While debugging this section is used.
            EmailScheduler myService = new EmailScheduler();
            myService.onDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

#else
            //In Release this section is used.This is the "normal" way.
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                                                                new EmailScheduler()
            };
            ServiceBase.Run(ServicesToRun);
#endif

            //EmailScheduler emailScheduler = new EmailScheduler();
            //emailScheduler.SendEmail();
        }
    }
}
