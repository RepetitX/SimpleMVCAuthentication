using System.Web;
using SimpleMVCAuthentication.Security.Principal;

namespace SimpleMVCAuthentication.Security
{
    public interface IAuthenticationHandler
    {
        AuthenticationResult Authenticate(string Login, string Password);
        AuthenticationResult Authenticate(string Login);        
        User AuthenticateRequest(HttpContextBase Context);
        void LogOut(HttpContextBase Context);

        HttpCookie CreateAuthCookie(User User, bool KeepLoggedIn);
        HttpCookie CreateSessionCookie(User User);
        void SetCookies(HttpResponseBase Response, User User, bool KeepLoggedIn);

        /*void SaveSessionData<T>(T Data);
        T LoadSessionData<T>(User User);*/
    }
}