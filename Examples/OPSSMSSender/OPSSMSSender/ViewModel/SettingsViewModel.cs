using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OPSSMSSender.Model.Config;
using OzCommon.Model;
using OzCommon.ViewModel;

namespace OPSSMSSender.ViewModel
{
    class SettingsViewModel : ViewModelBase
    {
        readonly IGenericSettingsRepository<SmsConfig> settingsRepository;

        public RelayCommand CancelCommand { get; protected set; }
        public RelayCommand SaveCommand { get; protected set; }

        public SmsConfig SmsConfig { get; set; }

        public SettingsViewModel()
        {
            InitCommands();
            settingsRepository = SimpleIoc.Default.GetInstance<IGenericSettingsRepository<SmsConfig>>();
            GetSettings();
        }

        private void GetSettings()
        {
            SmsConfig = new SmsConfig();

            var config = settingsRepository.GetSettings();
            if (config != null)
                SmsConfig = config.Clone() as SmsConfig;

        }

        public void InitCommands()
        {
            CancelCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage(Messages.DismissSettingsWindow)));
            SaveCommand = new RelayCommand(() =>
                                               {
                                                   settingsRepository.SetSettings(SmsConfig);
                                                   Messenger.Default.Send(new NotificationMessage(Messages.DismissSettingsWindow));
                                               }, () => SmsConfig.IsValid);
        }
    }
}
