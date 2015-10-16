using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Web;
using SimpleMVCAuthentication.Security;
using SimpleMVCAuthentication.Security.Principal;

namespace SimpleMVCAuthentication.Modules
{
    public abstract class SimpleAuthenticationModule : IAuthenticationModule
    {
        protected abstract IAuthenticationHandler AuthenticationHandler { get; set; }

        protected readonly string[] defaultIgnoredExtensions =
        {
            ".js", ".css", //resources
            ".png", ".jpg", ".jpeg", ".gif", ".ico", ".svg", //images
            ".woff2", ".woff", ".ttf", ".eot" //fonts
        };

        protected List<string> ignoredExtensions = new List<string>();
        protected List<string> ignoredRequests = new List<string>();

        public void Init(HttpApplication context)
        {
            /* test 2 */
            context.LogRequest += OnLogRequest;
            context.AuthenticateRequest += AuthenticateRequest;

            ignoredExtensions.AddRange(defaultIgnoredExtensions);
        }

        private void AuthenticateRequest(object sender, EventArgs e)
        {            
            //Не проверяем запросы добавленные в игнор
            if (
                ignoredRequests.Any(
                    ir =>
                        HttpContext.Current.Request.CurrentExecutionFilePath.StartsWith(ir,
                            StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }
            //Не проверяем ресурсы и статические файлы
            if (ignoredExtensions.Contains(HttpContext.Current.Request.CurrentExecutionFilePathExtension))
            {
                return;
            }

            User user = AuthenticationHandler.AuthenticateRequest(new HttpContextWrapper(HttpContext.Current));

            RequestAuthenticated(user);
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