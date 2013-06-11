using GalaSoft.MvvmLight.Ioc;
using OPSSDK;
using OzCommon.Utils.Schedule;
using OzCommonBroadcasts.Model;

namespace OPSSMSSender.Model
{
    public class SmsWorkerFactory : IWorkerFactory<SmsEntry>
    {
        private IExtensionContainer extensionContainer;
        public SmsWorkerFactory()
        {
            extensionContainer = SimpleIoc.Default.GetInstance<IExtensionContainer>();
        }
        public IWorker CreateWorker(SmsEntry work)
        {
            return new SmsWorker(work, extensionContainer.GetExtension());
        }
    }
}
