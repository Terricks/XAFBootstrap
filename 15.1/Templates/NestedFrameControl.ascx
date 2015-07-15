<%@ Control Language="C#" CodeBehind="NestedFrameControl.ascx.cs" ClassName="NestedFrameControl" Inherits="XAF_Bootstrap.Templates.NestedFrameControl"%>
<%@ Register Assembly="XAF Bootstrap" Namespace="XAF_Bootstrap.Controls" TagPrefix="cc1" %>
<%@ Register Assembly="DevExpress.Web.v15.1" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.1" Namespace="DevExpress.ExpressApp.Web.Templates.ActionContainers"
    TagPrefix="cc2" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.1" Namespace="DevExpress.ExpressApp.Web.Controls"
    TagPrefix="cc4" %>
<%@ Register Assembly="DevExpress.ExpressApp.Web.v15.1" Namespace="DevExpress.ExpressApp.Web.Templates"
    TagPrefix="cc3" %>

<div class="NestedFrame">    
    <cc3:XafUpdatePanel ID="UPToolBar" CssClass="ToolBarUpdatePanel" runat="server">
        <div class='<%= ToolBarHasActions() ? "panel panel-default" : "" %>'>
            <cc1:XbActionContainerHolder runat="server" ID="ToolBar"  CssClass="panel-heading" ContentBefore="<div class='btn-group'>" ContentAfter="</div>"
                Categories="ObjectsCreation;Link;Edit;RecordEdit;View;Reports;Export;Diagnostic;Filters"
                Tag="button" ItemClass="btn btn-primary btn-sm" LeftDirection="True" />            
            <cc4:ViewSiteControl ID="viewSiteControl" runat="server" Control-CssClass="NestedFrameViewSite"/>            
        </div>
    </cc3:XafUpdatePanel>
</div>
