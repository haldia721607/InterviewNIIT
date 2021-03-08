using MVCDemo.DBContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCDemo.Models
{
    public class CourseViewModel
    {
        public CourseViewModel()
        {
            this.CourseTypes = new List<SelectListItem>();
        }
        public int CourseId { get; set; }
        [DisplayName("Course Name")]
        [Required(AllowEmptyStrings =false,ErrorMessage = "Fill course")]
        [MaxLength(100, ErrorMessage = "Only 100 characters are allowed.")]
        [RegularExpression(@"^[0-9a-zA-Z''-'\s]{1,40}$",ErrorMessage = "Special characters are not  allowed.")]
        public string CourseName { get; set; }
        [DisplayName("Course Screen")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Fill course screen")]
        [MaxLength(10,ErrorMessage = "Only 10 digits are allowed.")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Course screen must be numeric")]
        public string CourseScreen { get; set; }
        [DisplayName("Course Type")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Select course type")]
        public int CourseTypeId { get; set; }

        [DisplayName("Upload Course")]
        [Required(ErrorMessage = "Select file")]
        public HttpPostedFileBase UploadCourseSaveToDatabase { get; set; }
        public FileViewModel UploadCourseSaveToDatabaseViewModel { get; set; }
        public List<SelectListItem> CourseTypes { get; set; }

        public virtual CourseType CourseType { get; set; }
    }
}