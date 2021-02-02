using System;
using System.Collections.Generic;

namespace Sample.Data
{
    public partial class Contacts
    {
        public Contacts()
        {
            ContactGroups = new HashSet<ContactGroups>();
        }

        public int ContactId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }

        public virtual ICollection<ContactGroups> ContactGroups { get; set; }
    }
}
