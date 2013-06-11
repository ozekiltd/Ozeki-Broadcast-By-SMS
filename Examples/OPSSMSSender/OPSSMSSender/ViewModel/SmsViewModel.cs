using GalaSoft.MvvmLight.Ioc;
using OPSSMSSender.Model;
using OPSSMSSender.Model.Config;
using OzCommon.Model;
using OzCommonBroadcasts.ViewModel;

namespace OPSSMSSender.ViewModel
{
    public class SmsViewModel : BroadcastMainViewModel<SmsEntry>
    {
        private readonly IGenericSettingsRepository<SmsConfig> settingsRepository;

        public SmsViewModel()
        {
            settingsRepository = SimpleIoc.Default.GetInstance<IGenericSettingsRepository<SmsConfig>>();
        }

        protected override object GetSettingsViewModel()
        {
            return new SettingsViewModel();
        }

        protected override int GetMaxConcurrentWorkers()
        {
            var config = settingsRepository.GetSettings();
            return config != null ? config.ConcurrentWorks : 1; //Default value is 1
        }

        protected override string GetApiExtensionID()
        {
            var settings = settingsRepository.GetSettings();
            return settings == null ? "" : settings.ExtensionId;
        }
    }
}
