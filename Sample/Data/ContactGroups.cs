using System;
using System.Collections.Generic;

namespace Sample.Data
{
    public partial class ContactGroups
    {
        public int ContactGroupId { get; set; }
        public int ContactId { get; set; }
        public string GroupName { get; set; }
        public int ContactMapId { get; set; }

        public virtual Contacts Contact { get; set; }
    }
}
