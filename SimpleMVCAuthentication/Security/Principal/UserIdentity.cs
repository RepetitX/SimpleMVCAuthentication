using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Security;

namespace SimpleMVCAuthentication.Security.Principal
{
    public class UserIdentity : IIdentity
    {
        public int UserId { get; private set; }
        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string AuthenticationType { get; private set; }
        public bool IsAuthenticated { get; private set; }
        public List<UserRole> Roles { get; private set; }

        public UserIdentity()
        {
            AuthenticationType = "SimpleMVC";
            IsAuthenticated = true;
            UserId = 0;
            Roles = new List<UserRole>();
        }

        public UserIdentity(string Name, int UserId)
            : this()
        {
            this.Name = Name;
            this.UserId = UserId;
        }

        public UserIdentity(string Name, int UserId, string DisplayName)
            : this(Name, UserId)
        {
            this.DisplayName = DisplayName;
        }

        public UserIdentity(string Name, int UserId, string DisplayName, List<UserRole> Roles)
            : this(Name, UserId, DisplayName)
        {
            this.Roles = Roles;
        }

        public UserIdentity(string Name, bool IsAuthenticated)
            : this()
        {
            this.Name = Name;
            this.IsAuthenticated = IsAuthenticated;
        }
    }
}