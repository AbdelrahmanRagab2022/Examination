﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Examination.Models
{
    public partial class topic
    {
        public int topic_ID { get; set; }
        public string topic_name { get; set; }
        public int course_ID { get; set; }

        public virtual course course { get; set; }
    }
}