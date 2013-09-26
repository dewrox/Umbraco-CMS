﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Security;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Umbraco.Core.Configuration;
using Umbraco.Core.Logging;
using Umbraco.Core.Models.Membership;
using System.Web.Security;
using umbraco.businesslogic.Exceptions;
using umbraco.IO;
using umbraco.cms.businesslogic.web;
using System.Linq;
using Umbraco.Core;
using User = umbraco.BusinessLogic.User;

namespace umbraco.cms.presentation
{
    /// <summary>
    /// Summary description for login.
    /// </summary>
    [Obsolete("This class is no longer used and will be removed")]
    public partial class login : BasePages.BasePage
    {
        [Obsolete("This property is no longer used")]
        protected umbWindow treeWindow;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ClientLoader.DataBind();

            // validate redirect url
            string redirUrl = Request["redir"];
            if (!String.IsNullOrEmpty(redirUrl))
            {
                validateRedirectUrl(redirUrl);
            }
        }


        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Button1.Text = ui.Text("general", "login");
            Panel1.Text = ui.Text("general", "welcome");
            Panel1.Style.Add("padding", "10px;");
            username.Text = ui.Text("general", "username");
            password.Text = ui.Text("general", "password");

            // Add bottom and top texts
            TopText.Text = ui.Text("login", "topText");


            BottomText.Text = ui.Text("login", "bottomText", DateTime.Now.Year.ToString(CultureInfo.InvariantCulture));

            //added this little hack to remove unessary formatting, without breaking all current language files.
            if (BottomText.Text.Contains("</p>"))
                BottomText.Text = BottomText.Text.Substring(29).Replace("<br />", "").Replace("</p>", "");
        }


        protected void Button1_Click(object sender, System.EventArgs e)
        {
            // Authenticate users by using the provider specified in umbracoSettings.config
            if (Membership.Providers[UmbracoConfig.For.UmbracoSettings().Providers.DefaultBackOfficeUserProvider].ValidateUser(lname.Text, passw.Text))
            {
                if (Membership.Providers[UmbracoConfig.For.UmbracoSettings().Providers.DefaultBackOfficeUserProvider] is ActiveDirectoryMembershipProvider)
                    ActiveDirectoryMapping(lname.Text, Membership.Providers[UmbracoConfig.For.UmbracoSettings().Providers.DefaultBackOfficeUserProvider].GetUser(lname.Text, false).Email);

                var u = new User(lname.Text);
                doLogin(u);
                
                if (hf_height.Value != "undefined")
                {
                    Session["windowHeight"] = hf_height.Value;
                    Session["windowWidth"] = hf_width.Value;
                }

                string redirUrl = Request["redir"];

                if (string.IsNullOrEmpty(redirUrl))
                    Response.Redirect("umbraco.aspx");
                else if (validateRedirectUrl(redirUrl))
                {
                    Response.Redirect(redirUrl, true);
                }
            }
            else
            {
                loginError.Visible = true;
            }
        }

        private bool validateRedirectUrl(string url)
        {
            if (!isUrlLocalToHost(url))
            {
                LogHelper.Info<login>(String.Format("Security warning: Login redirect was attempted to a site at another domain: '{0}'", url));

                throw new UserAuthorizationException(
                    String.Format(@"There was attempt to redirect to '{0}' which is another domain than where you've logged in. If you clicked a link to reach this login
                    screen, please double check that the link came from someone you trust. You *might* have been exposed to an *attempt* to breach the security of your website. Nothing 
                    have been compromised, though!", url));
            }

            return true;
        }

        private bool isUrlLocalToHost(string url)
        {
            if (String.IsNullOrEmpty(url))
            {
                return false;
            }

            Uri absoluteUri;
            if (Uri.TryCreate(url, UriKind.Absolute, out absoluteUri))
            {
                return String.Equals(HttpContext.Current.Request.Url.Host, absoluteUri.Host,
                            StringComparison.OrdinalIgnoreCase);
            }

            bool isLocal = !url.StartsWith("http:", StringComparison.OrdinalIgnoreCase)
                           && !url.StartsWith("https:", StringComparison.OrdinalIgnoreCase)
                           && Uri.IsWellFormedUriString(url, UriKind.Relative);
            return isLocal;
        }

        /// <summary>
        /// Maps active directory account to umbraco user account
        /// </summary>
        /// <param name="loginName">Name of the login.</param>
        /// <param name="email">Email address of the user</param>
        private void ActiveDirectoryMapping(string loginName, string email)
        {
            // Password is not copied over because it is stored in active directory for security!
            // The user is create with default access to content and as a writer user type
            if (BusinessLogic.User.getUserId(loginName) == -1)
            {
                BusinessLogic.User.MakeNew(loginName, loginName, string.Empty, email ?? "", BusinessLogic.UserType.GetUserType(2));
                var u = new User(loginName);
                u.addApplication(Constants.Applications.Content);
            }
        }

        /// <summary>
        /// ClientLoader control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::umbraco.uicontrols.UmbracoClientDependencyLoader ClientLoader;

        /// <summary>
        /// CssInclude1 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::ClientDependency.Core.Controls.CssInclude CssInclude1;

        /// <summary>
        /// JsInclude1 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::ClientDependency.Core.Controls.JsInclude JsInclude1;

        /// <summary>
        /// JsInclude3 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::ClientDependency.Core.Controls.JsInclude JsInclude3;

        /// <summary>
        /// JsInclude2 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::ClientDependency.Core.Controls.JsInclude JsInclude2;

        /// <summary>
        /// Form1 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.HtmlControls.HtmlForm Form1;

        /// <summary>
        /// Panel1 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::umbraco.uicontrols.UmbracoPanel Panel1;

        /// <summary>
        /// TopText control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.Literal TopText;

        /// <summary>
        /// username control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.Label username;

        /// <summary>
        /// lname control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.TextBox lname;

        /// <summary>
        /// password control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.Label password;

        /// <summary>
        /// passw control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.TextBox passw;

        /// <summary>
        /// Button1 control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.Button Button1;

        /// <summary>
        /// BottomText control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.Literal BottomText;

        /// <summary>
        /// hf_height control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.HiddenField hf_height;

        /// <summary>
        /// hf_width control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.HiddenField hf_width;

        /// <summary>
        /// loginError control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected global::System.Web.UI.WebControls.PlaceHolder loginError;
    }
}
