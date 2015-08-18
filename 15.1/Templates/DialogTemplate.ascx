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
        <button type="button" class="close" data-dismiss="modal" onclick="closeThisModal()" aria-label="Close"><span aria-hidden="true" class="text-primary">&times;</span> </button>
        <button type="button" class="close" data-dismiss="modal" onclick="fullscreenThisModal()" aria-label="Maximize"><span class="text-primary glyphicon glyphicon-fullscreen" style="font-size: 60%; margin-top: 5px; margin-right: 5px"></span> </button>
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
            <div class="row">
                <div class="col-sm-6">
                    <cc1:XbActionContainerHolder ID="OCC" runat="server" Categories="ObjectsCreation" />
                </div>
                <div class="col-sm-6">
                    <cc1:XbActionContainerHolder ID="SAC" runat="server" Categories="Search;FullTextSearch" LeftDirection="false" />
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
            <cc1:XbActionContainerHolder runat="server" ID="PAC" Categories="PopupActions;Diagnostic" LeftDirection="false">                    
            </cc1:XbActionContainerHolder>            
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

    function checkWindowScrolls(modalsCount) {
        var modals = $('.modal.in:visible');
        if (modals.length > modalsCount) {
            $('body').css({ overflow: 'hidden' });
        }
        else {
            $('body').css({ overflow: 'auto' });            
        };
    };
</script>

<script>
    $(document)
        .on('shown.bs.modal', '.modal.in', function (event) {
            setModalsAndBackdropsOrder();
            $('body').css({ overflow: 'hidden' });
        })
        .on('hide.bs.modal', '.modal.in', function (event) {
            window.checkWindowScrolls(1);
        })
        .on('hidden.bs.modal', '.modal', function (event) {
            if ($(this).hasClass('is-temporary')) {
                $(this).remove();
            };
        });

    function setModalsAndBackdropsOrder() {
        var modalZIndex = modalsStartZIndex;
        var modals = $('.modal.in');
        modals.each(function (index) {
            var $modal = $(this);
            modalZIndex++;
            $modal.css({ overflow: 'auto', zIndex: modalZIndex });
            $backdrop = $modal.find('.modal-backdrop.in');
            if ($backdrop.length > 0) {
                $backdrop.css({ zIndex: modalZIndex, width: '100%', height: '100%', position: 'fixed' });
                $backdrop.insertBefore($modal);                
            }

            if (!$modal.hasClass("managed")) {
                var contentClicked = false;
                $modal.find('.modal-content').click(function (e) {                    
                    contentClicked = true;
                });
                $modal.click(function (e) {
                    if (!contentClicked)
                        $modal.modal('hide');
                    contentClicked = false;
                });
                $modal.addClass("managed");
            }
        });        
        $('.modal.in:visible:last').focus().next('.modal-backdrop.in').removeClass('hidden');
    }

    $(document).ready(function () {
        globalCallbackControl.RaiseCallbackError = function (message) {
            stopProgress();
            ShowXafMessage("<%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Dialogs", "ConfirmAction") %>", message, "", "", "");
            return { isHandled: false };
        };
    });
</script>