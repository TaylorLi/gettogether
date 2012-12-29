using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace GetTogether.Studio.Database.BLL
{
    public class MySQL
    {
        public static GetTogether.Studio.Database.DAL.DO_StoreProcedureParameter.UOList_StoreProcedureParameter GetStoreProcedureParameters(ProjectParameter projParam, string spName)
        {
            string sql = StoreProcedure.GetStoreProcedureText(projParam, spName);
            bool isFoundStoreProcName = false;
            GetTogether.Studio.Database.DAL.DO_StoreProcedureParameter.UOList_StoreProcedureParameter spParameters =
                new GetTogether.Studio.Database.DAL.DO_StoreProcedureParameter.UOList_StoreProcedureParameter();
            foreach (string s in sql.Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                if (string.IsNullOrEmpty(s) || s.Trim() == "") continue;
                string sUpper = s.Trim().ToUpper();
                if (sUpper.IndexOf(spName.ToUpper()) > 0)
                {
                    isFoundStoreProcName = true;
                    continue;
                }
                if (sUpper.IndexOf("BEGIN") >= 0)
                {
                    break;
                }
                if (isFoundStoreProcName)
                {
                    foreach (string sParameter in s.Split(','))
                    {
                        if (sParameter.Trim() == "") continue;
                        GetTogether.Studio.Database.DAL.DO_StoreProcedureParameter.UO_StoreProcedureParameter p =
                            new GetTogether.Studio.Database.DAL.DO_StoreProcedureParameter.UO_StoreProcedureParameter();
                        string[] parameterInfo = sParameter.Trim().Replace("   ", " ").Replace("  ", " ").Split(' ');
                        if (parameterInfo.Length >= 3)
                        {
                            p.IsOutParam = parameterInfo[0].Trim().ToUpper() == "OUT" ? 1 : 0;
                            p.Name = parameterInfo[1];
                            string[] dataTypeInfo = parameterInfo[2].Split('(');
                            if (dataTypeInfo.Length > 1)
                            {
                                p.DataType = dataTypeInfo[0];
                                p.Length = (short)GetTogether.Utility.NumberHelper.ToInt(dataTypeInfo[1].Replace(")", ""), 0);
                            }
                            else
                            {
                                p.DataType = parameterInfo[2];
                            }
                            spParameters.Add(p);
                        }
                    }
                }
            }
            return spParameters;
        }
    }
}
