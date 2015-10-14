using System;
using System.Web.Security;
using SimpleMVCAuthentication.Security.Principal;

namespace SimpleMVCAuthentication.Security
{
    public class SimpleAuthenticationTicket
    {
        public string UserName { get; set; }
        public bool KeepLoggedIn { get;set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public SimpleAuthenticationTicket(string UserName, DateTime IssueDate, DateTime ExpirationDate, bool KeepLoggedIn)
        {
            this.UserName = UserName;
            this.KeepLoggedIn = KeepLoggedIn;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
        }

        public SimpleAuthenticationTicket(string Token)
        {
            //Пока используем Forms

            if (string.IsNullOrWhiteSpace(Token))
            {
                return;
            }

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(Token);

            if (ticket == null)
            {
                return;
            }

            IssueDate = ticket.IssueDate;
            ExpirationDate = ticket.Expiration;
            UserName = ticket.Name;
            KeepLoggedIn = ticket.IsPersistent;
        }

        public string Encrypt()
        {
            /*string userData = string.Format("UserId={0};DisplayName={1}", User.UserIdentity.UserId,
               User.UserIdentity.DisplayName);*/

            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, UserName, IssueDate,
                ExpirationDate, KeepLoggedIn, "");

            return FormsAuthentication.Encrypt(ticket);
        }
    }
}
