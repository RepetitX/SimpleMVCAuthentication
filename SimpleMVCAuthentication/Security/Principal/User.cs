using System.Linq;
using System.Security.Principal;

namespace SimpleMVCAuthentication.Security.Principal
{
    public class User : IPrincipal
    {
        public bool IsInRole(string role)
        {
            return UserIdentity.Roles.Any(rl => rl.Name == role);
        }

        public IIdentity Identity { get; private set; }

        public UserIdentity UserIdentity
        {
            get { return (UserIdentity) Identity; }
        }

        public User(string Name, int Id)
        {
            Identity = new UserIdentity(Name, Id);
        }

        public User(string Name, int Id, string DisplayName)
        {
            Identity = new UserIdentity(Name, Id, DisplayName);
        }

        public User(string Name, bool IsAuthenticated)
        {
            Identity = new UserIdentity(Name, IsAuthenticated);
        }

        public User(UserIdentity UserIdentity)
        {
            Identity = UserIdentity;
        }

        public static User Anonymous
        {
            get { return new User("anonymous", false); }
        }
    }
}