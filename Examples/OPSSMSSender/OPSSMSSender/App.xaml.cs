using System;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OPSSMSSender.Model;
using OPSSMSSender.Model.Config;
using OPSSMSSender.ViewModel;
using OzCommon.Model;
using OzCommon.Model.Mock;
using OzCommon.Utils;
using OzCommon.Utils.DialogService;
using OzCommon.Utils.Schedule;
using OzCommon.View;
using OzCommon.ViewModel;
using OzCommonBroadcasts.Model;
using OzCommonBroadcasts.Model.Csv;
using OzCommonBroadcasts.View;
using OzCommonBroadcasts.ViewModel;

namespace OPSSMSSender
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        readonly SingletonApp singletonApp;

        public App()
        {
            singletonApp = new SingletonApp("OPSSMSSender");
            InitDependencies();
        }

        void InitDependencies()
        {
            SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
            SimpleIoc.Default.Register<IBroadcastMainViewModel>(() => new SmsViewModel());
            SimpleIoc.Default.Register<ICsvImporter<SmsEntry>>(() => new CsvImporter<SmsEntry>());
            SimpleIoc.Default.Register<ICsvExporter<SmsEntry>>(() => new CsvExporter<SmsEntry>());
            SimpleIoc.Default.Register<IGenericSettingsRepository<SmsConfig>>(() => new GenericSettingsRepository<SmsConfig>());
            SimpleIoc.Default.Register<IWorkerFactory<SmsEntry>>(() => new SmsWorkerFactory());
            SimpleIoc.Default.Register<IExtensionContainer>(() => new ExtensionContainer());
            SimpleIoc.Default.Register<IScheduler<SmsEntry>>(() => new Scheduler<SmsEntry>(SimpleIoc.Default.GetInstance<IWorkerFactory<SmsEntry>>()));
            SimpleIoc.Default.Register(() => new ApplicationInformation("SMS Sender"));
            SimpleIoc.Default.Register<IUserInfoSettingsRepository>(() => new UserInfoSettingsRepository());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);

            if (e.Args.Length != 0 && e.Args[0].ToLower() == "-mock")
                SimpleIoc.Default.Register<IClient>(() => new MockClient());
            else
                SimpleIoc.Default.Register<IClient>(() => new Client());

            singletonApp.OnStartup(e);

            var loginWindow = new LoginWindow();
            loginWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, MessageReceived);
            base.OnExit(e);
        }

        private void MessageReceived(NotificationMessage notificationMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (notificationMessage.Notification == Messages.NavigateToMainWindow)
                {
                    var mainWindow = new BroadcastMainWindow();
                    mainWindow.Show();

                    Application.Current.MainWindow = mainWindow;
                }
            }));
        }
    }
}
