namespace XAF_Bootstrap.Controllers.XafBootstrapConfiguration
{
    partial class XafBootstrapConfigurationAction
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.XafBootstrapConfigurationActionItem = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // XafBootstrapConfigurationActionItem
            // 
            this.XafBootstrapConfigurationActionItem.Caption = "Xaf Bootstrap Configuration";
            this.XafBootstrapConfigurationActionItem.Category = "Security";
            this.XafBootstrapConfigurationActionItem.ConfirmationMessage = null;
            this.XafBootstrapConfigurationActionItem.Id = "XafBootstrapConfigurationActionItem";
            this.XafBootstrapConfigurationActionItem.ToolTip = null;
            this.XafBootstrapConfigurationActionItem.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.XafBootstrapConfigurationActionItem_Execute);
            // 
            // XafBootstrapConfigurationAction
            // 
            this.Actions.Add(this.XafBootstrapConfigurationActionItem);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction XafBootstrapConfigurationActionItem;
    }
}
