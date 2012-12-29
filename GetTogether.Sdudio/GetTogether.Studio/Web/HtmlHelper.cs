using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;
using System.Collections.Generic;

namespace GetTogether.Studio.Web
{
    public class HtmlHelper
    {
        public static string[] PAGE_SELECTOR = new string[] { "5,5", "10,10", "20,20", "30,30", "50,50", "100,100", "1000,All" };
        public const string STR_SELECT = "<select id=\"{0}\" name=\"{0}\" class={1} title=\"{2}\">{3}</select>";
        public const string STR_SELECT_WITH_EVENT = "<select id=\"{0}\" name=\"{0}\" class={1} title=\"{2}\" {4}>{3}</select>";
        public const string STR_SELECT_ITEM = "<option value=\"{0}\">{1}</option>";
        public const string STR_SELECT_ITEM_SELECTED = "<option value=\"{0}\" selected=\"selected\">{1}</option>";
        private static string _SortHeaderFmt = "<a href=\"javascript:{0}\" style=\"white-space:nowrap;\">{1}{2}</a>";
        private static string _SortHeaderWithStlyleFmt = "<a href=\"javascript:{0}\" style=\"{3}\">{1}</a>{2}";
        private const string MENU_MAIN = "<div id=\"tabs11\" style='font-weight: bold;'><ul>{0}<li></ul></div>";
        private const string MENU = "<li><a href=\"{0}\"><span>{1}</span></a></li>";
        private const string MENU_ACTIVE = "<li id=\"current\"><a href=\"{0}\"><span>{1}</span></a></li>";
        private const string MAIN_MENU = "<li class=\"mm-normal\"><a href=\"{0}{1}\">{2}</a></li>";
        private const string MAIN_MENU_ACTIVE = "<li class=\"mm-active\"><a href=\"{0}{1}\">{2}</a></li>";

        public static string GenMenus(System.Web.UI.Page p)
        {
            StringBuilder sb_menus = new StringBuilder();
            string absolute_url = p.Request.Url.AbsolutePath;
            System.Collections.Generic.List<string> lMenus = new System.Collections.Generic.List<string>();
            //lMenus.Add("1,Home.aspx,Home|Home.aspx");
            lMenus.Add("1,WebService/Index.aspx,Web Service|WebService/Index.aspx,WebService/Projects.aspx,WebService/Edit.aspx");
            lMenus.Add("1,Database/Index.aspx,Database|Database/Index.aspx,Database/Projects.aspx,Database/Template.aspx,Database/Edit.aspx");
            lMenus.Add("1,XML/Index.aspx,XML Tools|XML/Index.aspx,XML/Edit.aspx");
            if (SessionHelper.GetCurrentSession().UserCode == "127.0.0.1")
                lMenus.Add("1,Tools/SVN/Index.aspx,SVN Tools|Tools/SVN/Index.aspx,Tools/SVN/BackupChangedFiles.aspx");

            //lMenus.Add("1,Downloads/Index.aspx,Downloads|Downloads/Index.aspx");
            string resolve_url = p.ResolveUrl("~");
            bool isFirst = true;
            foreach (string s in lMenus)
            {
                string[] menu_info = s.Split('|');
                string[] menu_detail = menu_info[0].Split(',');
                string[] menu_page_ref = menu_info[1].Split(',');
                bool is_find_active_page = false;
                if (!isFirst) sb_menus.Append("<li class=\"mm-split\">|</li>");
                foreach (string pr in menu_page_ref)
                {
                    if (absolute_url.ToLower().EndsWith(pr.ToLower()))
                    {
                        sb_menus.Append(string.Format(MAIN_MENU_ACTIVE, resolve_url, menu_detail[1], menu_detail[2]));
                        is_find_active_page = true;
                        isFirst = false;
                        break;
                    }
                }
                if (!is_find_active_page)
                {
                    sb_menus.Append(string.Format(MAIN_MENU, resolve_url, menu_detail[1], menu_detail[2]));
                    isFirst = false;
                }
            }
            return sb_menus.ToString();
        }

        public static string MsgBoxHtml(string text, System.Web.UI.Page page)
        {
            string MsgBoxFormat = "<table width=\"100%\"><tr><td align=\"center\"><table border=\"0px\" cellpadding=\"0px\" cellspacing=\"0px\" width=\"450px\"><tr><td class=\"ad-lt\" /><td class=\"ad-mt\" /><td class=\"ad-rt\" /></tr><tr><td class=\"ad-l\" /><td class=\"ad-bg\" align=\"left\" style=\"height: 100px;\"><table width=\"90%\" border=\"0\"><tr><td style=\"padding: 5px 10px 5px 10px; width: 10%\">{0}</td><td align=\"left\" style=\"width: 90%\">{1}</td></tr></table></td><td class=\"ad-r\" /></tr><tr><td class=\"ad-lb\" /><td class=\"ad-mb\" /><td class=\"ad-rb\" /></tr></table></td></tr></table>";
            string Img = "<img src=\"" + page.ResolveUrl("~") + "images/error1.gif\" alt=\"\" />";
            return string.Format(MsgBoxFormat, Img, text);
        }

        public static string GetSort(string key, string sortColumn, bool isAsc)
        {
            if (key.ToLower() == sortColumn.ToLower())
            {
                if (isAsc)
                    return "<span class=\"asc\">&nbsp;</span>";
                else
                    return "<span class=\"desc\">&nbsp;</span>";
            }
            return string.Empty;
        }

        public static string GetSortHeader(string js, string title, string sortBy, bool isAsc, string currentSortBy)
        {
            return string.Format(_SortHeaderFmt, js, title, GetSort(currentSortBy, sortBy, isAsc));
        }

        public static string GetSortHeader(string js, string title, string sortBy, bool isAsc, string currentSortBy, string linkStyle)
        {
            return string.Format(_SortHeaderWithStlyleFmt, js, title, GetSort(currentSortBy, sortBy, isAsc), linkStyle);
        }

    }
}