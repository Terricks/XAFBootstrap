<!-- Search and others --><%@ Control Language="C#" CodeBehind="ContentTemplate.ascx.cs" ClassName="ContentTemplate" Inherits="XAF_Bootstrap.Templates.ContentTemplate" %>
<%@ Register Assembly="XAF Bootstrap" Namespace="XAF_Bootstrap.Controls" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v15.2" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.v15.2" Namespace="DevExpress.Web"
    TagPrefix="dxrp" %>
<%@ Register Assembly="DevExpress.Web.v15.2" Namespace="DevExpress.Web"
    TagPrefix="dxge" %>
<%@ Register Assembly="DevExpress.Web.v15.2" Namespace="DevExpress.Web"
    TagPrefix="cb1" %>
<%@ Register Assembly="DevExpress.Web.v15.2" Namespace="DevExpress.Web"
    TagPrefix="cp1" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.2" Namespace="DevExpress.ExpressApp.Web.Templates.Controls"
    TagPrefix="tc" %>

<div>
    <cc3:XafUpdatePanel ID="UPPopupWindowControl" runat="server">
        <cc4:XafPopupWindowControl runat="server" ID="PopupWindowControl" />
    </cc3:XafUpdatePanel>

    <dx:ASPxGlobalEvents ID="GE" ClientInstanceName="GE" ClientSideEvents-EndCallback=""
        runat="server" />    

    <!-- xb-mega-menu -->
    <dx:ASPxLabel runat="server" ID="xbMegaMenu" EncodeHtml="false"></dx:ASPxLabel>
    
    <div class="content well">
        <div class="container well" id="content">            
            <div class="view-header row no-margin">
                
                <span class="col-md-6 no-padding" id="leftColumnActions">
                    <!-- HEADER -->
                    <cc3:XafUpdatePanel ID="UPVH" runat="server">
                        <span class="h4">
                            <span class="pull-left view-header-image">
                                <!-- Image -->
                                <cc4:ViewImageControl ID="VIC" runat="server" />
                            </span>
                            <span class="pull-left view-header-text">
                                <!-- Caption -->
                                <cc4:ViewCaptionControl ID="VCC" runat="server" />                                
                            </span>
                        </span>
                        
                        <!-- ViewRecordActions -->
                        <cc3:XafUpdatePanel ID="VRUP" runat="server" EnableTheming="False" UpdateAlways="True">
                            <cc1:XbActionContainerHolder runat="server" ID="VRAH" 
                                Categories="ObjectsCreation;Edit;RecordEdit;View;Unspecified;" EnableViewState="False" 
                                Tag="a" ItemClass="view-action" ContainerClass="link-actions h6" />                                
                        </cc3:XafUpdatePanel>                        

                        <span>
                            <!-- Navigation to rows buttons -->
                            <div style="display: none">
                                <cc1:XbActionContainerHolder runat="server" ID="RNC" Categories="RecordsNavigation"
                                    Tag="a" ItemClass="view-action" ContainerClass="link-actions h6"
                                    />
                            </div>

                        </span>

                    </cc3:XafUpdatePanel>
                </span>

                <span class="col-md-6 no-padding" id="rightColumnActions">
                    <!-- Security -->
                    <span class="pull-right" style="display: none">
                        <cc3:XafUpdatePanel ID="UPSAC" runat="server">
                            <span>
                                <cc1:XbActionContainerHolder runat="server" ID="SAC" Categories="Security"
                                    Tag="a" ItemClass="view-action" ContainerClass="link-actions h6" />
                            </span>                                       
                        </cc3:XafUpdatePanel>
                    </span>

                    <!-- Search and others -->
                    <span class="pull-right">
                        <cc3:XafUpdatePanel ID="UPSHC" runat="server">                            
                            <cc1:XbActionContainerHolder ID="SHC" runat="server" Categories="RootObjectsCreation;Search;FullTextSearch;Export;"
                                Tag="button" ItemClass="btn btn-primary btn-sm view-action" LeftDirection="False" />
                        </cc3:XafUpdatePanel>                        
                    </span>
                    
                    <div class="col-md-12 nopaddings">
                        <!-- Object Actions -->

                        <span class="pull-right">
                            <cc3:XafUpdatePanel ID="UPTB" runat="server" EnableTheming="False" UpdateAlways="True">                                
                                <cc1:XbActionContainerHolder runat="server" ID="TB" Categories="Reports;ListEditorInplace;Filters;" EnableViewState="False"
                                    Tag="button" ItemClass="btn btn-primary btn-sm" LeftDirection="False" />                                
                            </cc3:XafUpdatePanel>
                        </span>
                    </div>
                </span>
            </div>            


            <!-- ACTIONS -->
            <div class="container-actions row no-margin">
                <span>
                    <span class="pull-right container row top-actions">
                        <!-- Top Actions -->
                        <cc3:XafUpdatePanel ID="UPEMA" runat="server">                            
                            <cc1:XbActionContainerHolder runat="server" ID="EMA" Categories="Save;UndoRedo"
                                Tag="button" ItemClass="btn btn-info btn-sm" LeftDirection="False" />                                                            
                        </cc3:XafUpdatePanel>
                    </span>
                    <span>
                        <!-- View -->
                        <cc3:XafUpdatePanel ID="UPEI" runat="server">
                            <cc1:XafBootstrapErrorInfoControl ID="ErrorInfo" Style="margin: 10px 0px 10px 0px" runat="server" />
                        </cc3:XafUpdatePanel>
                        <br />
                        <cc3:XafUpdatePanel ID="UPVSC" runat="server">
                            <!-- View -->
                            <cc4:ViewSiteControl ID="VSC" runat="server" />

                            <!-- Bottom Actions -->
                            <span class="pull-right bottom-actions">
                                <cc1:XbActionContainerHolder runat="server" ID="EditModeActions2" Categories="Save;UndoRedo"
                                    Tag="button" ItemClass="btn btn-info btn-sm" LeftDirection="False" />                                 
                            </span>                            
                        </cc3:XafUpdatePanel>

                    </span>
                    <span>
                        <cc3:XafUpdatePanel ID="UPQC" runat="server">
                            <cc2:QuickAccessNavigationActionContainer CssClass="NavigationLinks" ID="QC" runat="server"
                                ContainerId="ViewsNavigation" PaintStyle="Caption" ShowSeparators="True" />
                        </cc3:XafUpdatePanel>
                    </span>
                </span>

            </div>
        </div>
    </div>

    <div>
        <cc3:XafUpdatePanel ID="UPIMP" runat="server">
            <asp:Literal ID="InfoMessagesPanel" runat="server" Text="" Visible="False"></asp:Literal>
            
            <!-- Fixed navbar -->
            <dx:ASPxLabel runat="server" ID="TopMenu" EncodeHtml="false"></dx:ASPxLabel>
        </cc3:XafUpdatePanel>
    </div>    
