//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MVCDemo.DBContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseScreen { get; set; }
        public int CourseTypeId { get; set; }
        public byte[] UploadCourse { get; set; }
        public string ContentName { get; set; }
        public string ContentType { get; set; }
        public string FileExtension { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
    
        public virtual CourseType CourseType { get; set; }
    }
}
