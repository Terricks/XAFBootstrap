<%@ Control Language="C#" CodeBehind="LogonTemplate.ascx.cs" ClassName="LogonTemplate" Inherits="XAF_Bootstrap.Templates.LogonTemplate"%>
<%@ Register Assembly="XAF Bootstrap" Namespace="XAF_Bootstrap.Controls" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v14.2" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v14.2" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v14.2" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v14.2" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>

<script type="text/ecmascript">
    $('body').removeClass("Dialog");
</script>

<style>    
    body {
        width: 100%;
        height: 100%;
    }
    .Header {
        background: none;
    }
    .Caption, .StaticImage {
        display: none;
}
</style>

<div class="">
    <div class="progress" style="margin: 0;height: 5px; display: none" id="loadingProgress">
        <div class="progress-bar progress-bar-info progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
    </div>
    <div id="pleaseWaitDialog" class="hidden">
        <div class="modal-backdrop fade in" style="position: fixed; bottom: 0px; left: 0px; right: 0px; top: 0px; z-index: 2000; opacity:0"></div>    
    </div>
    <div class="container">
        <div class="row">
            <div class="col-sm-3"></div>
            <div class="col-sm-6">

                <cc3:XafUpdatePanel ID="UPPopupWindowControl" runat="server">
                    <cc4:XafPopupWindowControl runat="server" ID="PopupWindowControl" />
                </cc3:XafUpdatePanel>
                <cc3:XafUpdatePanel ID="UPHeader" runat="server">
                    <div class="Header">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td class="ViewImage">
                                    <cc4:ViewImageControl ID="viewImageControl" runat="server" />
                                </td>
                                <td class="ViewCaption">
                                    <h3>
                                        <cc4:ViewCaptionControl ID="viewCaptionControl" DetailViewCaptionMode="ViewCaption"
                                            runat="server" />
                                    </h3>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <hr />
                </cc3:XafUpdatePanel>

                <div class="well">
                    <cc3:XafUpdatePanel ID="UPEI" runat="server">
                        <cc1:XafBootstrapErrorInfoControl ID="ErrorInfo" Style="margin: 10px 0px 10px 0px" runat="server"/>
                    </cc3:XafUpdatePanel>
            
                    <cc3:XafUpdatePanel ID="UPVSC" runat="server">
                        <cc4:ViewSiteControl ID="viewSiteControl" runat="server" />
                        <br />                    
                        <cc1:XbActionContainerHolder ID="PopupActions" runat="server" Categories="PopupActions" LeftDirection ="false" DefaultIcon="">                            
                        </cc1:XbActionContainerHolder>
                        <br />
                    </cc3:XafUpdatePanel>
                </div>
                <cc3:XafUpdatePanel ID="UPO" runat="server">
                    <cc1:XbActionContainerHolder ID="OptionsActions" runat="server" Categories="Options; Tools;" Tag="a" ItemClass="view-action" ContainerClass="h6" DefaultIcon="">                            
                    </cc1:XbActionContainerHolder>
                </cc3:XafUpdatePanel>    
            </div>
            <div class="col-sm-3"></div>
        </div>
    </div>
</div>

<script>
    $("#logonAction").text($('#Logon_PopupActions span').text());
    window.startProgress = function (immediate) {
        $('#loadingProgress .progress-bar').css('width', '0%');

        $('#loadingProgress').fadeIn();

        setTimeout(function () {
            $('#loadingProgress .progress-bar').css('width', '100%');
        }, 1);


        if (window.StopProgressControlTimer) {
            window.clearTimeout(window.StopProgressControlTimer);
            window.StopProgressControlTimer = undefined;
        }

        if (!isCancelProgress) {
            window.ProgressControlTimer = window.setTimeout("$('#pleaseWaitDialog').removeClass('hidden');", 0);
            document.onstop = stopProgress;
            window.onblur = stopProgress;
            setTimeout(function () {
                if (window.stopProgress)
                    window.stopProgress();
            }, 7000);
        }
        isCancelProgress = false;
    };
    window.stopProgress = function () {
        $('#loadingProgress').fadeOut();
        $('#pleaseWaitDialog').addClass('hidden');
        window.clearTimeout(window.ProgressControlTimer);
        document.body.onscroll = null;
        document.body.onresize = null;
    };
    $('body').keyup(function (e) {
        if (e.which == 13) {
            RaiseXafCallback(globalCallbackControl, "Logon$PopupActions_Callback", "Action=Logon", "", false);
        }
    });
</script>