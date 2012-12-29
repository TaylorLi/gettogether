using System;
using System.Collections.Generic;
using System.Text;
using GetTogether.Utility;

namespace GetTogether.Studio
{
    public class OriginalSettings : GetTogether.ObjectBase.ConfigBase<OriginalSettings>
    {
        private bool _IsDebugMode;

        public bool IsDebugMode
        {
            get { return _IsDebugMode; }
            set { _IsDebugMode = value; }
        }

        private int _CommandTimeout = 15;

        public int CommandTimeout
        {
            get { return _CommandTimeout; }
            set { _CommandTimeout = value; }
        }

        private string _LogConfig;

        public string LogConfig
        {
            get { return _LogConfig; }
            set { _LogConfig = value; }
        }

        private bool _EnableCredential;

        public bool EnableCredential
        {
            get { return _EnableCredential; }
            set { _EnableCredential = value; }
        }

        private string _CredentialHost;

        public string CredentialHost
        {
            get { return _CredentialHost; }
            set { _CredentialHost = value; }
        }

        private string _CredentialPassword;

        public string CredentialPassword
        {
            get { return _CredentialPassword; }
            set { _CredentialPassword = value; }
        }

        private string _CredentialUserName;

        public string CredentialUserName
        {
            get { return _CredentialUserName; }
            set { _CredentialUserName = value; }
        }

        private bool _EnableWarningReport;

        public bool EnableWarningReport
        {
            get { return _EnableWarningReport; }
            set { _EnableWarningReport = value; }
        }

        private bool _EnableErrorReport;

        public bool EnableErrorReport
        {
            get { return _EnableErrorReport; }
            set { _EnableErrorReport = value; }
        }

        private string _ReporterEmail;

        public string ReporterEmail
        {
            get { return _ReporterEmail; }
            set { _ReporterEmail = value; }
        }

        private string _RecipientEmails;

        public string RecipientEmails
        {
            get { return _RecipientEmails; }
            set { _RecipientEmails = value; }
        }

        private string _EmailServer;

        public string EmailServer
        {
            get { return _EmailServer; }
            set { _EmailServer = value; }
        }

        private bool _LogDebug;

        public bool LogDebug
        {
            get { return _LogDebug; }
            set { _LogDebug = value; }
        }
        private bool _LogInfo;

        public bool LogInfo
        {
            get { return _LogInfo; }
            set { _LogInfo = value; }
        }
        private string _EncrKey;

        public string EncrKey
        {
            get { return _EncrKey; }
            set { _EncrKey = value; }
        }

        private bool _EnableDatabaseLogging;

        public bool EnableDatabaseLogging
        {
            get { return _EnableDatabaseLogging; }
            set { _EnableDatabaseLogging = value; }
        }

        private string _Version;

        public string Version
        {
            get { return _Version; }
            set { _Version = value; }
        }

        private string _ManageFileUploadUrl;

        public string ManageFileUploadUrl
        {
            get { return _ManageFileUploadUrl; }
            set { _ManageFileUploadUrl = value; }
        }
        private string _ManageFileUploadPath;

        public string ManageFileUploadPath
        {
            get { return _ManageFileUploadPath; }
            set { _ManageFileUploadPath = value; }
        }
        private string _ScriptStyleVersion;

        public string ScriptStyleVersion
        {
            get { return _ScriptStyleVersion; }
            set { _ScriptStyleVersion = value; }
        }

        public OriginalSettings()
        {

        }

        public override void ReadSetting()
        {
            base.ReadSetting();
            GetTogether.Utility.ConfigManager.InitConfig<OriginalSettings>(ref Config.Original, Config.Mode);
        }

        public override void InitSetting()
        {
            base.InitSetting();
            if (!string.IsNullOrEmpty(Config.Original.RecipientEmails))
                Config.RecipientEmails = Config.Original.RecipientEmails.Split(',');

            if (Config.Original.CommandTimeout == 0) Config.Original.CommandTimeout = 15;

        }

        public override void SaveSetting()
        {
            base.SaveSetting();
            GetTogether.Utility.ConfigManager.WriteConfig<OriginalSettings>(Config.Original, Config.Mode);
        }


    }
}