</div>

<div class="container xaf-bootstrap-page-footer">    
    <table cellpadding="0" cellspacing="0" border="0" width="100%">
        <tr>
            <td align="left">
                <div class="FooterCopyright">
                    <cc3:XafUpdatePanel ID="XafUpdatePanel2" runat="server">
                        <span>
                            <cc1:XbActionContainerHolder runat="server" ID="FooterActions" Categories="Footer"
                                Tag="a" ItemClass="view-action" ContainerClass="link-actions h6" />
                        </span>                                       
                    </cc3:XafUpdatePanel>
                    <cc4:AboutInfoControl ID="AIC" runat="server">Copyright text</cc4:AboutInfoControl>
                </div>
            </td>
        </tr>
    </table>
    <br />
    <br />
</div>

<cc3:XafUpdatePanel ID="XafUpdatePanel1" runat="server">
    <dx:ASPxLabel ID="Scripts" runat="server"></dx:ASPxLabel>
</cc3:XafUpdatePanel>

<div id="pleaseWaitDialog" class="hidden">
    <div class="modal-backdrop fade in" style="position: fixed; bottom: 0px; left: 0px; right: 0px; top: 0px; z-index: 2000; opacity:0"></div>    
</div>

<script type="text/javascript">
    window.modalsStartZIndex = 1050;
    function ShowXafMessage(caption, confirmationMessage, callback, buttonOKCaption, isCancelButtonNeeded, isOKButtonNeeded, isCanCloseTheModal, modalClass) {
        var modals = $('.modal.in');
        var OKButton = '';
        if (isOKButtonNeeded + "" != 'false')
            if (buttonOKCaption == '' || buttonOKCaption == undefined)
                OKButton = '<button type="button" class="btn btn-primary" data-dismiss="modal" id="XafBootstrapConfirmButton"><%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"DialogButtons", "OK") %></button>';
            else
                OKButton = '<button type="button" class="btn btn-primary" data-dismiss="modal" id="XafBootstrapConfirmButton">'+buttonOKCaption+'</button>';
        var cancelButton = '';
        if (isCancelButtonNeeded + "" != 'false')
            cancelButton = '<button type="button" class="btn btn-default" data-dismiss="modal"><%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"DialogButtons", "Cancel") %></button>';
        var showFooter = '<div class="modal-footer">' +
                            OKButton +
                            cancelButton +
                        '</div>';
        if (cancelButton == '' && OKButton == '')
            showFooter = '';
        var canCloseModal = '';
        var canCloseAttributes = 'data-backdrop="static" data-keyboard="false"';
        if (isCanCloseTheModal + "" != 'false') {
            canCloseModal = '<button type="button" class="close" data-dismiss="modal" aria-label="Close">&times;<span aria-hidden="true"></span></button>';
            canCloseAttributes = '';
        }
        var modal = $(
            '<div class="modal ' + (modalClass ? modalClass : "") + ' fade is-temporary" tabindex="-1" role="dialog" aria-hidden="true"' + canCloseAttributes + ' style="z-index:' + (window.modalsStartZIndex + modals.length) + '">' +
                '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                        '<div class="modal-header">' +
                            canCloseModal +                             
                            '<h4 class="modal-title"></h4>' +
                        '</div>' +
                        '<div class="modal-body">' +                
                        '</div>' + showFooter +
                    '</div>' +
                '</div>' +
            '</div>'
            );
        modal.find('h4.modal-title').text(caption);        
        modal.find('div.modal-body').append(confirmationMessage);
        modal.find('#XafBootstrapConfirmButton').on('click', callback);        
        modal.on('show.bs.modal', function (e) {
            $('body').css({ overflow: 'hidden' });
        });
        modal.modal();

        return modal;
    }
