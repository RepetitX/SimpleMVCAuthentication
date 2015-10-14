using System;
using System.Collections.Generic;

namespace SimpleMVCAuthentication.Security.Principal
{
    public class SessionData
    {
        public DateTime ExpirationDate { get; set; }
        public int UserId { get; set; }
        public string UserDisplayName { get; set; }
        public List<UserRole> Roles { get; set; }

        public SessionData()
        {
            Roles = new List<UserRole>();
        }
        public SessionData(User User, DateTime ExpirationDate)
        {
            this.ExpirationDate = ExpirationDate;
            UserId = User.UserIdentity.UserId;
            UserDisplayName = User.UserIdentity.DisplayName;
            Roles = User.UserIdentity.Roles;
        }
    }
}