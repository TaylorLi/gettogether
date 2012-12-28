<%@ Page Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true" CodeFile="Edit.aspx.cs"
    Inherits="XML_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MP1" runat="Server">
    <div id="dv-parameter">
        <div class="box-option">
            XML Source&nbsp;(Double click on the textarea to enlarge)
        </div>
        <div class="box">
            <textarea id="txtXml" rows="15" cols="20" style="width: 99.8%; "><?xml version="1.0"?>
<BannerCollections xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://www.opentravel.org/OTA/2003/05">
  <Banner>
    <ID>1</ID>
    <IsFlash>True</IsFlash>
    <URL>http://www.google.com</URL>
    <Target>http://www.google.com</Target>
  </Banner>
  <Banner>
    <ID>2</ID>
    <IsFlash>True</IsFlash>
    <URL>http://www.google.com</URL>
    <Target>http://www.google.com</Target>
  </Banner>
</BannerCollections></textarea>
        </div>
                <div class="box" style="margin-top: 5px;padding:2px;">
            <span style="font-weight: bold;">Generate Type</span> <span>
                <input type="radio" id="GenC" name="GenType" checked="checked" /><label for="GenC">Class</label><input
                    type="radio" name="GenType" id="GenS" /><label for="GenS">Data Set</label>
            </span><span style="font-weight: bold;">Name Space</span> <span>
                <input type="text" id="txtNameSpace" class="txt" style="width: 30%;" value="GetTogether.Project.Entity" />
            </span>
            <input type="button" value="Generate" onclick="GenCodeByXml();" class="btn5" />
        </div>
    </div>
    <div class="box-option" style="margin-top: 5px;">
        Generated Code&nbsp;(Double click on the textarea to enlarge)
    </div>
    <div class="box" id="dv-code" style="min-height:100px;">

    </div>
    <script language="javascript">
        $().ready(function () {
            EnhanceTextarea('dv-parameter');
        });
        function GenCodeByXml() {
            CommonCall('XML/Callback/Edit.aspx', 0, 'dv-code', 'dv-parameter', 'dv-parameter', 'XML_', '&ns=' + oa('txtNameSpace') + '&xml=' + oa('txtXml') + (o('GenS').checked ? '&isds=1' : ''), 'text', function () {
                $('#dv-code').hide().fadeIn();
                EnhanceTextarea('dv-code');
            });
        }
    </script>
</asp:Content>
