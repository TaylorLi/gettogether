using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Web.Entities
{
    public class LibraryInfos : GetTogether.ObjectBase.ListBase<LibraryInfos.LibraryInfo>
    {
        private string BasePath = string.Empty;
        public LibraryInfos()
        {

        }

        //public LibraryInfos(string path, string searchParttern)
        //{
        //    string[] files = System.IO.Directory.GetFiles(path, searchParttern);
        //    foreach (string f in files)
        //    {
        //        System.IO.FileInfo fi = new System.IO.FileInfo(f);
        //        System.Diagnostics.FileVersionInfo vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(f);
        //        this.Add(new LibraryInfos.LibraryInfo(fi.Name, fi.CreationTime, fi.LastWriteTime, vi.FileVersion));
        //    }
        //}

        public LibraryInfos(string path, string searchParttern)
        {
            BasePath = path;
            GetFiles(path, searchParttern);
        }

        private void GetFiles(string path, string searchParttern)
        {
            string[] files = System.IO.Directory.GetFiles(path, searchParttern);
            foreach (string f in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(f);
                System.Diagnostics.FileVersionInfo vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(f);
                this.Add(new LibraryInfos.LibraryInfo(fi.FullName.Replace(BasePath, ""), fi.CreationTime, fi.LastWriteTime, vi.FileVersion));
            }
            string[] folders = System.IO.Directory.GetDirectories(path);
            if (folders != null && folders.Length > 0)
            {
                foreach (string folder in folders)
                {
                    GetFiles(folder, searchParttern);
                }
            }
        }

        public override string ToString()
        {
            this.SortBy("Update_On", false);
            System.Web.UI.WebControls.Table tb = Web.TableHelper.GenListToTable(this);
            if (tb != null && tb.Rows != null && tb.Rows.Count > 0)
            {
                foreach (System.Web.UI.WebControls.TableCell tc in tb.Rows[0].Cells)
                {
                    if (string.IsNullOrEmpty(tc.Text)) continue;
                    tc.Text = tc.Text.Replace("_", " ");
                }
                tb.Style.Add("style", "font-size:10pt;font-family: \"ËÎÌו\", \"Tahoma\", \"Geneva\", sans-serif;");
                return Web.ControlHelper.ControlToHtml(tb);
            }
            else
            {
                return base.ToString();
            }
        }

        public class LibraryInfo
        {
            private string _File_Name;

            public string File_Name
            {
                get { return _File_Name; }
                set { _File_Name = value; }
            }

            private DateTime _Create_On;

            public DateTime Create_On
            {
                get { return _Create_On; }
                set { _Create_On = value; }
            }

            private DateTime _Update_On;

            public DateTime Update_On
            {
                get { return _Update_On; }
                set { _Update_On = value; }
            }

            private string _Version;

            public string Version
            {
                get { return _Version; }
                set { _Version = value; }
            }

            public LibraryInfo()
            {

            }
            public LibraryInfo(string fileName, DateTime createOn, DateTime updateOn, string version)
            {
                this.File_Name = fileName;
                this.Create_On = createOn;
                this.Update_On = updateOn;
                this.Version = version;
            }
        }
    }
}
