<%@ Page Title="" Language="C#" MasterPageFile="~/MP.master" AutoEventWireup="true"
    CodeFile="Edit.aspx.cs" Inherits="WebService_Edit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MP1" runat="Server">
    <%if (string.IsNullOrEmpty(Codes))
      { %>
    <div id="dv-table-web-service">

        <div style="margin-bottom: 5px;">
            <%=Parameter.Category %>&nbsp;&nbsp;<span class="mm-split">|</span>&nbsp;&nbsp;
            <%=Parameter.ProjectName%>&nbsp;&nbsp;<span class="mm-split">|</span>&nbsp;&nbsp;<a
                href="javascript:;;" onclick="GetWebMethod(true);">Refresh Method(s)</a>&nbsp;&nbsp;<span
                    class="mm-split">|</span>&nbsp;&nbsp;<a href="<%=Parameter.Address %>" target="_blank"
                        style="color: #676767;font-style:italic;"><%=Parameter.Address%></a>&nbsp;&nbsp;<span class="mm-split">|</span>&nbsp;&nbsp;<a
                            href="<%=Parameter.Address %>?WSDL" target="_blank" style="color: #676767;">WSDL</a>
            &nbsp;&nbsp;<span class="mm-split">|</span>&nbsp;&nbsp;<a href="Edit.aspx?pn=<%=Request["pn"] %>&gc=1"
                target="_blank" style="color: #676767;">Code</a>
        </div>
        <%--<div class="line-sub" style="margin-bottom: 5px;"></div>--%>
        <div style="float: left; width: 25%;">
            <div id="dv-web-methods" style="min-height: 200px;">
                <div id="dv-web-methods-default">
                </div>
            </div>
        </div>
        <div style="width: 75%; float: right;">
            <div style="margin: 0px 0px 5px 5px;" id="dv-web-service-content">
                <div class="box-option">
                    Request&nbsp;(Double click on the textarea to enlarge)
                </div>
                <div class="box">
                    <div id="dv-web-method-info">
                        <div style="min-height: 100px;">
                        </div>
                    </div>
                </div>
                <div class="box-option" style="margin-top: 5px;">
                    Response&nbsp;(Double click on the textarea to enlarge)
                </div>
                <div class="box">
                    <div id="dv-web-method-result">
                        <div style="min-height: 100px;">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <input type="hidden" id="init-history" />
    <input type="hidden" id="init-method" />
    <script language="javascript" type="text/javascript">
        var ProjectName = '<%=Request["pn"] %>';
        $(document).ready(function () {
            $('#init-history').val(getUrlParam('history'));
            $('#init-method').val(getUrlParam('method'));
            GetWebMethod(false);
        });
        function InitInvokeMethod() {
            var methodName = $('#init-method').val();
            var historyName = $('#init-history').val();
            if (!IsEmpty(historyName) && !IsEmpty(methodName)) {
                LoadRecent(methodName, historyName);
            }
        }
        function GetWebMethod(isRefresh) {
            $('#dv-web-methods-default').css('height', $(window).height() - 62 + 'px');
            var refreshParam = isRefresh ? '&refresh=1' : '';
            CommonCall('WebService/Callback/Edit.aspx', 0, 'dv-web-methods', 'dv-web-methods', 'dv-web-methods', '', '&pn=' + ProjectName + refreshParam, 'text', function () {
                $('#dv-web-method-list').css('height', $(window).height() - 90 + 'px');
                InitInvokeMethod();
            });
        }
        function GetWebMethodInfo(methodName, actionType, historyName) {
            document.title = methodName + ' - ' + ProjectName;
            if (actionType == null) actionType = 1;
            var methodDiv = $('#dv-method-' + methodName);
            $('body').css('overflow', 'hidden');
            $("#dv-web-methods").find('a[onclick^="GetWebMethodInfo"]').each(function () {
                if ($(this).html().trim() == methodName) {
                    $(this).parent().parent().attr('class', 'focus');
                }
                else {
                    $(this).parent().parent().attr('class', '');
                }
            });
            if (methodDiv.length > 0) {
                if ($('#invoke-' + methodName).length > 0) {
                    $('#dv-web-method-info').find('div[id^="dv-method-"]').hide();
                    methodDiv.fadeIn();
                    $('div[id^="dv-result-"]').hide();
                    $('#dv-result-' + methodName).fadeIn();
                    $('body').css('overflow', '');
                    return;
                }
                else {
                    $('#dv-method-' + methodName).remove();
                    $('#dv-result-' + methodName).remove();
                }
            }
            CommonCall('WebService/Callback/Edit.aspx', actionType, '', 'dv-web-service-content', '', '', '&pn=' + ProjectName + '&mn=' + methodName + '&history=' + historyName, 'text', function (data) {
                var methodDivs = $('#dv-web-method-info').find('div[id^="dv-method-"]');
                var isFirst = methodDivs.length == 0;
                if (isFirst) {
                    $('#dv-web-method-info').html(data);
                }
                else {
                    methodDivs.hide();
                    $('#dv-web-method-info').append(data);
                }
                EnhanceTextarea('dv-method-' + methodName);
                if ($('#dv-web-method-result').find('div[id^="dv-result-"]').length == 0) {
                    $('#dv-web-method-result').html('<div id="dv-result-' + methodName + '" style="min-height: 100px;"></div>');
                }
                else {
                    $('#dv-web-method-result').find('div[id^="dv-result-"]').hide();
                    if ($('#dv-result-' + methodName).length == 0) {
                        $('#dv-web-method-result').append('<div id="dv-result-' + methodName + '" style="min-height: 100px;"></div>');
                    }
                }
                $('body').css('overflow', '');
                if (actionType == 3 && !IsEmpty($('#init-method').val())) {
                    window.setTimeout(function () {
                        $('#init-method').val('');
                        $('#init-history').val('');
                        InvokeWebMethod(methodName);
                    }, 500);
                }
                if (actionType == 1) {
                    LoadRecent(methodName, 'Recent');
                }
            });
        }
        function LoadRecent(methodName, historyName) {
            if (historyName == null || historyName == '')
                historyName = $('#invoke-history-' + methodName).val();
            $('#dv-method-' + methodName).remove();
            $('#dv-result-' + methodName).remove();
            GetWebMethodInfo(methodName, 3, historyName);
        }
        function InvokeWebMethod(methodName) {
            var paramDvId = 'dv-method-' + methodName;
            var params = 'type=2&pn=' + ProjectName + '&mn=' + methodName + '&history=' + $('#invoke-history-' + methodName).val();
            params += getParams(paramDvId, 'WMP_', 'input,textarea');
            $('#dv-result-' + methodName).html('<span style="padding-left:2px;">Invoking,Please Wait...</span>');
            CommonCall('WebService/Callback/Edit.aspx', 2, '', paramDvId, '', '', params, 'text', function (data) {
                $('#dv-result-' + methodName).html(data);
                EnhanceTextarea('dv-result-' + methodName);
                $('#btn-forward-' + methodName).show();
            });
        }
        function ClearMethod(methodName) {
            $('#dv-method-' + methodName).remove();
            $('#dv-result-' + methodName).remove();
            GetWebMethodInfo(methodName);
        }
        function ShareInvoke(methodName) {
            MsgBox($('#dv-forward-' + methodName).html(), 'Share Invoking Information : "' + methodName + '"', false); //,'dv-result-'+methodName);
            $('#alert_div_MSGBOX').find('textarea')[0].select();
        }
        function DeleteInvokeHistory(methodName, historyName) {
            Confirm('Are you sure you want to delete "' + historyName + '"?', 'Delete Invoke History', false, '', 'DeleteInvokeHistoryExec(\'' + methodName + '\',\'' + historyName + '\');', '', '', '');
        }
        function DeleteInvokeHistoryExec(methodName, historyName) {
            CommonCall('WebService/Callback/Edit.aspx?pn=' + ProjectName + '&mn=' + methodName + '&history=' + historyName, 4, '', 'dv-table-web-service', '', '', '', 'json', function (data) {
                if (data.success) {
                    CM();
                    LoadRecent(methodName, 'Recent');
                }
                else {
                    MsgBox(data.message);
                }
            });
        }
        function ChangeRequestMode(methodName, mode) {
            if (mode == 'soap') {
                $('#dv-parameter-' + methodName).hide();
                $('#dv-parameter-soap-' + methodName).show();
                $('#WMP_Object_' + methodName).val('0');
                $('#WMP_SOAP_' + methodName).val('1');
            }
            else {
                $('#dv-parameter-' + methodName).show();
                $('#dv-parameter-soap-' + methodName).hide();
                $('#WMP_Object_' + methodName).val('1');
                $('#WMP_SOAP_' + methodName).val('0');
            }
        }
        function FormatXml(objId) {
            var paramDvId = '';
            sld(objId);
            var params = 'type=5&xml=' + $('#' + objId).val();
            CommonCall('WebService/Callback/Edit.aspx', 5, '', paramDvId, '', '', params, 'text', function (data) {
                $('#' + objId).val(data);
                CM();
            });
        }
    </script>
    <%}
      else
      { %>
    <div class="box">
        <textarea id="txt-codes" rows="50" style="width: 100%;"><%=Codes %></textarea>
    </div>
    <script language="javascript" type="text/javascript">
        EnhanceTextarea('txt-codes');
        $('#txt-codes').css('height', $(window).height() - 45 + 'px');
    </script>
    <%} %>
</asp:Content>
