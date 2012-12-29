using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Web.UI
{
    public class PageConfigManager<S> : PageAdmin
        where S : ObjectBase.ConfigBase<S>, new()
    {
        #region Properties

        /// <summary>
        /// key,type,value
        /// </summary>
        public readonly string SPLIT_FORMAT = string.Concat("{0}", GetTogether.Utility.ConfigManager.SPLIT_FLAG, "{1}", GetTogether.Utility.ConfigManager.SPLIT_FLAG, "{2}");

        private S _Original;

        public virtual S Original
        {
            get { return _Original; }
            set { _Original = value; }
        }

        private string _EncrKey;

        public virtual string EncrKey
        {
            get { return _EncrKey; }
            set { _EncrKey = value; }
        }

        private GetTogether.Utility.ConfigManager.SystemMode _Mode;

        public virtual GetTogether.Utility.ConfigManager.SystemMode Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        #endregion

        public void GetConfig(string key)
        {
            string[] c = Utility.ConfigHelper.GetConfig<S>(Original, key);
            if (c != null)
            {
                Response.Write(Encrypt(string.Format(SPLIT_FORMAT, c[0], c[1], c[2])));
            }
        }

        public void SetConfig(string key, string value)
        {
            try
            {
                GetTogether.Mapping.ObjectHelper.SetValue<S>(Original, key, value);
                string[] c = Utility.ConfigHelper.GetConfig<S>(Original, key);
                if (c != null)
                {
                    OK(Encrypt(string.Format(SPLIT_FORMAT, c[0], c[1], c[2])));
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        public void GetAllConfigs()
        {
            bool isFirst = true;
            StringBuilder sb = new StringBuilder();
            foreach (string[] c in Utility.ConfigHelper.GetConfigs<S>(this.Original))
            {
                if (!isFirst)
                {
                    sb.Append(Definition.SPLIT_ROW_FLAG);
                }
                sb.AppendFormat(SPLIT_FORMAT, c[0], c[1], c[2]);
                if (isFirst)
                {
                    isFirst = false;
                }
            }
            Response.Write(Encrypt(sb.ToString()));
        }

        private string Decrypt(string text)
        {
            return Utility.SecretHelper.DesDecrypt(text, EncrKey);
        }

        private string Encrypt(string text)
        {
            return Utility.SecretHelper.DesEncrypt(text, EncrKey);
        }

        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);
            string value = Request["value"];
            if (!string.IsNullOrEmpty(value))
            {
                value = Decrypt(value);
            }
            string dkey = key;
            if (!string.IsNullOrEmpty(dkey))
            {
                dkey = Decrypt(dkey);
            }
            switch (type)
            {
                case 0:
                    Response.Write(((int)Mode).ToString());
                    break;
                case 1:
                    GetConfig(dkey);
                    break;
                case 2:
                    SetConfig(dkey, value);
                    Original.InitSetting();
                    Original.SaveSetting();
                    break;
                case 3:
                    GetAllConfigs();
                    break;
                default:
                    break;
            }
        }
    }
}
