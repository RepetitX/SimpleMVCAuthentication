using System;
using System.Web;
using SimpleMVCAuthentication.Security.Principal;
using SimpleMVCAuthentication.Settings;

namespace SimpleMVCAuthentication.Security
{
    public abstract class SimpleAuthenticationHandler : IAuthenticationHandler 
    {
        public abstract AuthenticationResult Authenticate(string Login, string Password);
        public abstract AuthenticationResult Authenticate(string Login);

        protected ICryptoProvider cryptoProvider =
            new RijndaelCryptoProvider(SettingsManager.AuthenticationSettings.EncryptionKey);

        public void LogOut(HttpContextBase Context)
        {
            HttpCookie authCookie = Context.Request.Cookies[SettingsManager.AuthenticationSettings.CookieName];
            if (authCookie != null)
            {
                authCookie.Expires = DateTime.Now;
                Context.Response.SetCookie(authCookie);
            }

            HttpCookie sessionCookie = Context.Request.Cookies[SettingsManager.AuthenticationSettings.SessionCookieName];

            if (sessionCookie != null)
            {
                sessionCookie.Expires = DateTime.Now;
                Context.Response.SetCookie(sessionCookie);
            }
            Context.User = User.Anonymous;
        }

        public User AuthenticateRequest(HttpContextBase Context)
        {
            HttpCookie cookie = Context.Request.Cookies[SettingsManager.AuthenticationSettings.CookieName];

            User user;
            if (cookie == null)
            {
                user = User.Anonymous;
                Context.User = user;
                return user;
            }
            SimpleAuthenticationTicket ticket = new SimpleAuthenticationTicket(cookie.Value);

            if (string.IsNullOrWhiteSpace(ticket.UserName))
            {
                //cookie испорчен
                cookie.Expires = DateTime.Now;
                Context.Response.SetCookie(cookie);
                user = User.Anonymous;
                Context.User = user;
                return user;
            }
            //Проверка данных пользователя

            HttpCookie sessionCookie = Context.Request.Cookies[SettingsManager.AuthenticationSettings.SessionCookieName];

            if (sessionCookie != null)
            {
                var sessionData = DecryptSessionData(sessionCookie);

                if (sessionData != null && sessionData.ExpirationDate > DateTime.Now)
                {
                    //Проверка пока не нужна

                    user = GetUser(ticket, sessionData);
                    Context.User = user;
                    return user;
                }
            }
            //Нужна проверка

            AuthenticationResult result = Authenticate(ticket.UserName);

            if (result.Status == AuthenticationStatus.Success)
            {
                //Надо обновить cookie, на случай если права изменились  
                UpdateAuthCookie(Context.Response, cookie);
                sessionCookie = CreateSessionCookie(result.User);
                Context.Response.SetCookie(sessionCookie);
                Context.User = result.User;
                return result.User;
            }
            //Проверка не пройдена, убираем пользователя

            cookie.Expires = DateTime.Now;
            Context.Response.Cookies.Add(cookie);

            Context.User = User.Anonymous;

            return User.Anonymous;
        }

        public HttpCookie CreateAuthCookie(User User, bool KeepLoggedIn)
        {
            HttpCookie cookie = new HttpCookie(SettingsManager.AuthenticationSettings.CookieName);

            if (KeepLoggedIn)
            {
                cookie.Expires = DateTime.Now.AddDays(SettingsManager.AuthenticationSettings.DaysToExpiration);
            }

            cookie.Value = EncryptUser(User, KeepLoggedIn);

            return cookie;
        }

        protected void UpdateAuthCookie(HttpResponseBase Response, HttpCookie cookie)
        {
            SimpleAuthenticationTicket ticket = new SimpleAuthenticationTicket(cookie.Value);
            //Если KeepLoggedIn, то продляем
            if (ticket.KeepLoggedIn)
            {
                ticket.ExpirationDate = DateTime.Now.AddDays(SettingsManager.AuthenticationSettings.DaysToExpiration);
                cookie.Expires = ticket.ExpirationDate;
            }

            cookie.Value = ticket.Encrypt();

            Response.SetCookie(cookie);
        }

        public HttpCookie CreateSessionCookie(User User)
        {
            if (string.IsNullOrWhiteSpace(SettingsManager.AuthenticationSettings.SessionCookieName) ||
                SettingsManager.AuthenticationSettings.SessionTimeOut <= 0)
            {
                return null;
            }
            HttpCookie cookie = new HttpCookie(SettingsManager.AuthenticationSettings.SessionCookieName);            

            cookie.Expires = DateTime.Now.AddMinutes(SettingsManager.AuthenticationSettings.SessionTimeOut);
            cookie.Value = EncryptSessionData(new SessionData(User, cookie.Expires));

            return cookie;
        }        

        public void SetCookies(HttpResponseBase Response, User User, bool KeepLoggedIn)
        {
            HttpCookie cookie = CreateAuthCookie(User, KeepLoggedIn);
            Response.SetCookie(cookie);

            cookie = CreateSessionCookie(User);
            if (cookie != null)
            {
                Response.SetCookie(cookie);
            }
        }

        protected User GetUser(SimpleAuthenticationTicket Ticket, SessionData Data)
        {
            UserIdentity identity = new UserIdentity(Ticket.UserName, Data.UserId, Data.UserDisplayName, Data.Roles);

            return new User(identity);
        }

        protected string EncryptUser(User User, bool KeepLoggedIn)
        {
            SimpleAuthenticationTicket ticket = new SimpleAuthenticationTicket(User.Identity.Name, DateTime.Now,
                DateTime.Now.AddDays(SettingsManager.AuthenticationSettings.DaysToExpiration), KeepLoggedIn);

            return ticket.Encrypt();
        }        

        protected SessionData DecryptSessionData(HttpCookie SessionCookie)
        {
            if (SessionCookie == null)
            {
                return null;
            }
            try
            {
                return cryptoProvider.Decrypt<SessionData>(SessionCookie.Value);
            }
            catch (Exception e)
            {
                
            }            
            return null;
        }

        protected string EncryptSessionData(SessionData Data)
        {
            return cryptoProvider.Encrypt(Data);
        }


    }
}