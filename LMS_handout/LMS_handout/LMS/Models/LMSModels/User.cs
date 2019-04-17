using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class User
    {
        public string UId { get; set; }
        public string Type { get; set; }

        public virtual Administrators Administrators { get; set; }
        public virtual Professors Professors { get; set; }
        public virtual Students Students { get; set; }
    }
}
