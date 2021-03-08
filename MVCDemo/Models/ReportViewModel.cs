using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDemo.Models
{
    public class ReportViewModel
    {
        public string CourseName { get; set; }
        public string CourseTypeName { get; set; }
        public int TotalCourse { get; set; }
        public string NameOFMonth { get; set; }
    }
}