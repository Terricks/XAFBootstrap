<%@ Control Language="C#" CodeBehind="LogonTemplate.ascx.cs" ClassName="LogonTemplate" Inherits="XAF_Bootstrap.Templates.LogonTemplate"%>
<%@ Register Assembly="XAF Bootstrap" Namespace="XAF_Bootstrap.Controls" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>

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
    .XafVCap-Second, .Layout .StaticText {
        color: #111;
    }

    div.Header table {
        margin: 0px 10px 0px 10px;        
    }

    table {
        background: transparent;
    }

    ::-webkit-input-placeholder { /* WebKit, Blink, Edge */
        color: rgba(0, 0, 0, 0.5) !important;
        font-style: italic;
    }
    :-moz-placeholder { /* Mozilla Firefox 4 to 18 */
       color: rgba(0, 0, 0, 0.5) !important;
       opacity:  1;
       font-style: italic;
    }
    ::-moz-placeholder { /* Mozilla Firefox 19+ */
       color: rgba(0, 0, 0, 0.5) !important;
       opacity:  1;
       font-style: italic;
    }
    :-ms-input-placeholder { /* Internet Explorer 10-11 */
       color: rgba(0, 0, 0, 0.5) !important;
       font-style: italic;
    }

    .CardGroupContent {
        background: none;
        padding: 0px;
    }

    .CardGroupBase {
        border: 0px solid rgba(0, 0, 0, 0);
    }
</style>

<div id="LogonContent">
    <div class="progress" style="margin: 0;height: 5px; display: none" id="loadingProgress">
        <div class="progress-bar progress-bar-info progress-bar-striped active" role="progressbar" aria-valuenow="100" aria-valuemin="0" aria-valuemax="100" style="width: 100%"></div>
    </div>
    <div id="pleaseWaitDialog" class="hidden">
        <div class="modal-backdrop fade in" style="position: fixed; bottom: 0px; left: 0px; right: 0px; top: 0px; z-index: 2000; opacity:0"></div>    
    </div>
    <div class="container valign">
        <div class="row ">
            <div class="col-sm-3"></div>
            <div class="col-sm-6">
                <div class="well" style="border: 0; background: rgba(0, 0, 0, 0.11); padding-bottom: 50px;">
                    <cc3:XafUpdatePanel ID="UPPopupWindowControl" runat="server">
                        <cc4:XafPopupWindowControl runat="server" ID="PopupWindowControl" />
                    </cc3:XafUpdatePanel>
                        <div id='logoImgDiv' style='text-align:center'></div>
                    <cc3:XafUpdatePanel ID="UPHeader" runat="server">
                        <div class="Header">
                            <table cellpadding="0" cellspacing="0" border="0">
                                <tr>
                                    <td class="ViewImage">
                                        <cc4:ViewImageControl ID="viewImageControl" runat="server" />
                                    </td>
                                    <td class="ViewCaption">
                                        <h3 style="color: #111">
                                            <cc4:ViewCaptionControl ID="viewCaptionControl" DetailViewCaptionMode="ViewCaption"
                                                runat="server" />
                                        </h3>
                                    </td>
                                </tr>
                            </table>
                        </div>                        
                    </cc3:XafUpdatePanel>

                
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

<script type="text/ecmascript">
    $('#logoImgDiv').html('<%= String.Concat(XAF_Bootstrap.BusinessObjects.XAFBootstrapConfiguration.Instance.LogoImageUrl) != "" ? String.Format(@"<img style=""max-width:250px"" src=""{0}"">",XAF_Bootstrap.BusinessObjects.XAFBootstrapConfiguration.Instance.LogoImageUrl) : "" %>');
    $('body').removeClass("Dialog");    
    $('body').css({ "background": "url(<%= (String.Concat(XAF_Bootstrap.BusinessObjects.XAFBootstrapConfiguration.Instance.LogonPageBackgroundUrl) != "" ? XAF_Bootstrap.BusinessObjects.XAFBootstrapConfiguration.Instance.LogonPageBackgroundUrl : "bootstrap_images/logon_bg.jpg") %>) no-repeat center center fixed" });
    $('body').css({ height: $(window).height() });
    $('#LogonContent').css({ height: $(window).height() });
</script>

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