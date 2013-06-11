using System;
using OPSSDK;
using OPSSDKCommon.Model.Message;
using OzCommon.Utils.Schedule;

namespace OPSSMSSender.Model
{
    class SmsWorker : IWorker
    {
        readonly SmsEntry smsEntry;
        readonly IAPIExtension apiExtension;
        string messageId;

        public SmsWorker(SmsEntry smsEntry, IAPIExtension extension)
        {
            this.smsEntry = smsEntry;
            apiExtension = extension;
            apiExtension.MessageSubmitted += apiExtension_MessageSubmitted;
        }


        void apiExtension_MessageSubmitted(object sender, MessageResultEventArgs e)
        {
            if (messageId == e.CallbackId)
            {
                OnWorkCompleted(e.Result ? WorkState.Success : WorkState.DeliveringFailed);

                apiExtension.MessageSubmitted -= apiExtension_MessageSubmitted;
            }
        }

        public event EventHandler<WorkResult> WorkCompleted;

        public void StartWork()
        {
            if (smsEntry.State == WorkState.Success)
            {
                OnWorkCompleted(WorkState.Success);
                return;
            }

            if (!smsEntry.IsValid)
            {
                OnWorkCompleted(smsEntry.State);
                return;
            }

            smsEntry.State = WorkState.InProgress;

            var message = new SMSMessage(smsEntry.Recipient, smsEntry.Content);
            messageId = message.MessageId;

            apiExtension.SendMessageAsync(message, result =>
                    {
                        switch (result.RoutingState)
                        {
                            case RoutingState.DestinationAccepted:
                                if (smsEntry.State == WorkState.InProgress)
                                    smsEntry.State = WorkState.Routed;
                                break;
                            case RoutingState.DestinationNotFound:
                                OnWorkCompleted(WorkState.DeliveringFailed);
                                apiExtension.MessageSubmitted -= apiExtension_MessageSubmitted;
                                break;
                        }
                    });
        }

        public void CancelWork()
        {

        }

        private void OnWorkCompleted(WorkState e)
        {
            smsEntry.State = e;

            if (e == WorkState.Success)
                smsEntry.IsCompleted = true;

            var handler = WorkCompleted;
            if (handler != null)
                handler(this, new WorkResult() { IsSuccess = smsEntry.IsCompleted });
        }
    }
}
