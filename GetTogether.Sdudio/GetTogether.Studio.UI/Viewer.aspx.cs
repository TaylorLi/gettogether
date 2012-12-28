using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class WebService_Viewer : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Request.Form.Count > 0)
        {
            string contentType = "text/xml";
            string content = string.Empty;
            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("result-"))
                {
                    content = Request.Form[key];
                }
                else if (key.StartsWith("type-"))
                {
                    contentType = Request.Form[key];
                }
            }
            if (contentType == "text/xml" && Request.Browser.Type.ToLower().IndexOf("opera") >= 0)
            {
                contentType = "text/plain";
            }
            Response.ContentType = contentType;
            Response.Write(content);
            Response.End();
        }
        else
        {
            Response.Write("Invalid parameter(No request found!)");
        }
    }
}