</script>

<script>    
    function checkWindowScrolls(modalsCount) {
        var modals = $('.modal.in:visible');
        if (modals.length > modalsCount) {
            $('body').css({ overflow: 'hidden' });
        }
        else {
            $('body').css({ overflow: 'auto' });            
        };
    };

    $(document).ready(function () {
        $("#content").css('margin-top', $('#navbar').height());
        
        window.calcHeaderOffset = function () {            
            $("#content").css('margin-top', $('#navbar').height());
        };

        $(window).resize(window.calcHeaderOffset);
        setTimeout(window.calcHeaderOffset, 750);        
        
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
            window.createXafBootstrapPopup(contentUrl);            
        }

        window.createXafBootstrapPopup = function (url, sender) {
            if (!(sender))
                sender = window;

            var modals = $('.modal.in');
            var modal = $("<div class='modal fade is-temporary' style='z-index:" + (window.modalsStartZIndex + modals.length) + "'><div class='modal-dialog modal-lg'><div class='modal-content'>"
                    + "<div class='modal-body' style='height: 200px'><div class='progress' style='margin: 0;height: 14px; position: absolute; width: 20%; top: 50%; left: 40%'>"
                        + "<div class='progress-bar progress-bar-info progress-bar-striped active' role='progressbar' aria-valuenow='100' aria-valuemin='0' aria-valuemax='100' style='width: 100%'></div>"
                    + "</div></div>"
                + "</div></div></div>")
            var iframe = $("<iframe scrolling='no' frameborder='0' src='" + url + "' style='width: 100%; height: 254px; overflow: auto; vertical-align: text-bottom;'></iframe>");
                        
            startProgress();            
            
            iframe.on("load", function () {
                var iframeDoc = iframe[0].contentWindow;                

                iframeDoc.createXafBootstrapPopup = window.createXafBootstrapPopup;
                iframeDoc.ShowXafMessage = window.ShowXafMessage;
                iframeDoc.RaiseXafCallback = function (callbackControl, handlerId, parameters, confirmation, usePostBack) {
                    window.RaiseXafCallback(callbackControl, handlerId, parameters, confirmation, usePostBack, iframeDoc);
                };

                iframeDoc.startProgress = startProgress;
                iframeDoc.stopProgress = stopProgress;                

                iframeDoc.closeThisModal = function (force) {
                    modal.attr('forcedClose', force);
                    modal.modal('hide');
                };
                iframeDoc.fullscreenThisModal = function () {
                    if (modal.find('.modal-dialog').attr("style"))
                        $('.modal.in .modal-dialog').removeAttr('style');
                    else
                        $('.modal.in .modal-dialog').attr('style', 'width: 100%');
                };

                iframeDoc.isChildFrame = true;
                if (sender.isChildFrame) {
                    iframeDoc.parentFrameWindow = sender;
                }
                iframeDoc.calculateWindowSize = function () {
                    var frameWindow = iframe[0].contentWindow;                    
                    if (frameWindow != null) {
                        var newHeight = frameWindow.document.body.offsetHeight;
                        if (iframe.height() != newHeight) {
                            iframe.animate({                                
                                height: newHeight + "px"
                            }, 100, function () {
                                $(window).trigger('resize.modal');
                            });                                                        
                        }
                        newHeight = iframe.height();
                    }
                }

                var calcFunc = function () {
                    for(i = 250; i < 7750; i += 750)
                        setTimeout(iframeDoc.calculateWindowSize, i);
                };

                iframeDoc.globalCallbackControl.EndCallback.AddHandler(calcFunc);                
                
                $(iframeDoc).click(function () {
                    calcFunc();
                });
                modal.on('hide.bs.modal', function (e) {
                    if (iframeDoc.DataChanged) {                        
                        e.preventDefault();                        
                        checkDataChanged(iframeDoc, function () {                            
                            iframeDoc.closeThisModal();
                        });
                    } else {
                        if (!iframeDoc.isModalClosing && !modal.attr('forcedClose')) {
                            e.preventDefault();
                            iframeDoc.closeThisModalBefore();
                        }
                    }
                });
                calcFunc();
                modal.on('hidden.bs.modal', function (e) {
                    if (sender.isChildFrame) {
                        iframeDoc.globalCallbackControl.EndCallback.RemoveHandler(calcFunc);
                    }
                    sender.RaiseXafCallback(sender.globalCallbackControl, "", "XafParentWindowRefresh", "", false);
                });
                
                stopProgress();
            });            
            modal.on('show.bs.modal', function (e) {                
                $('body').css({ overflow: 'hidden' });
                setTimeout(function () {
                    modal.find(".modal-content").html("");
                    modal.find(".modal-content").append(iframe);
                }, 100);
            });
            modal.modal();
        };

        window.startProgress = function (immediate) {            
            $('#loadingProgress .progress-bar').css('width', '0%');

            $('#loadingProgress').fadeIn();

            setTimeout(function () {
                $('#loadingProgress .progress-bar').css('width', '100%');
            }, 1);

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

        window.RaiseXafCallbackCached = window.RaiseXafCallback;
        window.RaiseXafCallback = function RaiseXafCallback(callbackControl, handlerId, parameters, confirmation, usePostBack, sender) {            
            if (!(sender))
                sender = window;            
            var isRaised = false;
            var raiseFunc = function () {
                startProgress();
                var parameter = handlerId + ':' + parameters;
                if (usePostBack) {
                    callbackControl.SendPostBack(parameter);
                } else {
                    sender.startProgress();
                    callbackControl.PerformCallback(parameter);
                }
                isRaised = true;
            };

            if (confirmation != "") {
                sender.ShowXafMessage("<%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Dialogs", "ConfirmAction") %>", confirmation, raiseFunc, '', '');
            } else {
                raiseFunc();
            };

            sender.checkWindowScrolls(0);

            return isRaised;
        }        
    });
