﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Examination.Models
{
    public partial class course
    {
        public course()
        {
            exams = new HashSet<exam>();
            questions = new HashSet<question>();
            topics = new HashSet<topic>();
            insts = new HashSet<instructor>();
            studs = new HashSet<student>();
        }

        public int c_ID { get; set; }
        public string c_name { get; set; }

        public virtual ICollection<exam> exams { get; set; }
        public virtual ICollection<question> questions { get; set; }
        public virtual ICollection<topic> topics { get; set; }

        public virtual ICollection<instructor> insts { get; set; }
        public virtual ICollection<student> studs { get; set; }
    }
}