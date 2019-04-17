using System;
using System.Collections.Generic;

namespace LMS.Models.LMSModels
{
    public partial class Classes
    {
        public Classes()
        {
            AssignmentCategories = new HashSet<AssignmentCategories>();
            Enrolled = new HashSet<Enrolled>();
            EnrollmentGrade = new HashSet<EnrollmentGrade>();
        }

        public uint ClassId { get; set; }
        public uint CId { get; set; }
        public string Semester { get; set; }
        public string Teacher { get; set; }
        public string Loc { get; set; }
        public DateTime STime { get; set; }
        public DateTime ETime { get; set; }

        public virtual Courses C { get; set; }
        public virtual Professors TeacherNavigation { get; set; }
        public virtual ICollection<AssignmentCategories> AssignmentCategories { get; set; }
        public virtual ICollection<Enrolled> Enrolled { get; set; }
        public virtual ICollection<EnrollmentGrade> EnrollmentGrade { get; set; }
    }
}
