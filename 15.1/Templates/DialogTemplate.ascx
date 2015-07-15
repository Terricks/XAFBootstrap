<%@ Control Language="C#" CodeBehind="DialogTemplate.ascx.cs" ClassName="DialogTemplate" Inherits="XAF_Bootstrap.Templates.DialogTemplate"%>
<%@ Register Assembly="XAF Bootstrap" Namespace="XAF_Bootstrap.Controls" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v15.1" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.1" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.1" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="tc" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.1" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.1" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>

    <cc3:XafUpdatePanel ID="UPPopupWindowControl" runat="server">
        <cc4:XafPopupWindowControl runat="server" ID="PopupWindowControl" />
    </cc3:XafUpdatePanel>    

    <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal" onclick="closeThisModal()" aria-label="Close"><span aria-hidden="true" style="color: #000">&times;</span></button>
        <h4 class="modal-title">
            <cc3:XafUpdatePanel ID="UPVH" runat="server">            
                <div class="row no-margin">                
                        <cc4:ViewImageControl ID="VIC" runat="server" Control-UseLargeImage="false"  />
                        <cc4:ViewCaptionControl ID="VCC" runat="server" DetailViewCaptionMode="ViewAndObjectCaption" />
                </div>
            </cc3:XafUpdatePanel>
        </h4>
    </div>
        
    <div class="modal-body">
        <cc3:XafUpdatePanel ID="UPSAC" runat="server">
            <div style="display: none">                                                
                <cc2:ActionContainerHolder ID="OCC" runat="server" ContainerStyle="Buttons"
                    Orientation="Horizontal" Categories="ObjectsCreation" style="float: left" />
                                                
                <cc2:ActionContainerHolder ID="SAC" runat="server" Categories="Search;FullTextSearch"
                    CssClass="HContainer" Orientation="Horizontal" ContainerStyle="Buttons" />
            </div>                                                

            <div class="row">
                <div class="col-sm-6">
                    <dx:ASPxLabel ID="LeftActions" runat="server"></dx:ASPxLabel>
                </div>
                <div class="col-sm-6">
                    <dx:ASPxLabel ID="RightActions" runat="server"></dx:ASPxLabel>
                </div>
            </div>
        </cc3:XafUpdatePanel>                
    
        <div class="Dialog" id="DialogContent">        
            <cc3:XafUpdatePanel ID="UPEI" runat="server">
                <cc1:XafBootstrapErrorInfoControl ID="ErrorInfo" Style="margin: 10px 0px 10px 0px" runat="server" />
            </cc3:XafUpdatePanel>
                                
            <cc3:XafUpdatePanel ID="UPVSC" runat="server">
                <cc1:XafBootstrapViewSiteControl ID="VSC" runat="server" />
            </cc3:XafUpdatePanel>        
        </div>
    </div>

    <div class="modal-footer" id="DialogFooter">    
        <cc3:XafUpdatePanel ID="UPPAC" runat="server">
            <span style="display: none">
                <cc2:ActionContainerHolder runat="server" ID="PAC" ContainerStyle="Buttons"
                    Orientation="Horizontal" Categories="PopupActions;Diagnostic">
                    <menu width="100%" itemautowidth="False" horizontalalign="Right" />
                </cc2:ActionContainerHolder>
            </span>
            <dx:ASPxLabel ID="Actions" runat="server"></dx:ASPxLabel>
        </cc3:XafUpdatePanel>    
    </div>

<script>
    
    $(document).ready(function () {
        $("#content").css('margin-top', $('#navbar').height());
        $(window).resize(function () {
            $("#content").css('margin-top', $('#navbar').height());
        });

        window.initializePopupWindow = function (callbackControlID, updatePanelID, callBackFuncName, forcePostBack, targetControlId, wrapperPanelId) {                        
            dialog.callbackFunc = callBackFuncName;
            dialog.forcePostBack = forcePostBack;
            dialog.targetControlId = targetControlId;
            var topWindow = window.top;
            var callbackControl = window[callbackControlID];
            var popupControlId = callbackControl.cpControlID.toString();
            var searchItem = "dxo.contentUrl='";
            var contentUrl = callbackControl.cpMarkup.substring(callbackControl.cpMarkup.indexOf(searchItem) + searchItem.length);            
            contentUrl = contentUrl.substring(0, contentUrl.indexOf("'"));
            window.createXafBootstrapPopup(contentUrl, window);
        }
    });
</script>

<script>
    $(document)
        .on('shown.bs.modal', '.modal.in', function (event) {
            setModalsAndBackdropsOrder();
        })
        .on('hidden.bs.modal', '.modal', function (event) {
            setModalsAndBackdropsOrder();
        });

    function setModalsAndBackdropsOrder() {
        var modalZIndex = 1040;
        $('.modal.in').each(function (index) {
            var $modal = $(this);
            modalZIndex++;
            $modal.css('zIndex', modalZIndex);
            $modal.next('.modal-backdrop.in').addClass('hidden').css('zIndex', modalZIndex - 1);
        });
        $('.modal.in:visible:last').focus().next('.modal-backdrop.in').removeClass('hidden');
    }
</script>