using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Security.Strategy;
using XAF_Bootstrap.Templates;
using DevExpress.ExpressApp.DC;

namespace XAF_Bootstrap.Controllers.XafBootstrapConfiguration
{    
    public partial class XafBootstrapConfigurationAction : ViewController
    {
        public XafBootstrapConfigurationAction()
        {
            InitializeComponent();            
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            var actionVisible = SecuritySystem.CurrentUser == null;
            if (!actionVisible) {
                IMemberInfo memberInfo;
                var roles = ObjectFormatValues.GetValueRecursive("Roles", SecuritySystem.CurrentUser, out memberInfo) as IEnumerable<object>;
                foreach (var role in roles)
                {
                    Boolean isAdministrative;
                    if (Boolean.TryParse(String.Concat(ObjectFormatValues.GetValueRecursive("IsAdministrative", role, out memberInfo)), out isAdministrative))
                    {
                        if (isAdministrative)
                        {
                            actionVisible = true;
                            break;
                        }
                    }
                }
            }
        }
        protected override void OnDeactivated()
        {
            XafBootstrapConfigurationActionItem.Active.RemoveItem("IsActionActive");
            base.OnDeactivated();
        }
        private void XafBootstrapConfigurationActionItem_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var os = Application.CreateObjectSpace();            
            var view = Application.CreateDetailView(os, XAF_Bootstrap.DatabaseUpdate.Updater.Configuration(os));
            Application.MainWindow.SetView(view);
        }
    }
}
