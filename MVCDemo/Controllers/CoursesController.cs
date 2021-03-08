using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MVCDemo.DBContext;
using MVCDemo.Models;
using SolrNet.Utils;

namespace MVCDemo.Controllers
{
    public class CoursesController : Controller
    {
        private MVCDemoEntities db = new MVCDemoEntities();

        // GET: Courses
        public ActionResult CourseList()
        {
            List<CourseListViewModel> courseViewModel = new List<CourseListViewModel>();

            var courses = (from objCourse in db.Courses
                                       join objCourseType in db.CourseTypes on objCourse.CourseTypeId equals objCourseType.CourseTypeId
                                       select new
                                       {
                                           objCourse.CourseId,
                                           objCourse.CourseName,
                                           objCourse.CourseScreen,
                                           objCourseType.CourseTypeName,
                                           objCourse.ContentName,
                                           objCourse.FileExtension,
                                           objCourse.UploadCourse
                                       }).ToList();
            if (courses.Count>0)
            {
                foreach (var item in courses)
                {
                    CourseListViewModel courseListViewModel = new CourseListViewModel();
                    courseListViewModel.CourseId = item.CourseId;
                    courseListViewModel.CourseName = item.CourseName;
                    courseListViewModel.CourseScreen = item.CourseScreen;
                    courseListViewModel.CourseTypeName = item.CourseTypeName;
                    courseListViewModel.ContentName = item.ContentName;
                    courseListViewModel.FileExtension = item.FileExtension;
                    if (item.FileExtension==".zip")
                    {
                        using (var zippedStream = new MemoryStream(item.UploadCourse))
                        {
                            using (var archive = new ZipArchive(zippedStream))
                            {
                                var entry = archive.Entries.FirstOrDefault();
                                foreach (ZipArchiveEntry entrya in archive.Entries)
                                {
                                    if (entrya.FullName.EndsWith("index.html", StringComparison.OrdinalIgnoreCase))
                                    {
                                        courseListViewModel.HtmlExists = ".html";
                                    }
                                }
                            }
                        }
                    }
                    
                    courseViewModel.Add(courseListViewModel);

                }
                return View(courseViewModel);
            }

            return View();
        }
        //Download file from database
        public ActionResult OpenPage(int courseId)
        {
            var getFile = (from obj in db.Courses where obj.CourseId == courseId select obj).FirstOrDefault();

            string[] name = getFile.ContentName.Split('.');
            string fileName = name[1] + getFile.FileExtension;
            var myStr = "";
            using (var zippedStream = new MemoryStream(getFile.UploadCourse))
            {
                using (var archive = new ZipArchive(zippedStream))
                {
                    var entry = archive.Entries.FirstOrDefault();
                    foreach (ZipArchiveEntry entrya in archive.Entries)
                    {
                        if (entrya.FullName.EndsWith("index.html", StringComparison.OrdinalIgnoreCase))
                        {
                            Stream s = entrya.Open();
                            var sr = new StreamReader(s);
                            myStr = sr.ReadToEnd();
                        }
                    }
                }
            }
            return Content(myStr);
        }
        public FileResult DownloadFileFromDb(int courseId)
        {
            //Fetch the File data from Database.
            var getFile = (from obj in db.Courses where obj.CourseId==courseId select obj).FirstOrDefault();
            if (getFile.FileExtension==".zip")
            {
                string[] name = getFile.ContentName.Split('.');
                string fileName = name[1] + getFile.FileExtension;
                return File(getFile.UploadCourse, getFile.ContentType, fileName);
            }
            else
            {
                string[] name = getFile.ContentName.Split('.');
                string fileName = name[1] + getFile.FileExtension;
                return File(getFile.UploadCourse, getFile.ContentType, fileName);
            }
        }
        public FileResult CourseReport()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("Course-" + dTime.ToString("dd-MM-yyyy") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0F, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(4);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;
            return File(workStream, "application/pdf", strPDFFileName);

        }

        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 100, 100, 30, 50 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  

            List<ReportViewModel> course =new List<ReportViewModel>();
            var varCourse = db.uspGetMonthWiseCount().ToList();
            if (varCourse.Count>0)
            {
                foreach (var item in varCourse)
                {
                    ReportViewModel reportViewModel = new ReportViewModel();
                    reportViewModel.CourseName = item.CourseName;
                    reportViewModel.CourseTypeName = item.CourseTypeName;
                    reportViewModel.TotalCourse = Convert.ToInt32(item.TotalCourse);
                    reportViewModel.NameOFMonth = item.NameOFMonth;
                    course.Add(reportViewModel);
                }
            }


            tableLayout.AddCell(new PdfPCell(new Phrase("Course Report", new Font(Font.FontFamily.HELVETICA, 11, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) {
                Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER
            });


            ////Add header  
            AddCellToHeader(tableLayout, "Course Name");
            AddCellToHeader(tableLayout, "Course Type Name");
            AddCellToHeader(tableLayout, "Total Course");
            AddCellToHeader(tableLayout, "Month Name");

            ////Add body  

            foreach (var item in course)
            {

                AddCellToBody(tableLayout, item.CourseName);
                AddCellToBody(tableLayout, item.CourseTypeName);
                AddCellToBody(tableLayout, Convert.ToString(item.TotalCourse));
                AddCellToBody(tableLayout, item.NameOFMonth);

            }

            return tableLayout;
        }
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {

            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.YELLOW)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0)
             });
        }

        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
             {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
             });
        }
        public ActionResult Create()
        {
            CourseViewModel courseViewModel = new CourseViewModel();
            var coursesTypes = db.CourseTypes.ToList();
            foreach (var item in coursesTypes)
            {
                courseViewModel.CourseTypes.Add(new SelectListItem { Text = item.CourseTypeName, Value = item.CourseTypeId.ToString(), Selected = true });
            }
            return View(courseViewModel);
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseViewModel courseViewModel)
        {
            var coursesTypes = db.CourseTypes.ToList();
            foreach (var item in coursesTypes)
            {
                courseViewModel.CourseTypes.Add(new SelectListItem { Text = item.CourseTypeName, Value = item.CourseTypeId.ToString(), Selected = true });
            }
            if (courseViewModel.UploadCourseSaveToDatabase ==null)
            {
                ModelState.AddModelError("UploadCourseSaveToDatabase", "Select file");
                return View(courseViewModel);
            }
            if (ModelState.IsValid)
            {
                Course course = new Course();
                course.CourseName = courseViewModel.CourseName;
                course.CourseScreen = courseViewModel.CourseScreen;
                course.CourseTypeId = courseViewModel.CourseTypeId;
                course.CreatedDate = DateTime.Now;
                if (courseViewModel.UploadCourseSaveToDatabase != null)
                {
                    course.ContentName = Guid.NewGuid().ToString() + "." + courseViewModel.UploadCourseSaveToDatabase.FileName;
                    course.ContentType = courseViewModel.UploadCourseSaveToDatabase.ContentType;
                    course.FileExtension = Path.GetExtension(courseViewModel.UploadCourseSaveToDatabase.FileName); 
                    if (courseViewModel.UploadCourseSaveToDatabase.InputStream.Length >= 0)
                    {
                        byte[] bytes;
                        using (BinaryReader br = new BinaryReader(courseViewModel.UploadCourseSaveToDatabase.InputStream))
                        {
                            bytes = br.ReadBytes(courseViewModel.UploadCourseSaveToDatabase.ContentLength);
                        }
                        course.UploadCourse = bytes;
                    }
                }
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("CourseList");
            }
            return View(courseViewModel);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
