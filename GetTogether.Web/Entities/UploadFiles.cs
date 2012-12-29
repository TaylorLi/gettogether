using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Web.Entities
{
    public class UploadFiles : GetTogether.ObjectBase.ListBase<UploadFiles.UploadFile>
    {
        public UploadFiles()
        {

        }

        public static UploadFiles GetFiles(int count)
        {
            UploadFiles ret = new UploadFiles();
            for (int i = 1; i <= count; i++)
            {
                UploadFiles.UploadFile f = new UploadFile();
                f.File = string.Format("<input type=\"file\" name=\"UploadFile_{0}_File\" style=\"width:100%;\" />", i.ToString());
                f.FileSaveTo = string.Format("<input type=\"text\" name=\"UploadFile_{0}_FileSaveTo\" style=\"width:99%;\" />", i.ToString());
                ret.Add(f);
            }
            return ret;
        }

        public class UploadFile
        {
            private string _File;

            public string File
            {
                get { return _File; }
                set { _File = value; }
            }

            private string _FileSaveTo;

            public string FileSaveTo
            {
                get { return _FileSaveTo; }
                set { _FileSaveTo = value; }
            }

            public UploadFile()
            {

            }
        }

        public static string SaveUploadFiles()
        {
            StringBuilder sbLogs = new StringBuilder();
            List<UploadFiles.UploadFile> ups = GetTogether.Web.WebHelper.GetListFromRequest<UploadFiles.UploadFile>("UploadFile_");
            foreach (UploadFiles.UploadFile f in ups)
            {
                string path = System.Web.HttpContext.Current.Server.MapPath(string.Concat("~/", f.FileSaveTo));
                if (!System.IO.Directory.Exists(path))
                {
                    sbLogs.AppendFormat("Not existed directory : \"{0}\"", path);
                    continue;
                }
                foreach (string k in System.Web.HttpContext.Current.Request.Files.Keys)
                {
                    if (string.IsNullOrEmpty(System.Web.HttpContext.Current.Request.Files[k].FileName))
                    {
                        continue;
                    }
                    if (System.Web.HttpContext.Current.Request.Files[k].FileName == f.File)
                    {
                        string[] fileNameArr = System.Web.HttpContext.Current.Request.Files[k].FileName.Split('\\');
                        string filePath = System.IO.Path.Combine(path, fileNameArr[fileNameArr.Length - 1]);
                        System.Web.HttpContext.Current.Request.Files[k].SaveAs(filePath);
                        sbLogs.Append(string.Concat("Upload completed : \"", f.File, "\" save to \"", filePath, "\"<hr />"));
                    }
                }
            }
            if (sbLogs.Length == 0)
            {
                sbLogs.Append("Not upload file found");
            }
            return sbLogs.ToString();
        }

        public override string ToString()
        {
            System.Web.UI.WebControls.Table tb = Web.TableHelper.GenListToTable(this);
            if (tb != null)
            {
                tb.Rows[0].Cells[0].Text = "File";
                tb.Rows[0].Cells[1].Text = "Save As";
                tb.Style.Add("style", "font-size:10pt;font-family: \"ËÎÌו\", \"Tahoma\", \"Geneva\", sans-serif;");

                StringBuilder sbHtml = new StringBuilder();
                sbHtml.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
                sbHtml.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
                sbHtml.Append("<head><title>Upload Management</title></head>");
                sbHtml.Append("<body>");
                sbHtml.AppendFormat("<form id=\"form-files\" action=\"PUBLISH_FILES.ASPX?{0}=1\" method=\"post\" enctype=\"multipart/form-data\">", GetTogether.Web.RequestHandler.GetDynamicsKey(true));
                //sbHtml.Append("<input type=\"file\" name=\"file-1\" style=\"width:100%;\" />");
                sbHtml.Append(GetTogether.Web.ControlHelper.ControlToHtml(tb));
                sbHtml.Append("<input type=\"hidden\" id=\"UploadFile_Num\" name=\"UploadFile_Num\" value=\"10\" />");
                sbHtml.Append("<div style=\"border-bottom:solid 1px #000;margin:10px 0px;\"></div>");
                sbHtml.Append("<div style=\"text-align:center;\"><input type=\"button\" style=\"padding:5px;\" onclick=\"document.getElementById('form-files').submit();\" value=\"Upload\" /></div>");
                sbHtml.Append("</form>");
                sbHtml.Append("</body>");
                sbHtml.Append("</html>");
                return sbHtml.ToString();
            }
            else
            {
                return base.ToString();
            }
        }
    }
}


