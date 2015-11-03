using System;
using System.Runtime.Serialization;
using System.Web.Security;
using SimpleMVCAuthentication.Security.Principal;
using SimpleMVCAuthentication.Settings;

namespace SimpleMVCAuthentication.Security
{    
    public class SimpleAuthenticationTicket
    {
        public string UserName { get; set; }
        public bool KeepLoggedIn { get;set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public SimpleAuthenticationTicket()
        {
            UserName = "";
            KeepLoggedIn = false;
            IssueDate = DateTime.Now;
            ExpirationDate = DateTime.Now;            
        }

        public SimpleAuthenticationTicket(string UserName, DateTime IssueDate, DateTime ExpirationDate, bool KeepLoggedIn)
        {            
            this.UserName = UserName;
            this.KeepLoggedIn = KeepLoggedIn;
            this.IssueDate = IssueDate;
            this.ExpirationDate = ExpirationDate;
        }

        public static SimpleAuthenticationTicket Decrypt(string Token)
        {
            ICryptoProvider provider = new RijndaelCryptoProvider(SettingsManager.AuthenticationSettings.EncryptionKey);

            if (string.IsNullOrWhiteSpace(Token))
            {
                return null;
            }

            SimpleAuthenticationTicket ticket = provider.Decrypt<SimpleAuthenticationTicket>(Token);

            return ticket;
        }

        /*public SimpleAuthenticationTicket(string Token)
        {
            //Пока используем Forms

            cryptoProvider = new RijndaelCryptoProvider(SettingsManager.AuthenticationSettings.EncryptionKey);

            if (string.IsNullOrWhiteSpace(Token))
            {
                return;
            }
          

            if (ticket == null)
            {
                return;
            }

            IssueDate = ticket.IssueDate;
            ExpirationDate = ticket.Expiration;
            UserName = ticket.Name;
            KeepLoggedIn = ticket.IsPersistent;
        }*/

        public string Encrypt()
        {
            /*string userData = string.Format("UserId={0};DisplayName={1}", User.UserIdentity.UserId,
               User.UserIdentity.DisplayName);*/

            ICryptoProvider provider = new RijndaelCryptoProvider(SettingsManager.AuthenticationSettings.EncryptionKey);

            return provider.Encrypt(this);

            /*FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, UserName, IssueDate,
                ExpirationDate, KeepLoggedIn, "");

            return FormsAuthentication.Encrypt(ticket);*/
        }
    }
}
