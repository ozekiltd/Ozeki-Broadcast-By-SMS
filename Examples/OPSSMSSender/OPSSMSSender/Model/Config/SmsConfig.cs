using System;
using System.ComponentModel;
using OzCommon.Model;

namespace OPSSMSSender.Model.Config
{
    public class SmsConfig : BaseModel, ICloneable, IDataErrorInfo
    {
        #region Properties

        private Int32 concurrentWorks;
        string extensionId;

        #region Properties => OnPropertyChanged

        public string ExtensionId
        {
            get { return extensionId; }
            set { extensionId = value; Raise(() => ExtensionId); }
        }

        public Int32 ConcurrentWorks
        {
            get { return concurrentWorks; }
            set { concurrentWorks = value; Raise(() => ConcurrentWorks); }
        }

        #endregion

        #endregion

        public SmsConfig()
        {
            ConcurrentWorks = 1;
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "ConcurrentWorks":
                        if (!(ConcurrentWorks > 0))
                            return "Concurrent works must be greater than 0";
                        break;
                    case "ExtensionId":
                        if (string.IsNullOrWhiteSpace(ExtensionId))
                            return "Extension ID cannot be empty";
                        break;
                }

                return null;
            }
        }

        public string Error { get; set; }

        public bool IsValid
        {
            get
            {
                return (ConcurrentWorks > 0 && !string.IsNullOrWhiteSpace(ExtensionId));
            }
        }

        public object Clone()
        {
            return new SmsConfig 
                                {   
                                    ConcurrentWorks = this.ConcurrentWorks,
                                    ExtensionId = this.ExtensionId
                                };
        }
    }
}
