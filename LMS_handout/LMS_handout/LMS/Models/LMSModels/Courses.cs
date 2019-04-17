﻿using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Courses
    {
        public Courses()
        {
            Classes = new HashSet<Classes>();
        }

        public string Name { get; set; }
        public uint Number { get; set; }
        public uint CId { get; set; }
        public string Listing { get; set; }

        public virtual Departments ListingNavigation { get; set; }
        public virtual ICollection<Classes> Classes { get; set; }
    }
}
