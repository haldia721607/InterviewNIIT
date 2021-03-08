using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace MVCDemo.Models
{
    public class CourseListViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseScreen { get; set; }
        public string CourseTypeName { get; set; }
        public string UploadCourse { get; set; }
        public string FileExtension { get; set; }
        public string HtmlExists { get; set; }
        public string ContentName { get; set; }
    }
}