</script>

<script>
    $(document).ready(function () {
        window.onbeforeunload = function (e) {            
            if (window.DataChanged) return '<%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Dialogs", "DiscardChangesText") %>';
        };
        if (window.CachedRaiseXafCallback == null) {
            {
                window.CachedRaiseXafCallback = RaiseXafCallback;
                window.DataChanged = false;
                RaiseXafCallback = function (callbackControl, handlerId, parameters, confirmation, usePostBack, sender) {
                    if (!(sender))
                        sender = window;

                    //It works with bugs and not localized now.
                    sender.xaf.ConfirmUnsavedChangedController.DropModified();

                    sender.DataChangedFunctionWithoutCommit = function () {
                        sender.DataChanged = false;
                        window.CachedRaiseXafCallback(callbackControl, handlerId, parameters, confirmation, usePostBack, sender);
                    }
                    if (handlerId == 'MenuItemClickControllerCallback' || (handlerId.indexOf('$SAC_Callback') > -1) || parameters == 'Action=Refresh')
                        checkDataChanged(sender, sender.DataChangedFunctionWithoutCommit);
                    else
                    {
                        sender.DataChangedFunctionWithoutCommit();
                    }
                }
            }
        }
    });

    window.ProcessCallbackResult = function (callbackResult) {        
        if (callbackResult.cpPageTitle) {
            document.title = callbackResult.cpPageTitle;
        }
        ProcessMarkup(callbackResult);
        if (callbackResult.cpCurrentView && callbackResult.cpCurrentView != '') {
            UpdateDocumentLocation(callbackResult.cpCurrentView);
        }
        stopProgress();
    }

    function checkDataChanged(sender, callback) {
        if (!(sender))
            sender = window;
        if (sender.DataChanged == true)
        {
            var modals = $('.modal.in');            

            var dialog =
            $('<div class="modal fade is-temporary" id="confirm-delete" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="z-index:' + (window.modalsStartZIndex + modals.length) + '">'
                + '<div class="modal-dialog">'
                    + '<div class="modal-content">'
                        + '<div class="modal-header"><%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Dialogs", "DiscardChangesCaption") %>'
                        + '</div>'
                        + '<div class="modal-body"><%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Dialogs", "DiscardChangesText") %>'
                        + '</div>'
                        + '<div class="modal-footer">'
                            + '<button type="button" class="btn btn-default" data-dismiss="modal"><%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"DialogButtons", "Cancel") %></button>'
                            + '<button id="DoWithoutCommitButton"type="button" class="btn btn-danger danger" data-dismiss="modal"><%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"DialogButtons", "Ignore") %></button>'
                        + '</div>'
                    + '</div>'
                + '</div>'
            + '</div>');
            dialog.on('show.bs.modal', function (e) {
                $('body').css({ overflow: 'hidden' });
            });
            dialog.modal({ show: true });
            if (callback)
            {
                dialog.find("#DoWithoutCommitButton").click(function () {sender.DataChanged = false; callback(); });
            }
        }
        else
        {
            sender.DataChanged = false;
            callback();
        }
    }

    function CalcActionsColumns() {
        var leftCount = $('#leftColumnActions .view-action:visible').length + $('#leftColumnActions .parametrized-action:visible').length * 2;
        var rightCount = $('#rightColumnActions .view-action:visible').length + $('#rightColumnActions .parametrized-action:visible').length * 2;

        var left = 6;
        var right = 6;

        if (leftCount == 0) {
            left = 2;
            right = 10;

            if (rightCount < 4) {
                left = 6;
                right = 6;
            }
        }
        else if (rightCount == 0) {
            left = 12;
            right = 0;
        }
        else {
            var maxRelation = Math.max(leftCount, rightCount) / Math.min(leftCount, rightCount);

            if (maxRelation >= 2) {
                left = leftCount > rightCount ? 7 : 5;
                right = rightCount > leftCount ? 7 : 5;
            }

            if (maxRelation >= 3) {
                left = leftCount > rightCount ? 8 : 4;
                right = rightCount > leftCount ? 8 : 4;
            }

            if (maxRelation >= 4) {
                left = leftCount > rightCount ? 9 : 3;
                right = rightCount > leftCount ? 9 : 3;
            }

            if (maxRelation >= 5) {
                left = leftCount > rightCount ? 10 : 2;
                right = rightCount > leftCount ? 10 : 2;
            }
        }

        $('#leftColumnActions').attr('class', 'no-padding col-md-' + left + ' col-sm-' + left); 
        $('#rightColumnActions').attr('class', 'no-padding col-md-' + right + ' col-sm-' + right);
    };
