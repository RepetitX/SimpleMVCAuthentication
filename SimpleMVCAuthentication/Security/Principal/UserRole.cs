using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SimpleMVCAuthentication.Security.Principal
{
    [DataContract]
    public class UserRole
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Name { get; set; }        

        public UserRole(int Id, string Name)
        {
            this.Id = Id;
            this.Name = Name;
        }
    }
}