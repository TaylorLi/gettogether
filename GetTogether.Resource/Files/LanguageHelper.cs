using System;
using System.Collections.Generic;
using System.Text;
using GetTogether.Utility;
using System.IO;

namespace GetTogether.Resource.Files
{
    public class LanguageHelper
    {
        static Dictionary<string, object> caches = new Dictionary<string, object>();

        public static void ClearCaches()
        {
            caches.Clear();
        }

        public static Language GetResoureces(GetTogether.Utility.MutiLanguage.Languages lang, string filePath, string cacheKey)
        {
            return GetResoureces(GetTogether.Utility.MutiLanguage.EnumToString(lang), filePath, cacheKey);
        }

        public static Language GetResoureces(string lang, string filePath, string cacheKey)
        {
            cacheKey = string.Concat(cacheKey, lang);
            //Language resoureces = CacheHelper.GetCache(cacheKey) as Language;
            Language resoureces = null;
            if (caches.ContainsKey(cacheKey)) resoureces = caches[cacheKey] as Language;
            if (resoureces == null)
            {
                FileInfo fi = new FileInfo(filePath);
                string postfix;
                if (lang == MutiLanguage.EnumToString(MutiLanguage.Languages.en_us)) postfix = fi.Extension;
                else postfix = string.Concat(".", lang, fi.Extension);
                filePath = fi.FullName.Replace(fi.Extension, postfix);
                if (File.Exists(filePath))
                {
                    resoureces = new Language().FormXml(File.ReadAllText(filePath));
                    //CacheHelper.SetCache(cacheKey, resoureces);
                    caches[cacheKey] = resoureces;
                }
            }
            return resoureces;
        }

        public static string GetResource(Language rs, object rn)
        {
            return GetResource(rs, rn.ToString());
        }

        public static string GetResource(Language rs, string key)
        {
            string resource = string.Empty;
            if (rs != null && rs.Items != null && rs.Items.Length > 0)
            {
                foreach (LanguagesLanguage r in rs.Items)
                {
                    if (r.Key == key)
                        resource = r.Value;
                }
            }
            return resource;
        }

        public static string GetResourceFromFile(string filePath, MutiLanguage.Languages language)
        {
            string lang = MutiLanguage.EnumToString(language);
            string text = "";
            FileInfo fi = new FileInfo(filePath);
            string postfix;
            if (lang == MutiLanguage.EnumToString(MutiLanguage.Languages.en_us)) postfix = fi.Extension;
            else postfix = string.Concat(".", lang, fi.Extension);
            filePath = fi.FullName.Replace(fi.Extension, postfix);
            if (File.Exists(filePath))
            {
                text = File.ReadAllText(filePath, Encoding.UTF8);
            }
            return text;
        }
        public static LanguageObj GetResourceFromFile(string filePath)
        {
            return new LanguageObj(GetResourceFromFile(filePath, MutiLanguage.Languages.en_us), GetResourceFromFile(filePath,
                 MutiLanguage.Languages.zh_cn), GetResourceFromFile(filePath, MutiLanguage.Languages.zh_tw));
        }
    }
}
