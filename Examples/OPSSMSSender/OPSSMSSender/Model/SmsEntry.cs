using System.ComponentModel;
using OzCommon.Model;
using OzCommonBroadcasts.Model;

namespace OPSSMSSender.Model
{
    public class SmsEntry : BaseModel, ICompletedWork, IDataErrorInfo
    {
        #region Properties

        private string recipient;
        private string content;
        private WorkState state;
        bool isCompleted;

        #region Properties with OnProertyChanged
        public string Recipient
        {
            get { return recipient; }
            set { recipient = value; Raise(() => Recipient); }
        }
        public string Content
        {
            get { return content; }
            set { content = value; Raise(() => Content); }
        }

        [ReadOnlyProperty]
        [ExportIgnoreProperty]
        public WorkState State
        {
            get { return state; }
            set { state = value; Raise(() => State); }
        }

        [InvisibleProperty]
        [ExportIgnoreProperty]
        public bool IsCompleted
        {
            get { return isCompleted; }
            set { isCompleted = value; Raise(() => IsCompleted); }
        }

        #endregion
 
        #endregion

        public SmsEntry()
        {
            state = WorkState.Init;
        }

        public SmsEntry(string name, WorkState state, string phoneNumber)
        {
            Recipient = phoneNumber;
            Content = name;
            State = state;
        }

        [InvisibleProperty]
        [ExportIgnoreProperty]
        public string Error { get; private set; }

        [InvisibleProperty]
        [ExportIgnoreProperty]
        public bool IsValid
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(Recipient));
            }
        }

        [InvisibleProperty]
        [ExportIgnoreProperty]
        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "Recipient":
                        if (string.IsNullOrWhiteSpace(Recipient))
                            return "Recipient ID cannot be empty";
                        break;
                }
                return null;
            }
        }
    }
}
