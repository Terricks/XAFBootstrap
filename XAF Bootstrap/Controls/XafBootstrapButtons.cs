using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace XAF_Bootstrap.Controls
{
    public class XafBootstrapButton
    {   
        public XafBootstrapButton(XafBootstrapButtons buttons)
        {
            Buttons = buttons;
            Tag = "a";
        }

        protected XafBootstrapButtons Buttons;

        public String Caption;
        public String CssClass;
        public event EventHandler OnExecution;        

        public void RaiseExecutionEvent(object sender, EventArgs e)
        {
            if (OnExecution != null)
                OnExecution(sender, e);
        }

        public String Tag;
        public object Data;
    }
    public class XafBootstrapButtons : XafBootstrapBaseControl
    {
        public XafBootstrapButtons()
        {
            Content = new HTMLText();
            Buttons = new List<XafBootstrapButton>();
            Controls.Add(Content);
        }
        private HTMLText Content;
        public CallbackHandler Handler;
        public IList<XafBootstrapButton> Buttons;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitHandler(ClientID);
            InnerRender();
        }

        public XafBootstrapButton AddButton(String Caption, String CssClass = "glyphicon btn-sm xaf-bootstrap-button")
        {
            var btn = new XafBootstrapButton(this) { Caption = Caption, CssClass = CssClass };
            Buttons.Add(btn);
            return btn;
        }

        public void InitHandler(String HandlerID)
        {
            Handler = new CallbackHandler(HandlerID);
            Handler.OnCallback += Handler_OnCallback; ;
        }

        private void Handler_OnCallback(object source, DevExpress.Web.CallbackEventArgs e)
        {
            var idx = -1;
            if (int.TryParse(e.Parameter, out idx) && (idx > -1 && idx < Buttons.Count))            
                Buttons[idx].RaiseExecutionEvent(Buttons[idx], e);
        }

        public String ContentBefore;
        public String ContentAfter;

        public String InnerRender()
        {
            Content.Text = "";

            Content.Text += String.Concat(ContentBefore);

            var idx = 0;
            foreach(var button in Buttons)
            {
                Content.Text += String.Format("<{3} {4} class='{2}' style='min-width: 30px;' onclick=\"{1}\">{0}</{3}>"
                    , button.Caption
                    , Handler != null ? Handler.GetScript(String.Format("'{0}'", idx)) : ""
                    , button.CssClass
                    , button.Tag
                    , (button.Tag == "a" ? "href='javascript:;'" : button.Tag == "button" ? "type='button'" : "")
                );
                idx++;
            }

            Content.Text += String.Concat(ContentAfter);

            return Content.Text;
        }
    }
}
