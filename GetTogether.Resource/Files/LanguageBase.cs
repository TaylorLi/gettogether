using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace GetTogether.Resource.Files
{
    public class LanguageBase<T>
        where T : LanguageObj, new()
    {
        public LanguageBase()
        {

        }

        public virtual void Load()
        {

        }
        public void LoadResourecesFromFile(string filePath, string cacheKey, Type resourceEnumType)
        {
            Type objType = this.GetType();
            foreach (string k in Enum.GetNames(resourceEnumType))
            {
                FieldInfo fi = objType.GetField(k);
                if (fi != null)
                {
                    foreach (PropertyInfo pi in fi.FieldType.GetProperties())
                    {
                        if (pi.CanWrite)
                            pi.SetValue(fi.GetValue(this), LanguageHelper.GetResource(LanguageHelper.GetResoureces(PropertyNameToLang(pi.Name), filePath, cacheKey), k), null);
                    }
                }
            }
            //Utility.CacheHelper.RemoveCaches(cacheKey, Utility.CacheHelper.RemoveCacheType.StartWith);
        }

        public void LoadResourecesFromFile<R>(string filePath, string cacheKey, Dictionary<string, R> objs) where R : class, new()
        {
            foreach (string k in objs.Keys)
            {
                Type objType = objs[k].GetType();
                foreach (PropertyInfo pi in objType.GetProperties())
                {
                    if (pi.CanWrite)
                        pi.SetValue(objs[k], LanguageHelper.GetResource(LanguageHelper.GetResoureces(PropertyNameToLang(pi.Name), filePath, cacheKey), k), null);
                }
            }
            //Utility.CacheHelper.RemoveCaches(cacheKey, Utility.CacheHelper.RemoveCacheType.StartWith);
        }

        private static string PropertyNameToLang(string name)
        {
            return name.Replace("_", "-");
        }
    }
}