</script>

<script>
    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
              .toString(16)
              .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
          s4() + '-' + s4() + s4() + s4();
    }

    $(document)
        .on('show.bs.modal', '.modal', function (event) {            
            $('body').css({ overflow: 'hidden' });
        })
        .on('shown.bs.modal', '.modal.in', function (event) {
            var $backdrop = $(this).find('.modal-backdrop');
            var id = guid();
            $backdrop.attr('id', id);
            $(this).attr('backdrop-id', id);
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
            $($(this).attr('backdrop-id')).fadeOut();
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
                    if (!contentClicked && $modal.attr('data-backdrop') != 'static') {                        
                        //$modal.modal('hide');
                    }
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
            $('.modal').modal('hide');
            ShowXafMessage("<%= XAF_Bootstrap.Templates.Helpers.GetLocalizedText(@"XAF Bootstrap\Dialogs", "ConfirmAction") %>", message, "", "", "");
            return { isHandled: false };
        };

        $('.xb-mega-menu.mobile li a').click(function (event) {
            if ($(this).parent().hasClass("hover")) {
                $(this).parent().removeClass("hover");
                event.stopPropagation();
            } else {
                $('.xb-mega-menu.mobile li.hover').removeClass("hover");
            }
            if ($(this).attr("islink") == "true")
                event.stopPropagation();
        });

        $('.xb-mega-menu.mobile li').click(function (event) {
            $(this).addClass("hover");
        });
    });

    function XafBootstrapHideHeader(value) {
        if (value != window["XafBootstrapHideHeaderCurrentValue"])
            if (value) {
                window.lastContentPaddingMargin = { margin: $('.content').css('margin-top'), padding: $('.content').css('padding-top') };
                $('.view-header').hide();
                $('.top-actions').hide();
                $('.content').css({ 'margin-top': '-10px' });
            } else {
                if (window.lastContentPaddingMargin) {
                    $('.content').css({ 'margin-top': window.lastContentPaddingMargin.margin, 'padding-top': window.lastContentPaddingMargin.padding });
                    delete window.lastContentPaddingMargin;
                }
                $('.view-header').show();
                $('.top-actions').show();
            }
        window["XafBootstrapHideHeaderCurrentValue"] = value;
    }

    function XafBootstrapHideFooter(value) {
        if (value != window["XafBootstrapHideFooterCurrentValue"])
            if (value) {
                $('.xaf-bootstrap-page-footer').hide();
            } else {
                $('.xaf-bootstrap-page-footer').show();
            }
        window["XafBootstrapHideFooterCurrentValue"] = value;
    }

    function XafBootstrapFullScreen(value) {        
        if (value != window["XafBootstrapFullScreenCurrentValue"])
            if (value) {
                window.lastContentPaddingMargin = { margin: $('.content').css('margin-top'), padding: $('.content').css('padding-top') };
                window.lastContentPaddingClass = $('#content').attr('class');
                $('.content').css({ 'padding-top': '8px' });
                $('#content').attr('class', 'row no-margin');
            } else {
                if (window.lastContentPaddingClass) {
                    $('#content').attr('class', window.lastContentPaddingClass);
                    delete window.lastContentPaddingClass;
                }
                if (window.lastContentPaddingMargin) {
                    $('.content').css({ 'margin-top': window.lastContentPaddingMargin.margin, 'padding-top': window.lastContentPaddingMargin.padding });
                    delete window.lastContentPaddingMargin;
                }                                
            }         
        window["XafBootstrapFullScreenCurrentValue"] = value;
    }
</script>

<div id="LPcell" style="display: none"></div>
<div id="separatorCell" style="display: none"></div>
<div id="separatorImage" style="display: none"></div>
<div id="MT" style="display: none" ></div>
<div id="MRC" style="display: none" ></div>