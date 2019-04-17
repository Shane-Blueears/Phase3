using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Administrators
    {
        public string UId { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public uint? Dob { get; set; }

        public virtual User U { get; set; }
    }
}
