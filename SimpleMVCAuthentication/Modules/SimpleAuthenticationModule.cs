using System;
using System.Web;
using SimpleMVCAuthentication.Security;
using SimpleMVCAuthentication.Security.Principal;

namespace SimpleMVCAuthentication.Modules
{
    public abstract class SimpleAuthenticationModule : IAuthenticationModule
    {
        protected abstract IAuthenticationHandler AuthenticationHandler { get; set; }

        public void Init(HttpApplication context)
        {
            context.LogRequest += OnLogRequest;
            context.AuthenticateRequest += AuthenticateRequest;
        }

        private void AuthenticateRequest(object sender, EventArgs e)
        {
            User user = AuthenticationHandler.AuthenticateRequest(new HttpContextWrapper(HttpContext.Current));
        }

        public abstract void RequestAuthenticated(User User);

        public void OnLogRequest(Object source, EventArgs e)
        {

        }

        public void Dispose()
        {

        }
    }
}