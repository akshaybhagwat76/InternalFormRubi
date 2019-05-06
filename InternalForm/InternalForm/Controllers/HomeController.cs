using InternalFormRubi.Helpers;
using InternalFormRubi.Models;
using InternalFormRubi.ViewModels;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace InternalFormRubi.Controllers
{
    public class HomeController : Controller
    {
        private FeedbackAppEntities _DB = new FeedbackAppEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [HttpGet]
        public ActionResult Internalform()
        {
            BindDropDownList();
            return View();
        }
        [HttpPost]
        public ActionResult Internalform(Employee employee)
        {
            try
            {
                var pdfMemory = GeneratePDf(employee);
                EmailSender.SendMailMessage("akshaybhagwat76@gmail.com", "asas@gmail.com", "Internal form", GetEmailBody(employee), pdfMemory);
                TempData["lstEmployee"] = employee;
                return Json(employee, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult InternalFormView(Employee employee)
        {
            return Json("Sucesss", JsonRequestBehavior.AllowGet);
        }

        public ActionResult FormRecords()
        {
            return View();
        }


        private void BindDropDownList()
        {


            var employees = _DB.Database.SqlQuery<internalformModel>(@"select emp_namefirst + ' ' + emp_namelast as empFullName, emp_id  from dbo.employee").ToList();

            ViewBag.Employees = new SelectList(employees, "emp_id", "empFullName");

        }

        private string GetEmailBody(Employee empObj)
        {
            string htmlBody = string.Empty;
            string brString = "<br />";

            htmlBody += "Position Apppling :" + empObj.PositionApplied + brString + brString;
            if(!string.IsNullOrEmpty(empObj.Manager))
                htmlBody += "Manager :" + empObj.Manager + brString + brString;
            htmlBody += "Date started :" + empObj.DateStarted.ToLongDateString() + brString;

            if (empObj.Education != null)
            {
                htmlBody += "<h4>Education : </h4> <br />";
                if (!string.IsNullOrEmpty(empObj.Education.Name))
                    htmlBody += "Name :" + empObj.Education.Name + brString;
                if (!string.IsNullOrEmpty(empObj.Education.Subject))
                    htmlBody += "Major Course or Subject :" + empObj.Education.Subject + brString;
                if (!string.IsNullOrEmpty(empObj.Education.CompletedYear))
                    htmlBody += "Last Year Completed :" + empObj.Education.CompletedYear + brString;
                if (!string.IsNullOrEmpty(empObj.Education.Graduated))
                    htmlBody += "Graduated? :" + empObj.Education.Graduated + brString;
                if (!string.IsNullOrEmpty(empObj.Education.Degree))
                    htmlBody += "Degree :" + empObj.Education.Degree + brString;
            }

            if (empObj.Colleges != null && empObj.Colleges.Count > 0)
            {
                htmlBody += "<h4>College : </h4> <br />";

                empObj.Colleges.ForEach(o =>
                {
                    if (!string.IsNullOrEmpty(o.Name))
                        htmlBody += "Name :" + o.Name + brString;
                    if (!string.IsNullOrEmpty(o.Subject))
                        htmlBody += "Major Course or Subject :" + o.Subject + brString;
                    if (!string.IsNullOrEmpty(o.CompletedYear))
                        htmlBody += "Last Year Completed :" + o.CompletedYear + brString;
                    if (!string.IsNullOrEmpty(o.CompletedYear))
                        htmlBody += "Graduated? :" + o.Graduated + brString;
                    if (!string.IsNullOrEmpty(o.Degree))
                        htmlBody += "Degree :" + o.Degree + brString;
                });
            }

            if (empObj.Languages != null && empObj.Languages.Count > 0)
            {
                htmlBody += "<h4>Languages : </h4> <br />";

                empObj.Languages.ForEach(o =>
                {
                    if (!string.IsNullOrEmpty(o.Name))
                        htmlBody += "Name :" + o.Name + brString;
                    if (!string.IsNullOrEmpty(o.Skills))
                        htmlBody += "Skills :" + o.Skills + brString;
                });
            }

            if (!string.IsNullOrEmpty(empObj.Comments))
                htmlBody += brString +  "<h4>Others</h4> : " + empObj.Comments + brString;

            if (empObj.EmploymentHistory != null && empObj.EmploymentHistory.Count > 0)
            {
                htmlBody += "<h4>Company</h4> <br />";

                empObj.EmploymentHistory.ForEach(e =>
                {
                    if (!string.IsNullOrEmpty(e.Name))
                        htmlBody += "Name :" + e.Name + brString;
                    if (!string.IsNullOrEmpty(e.Address))
                        htmlBody += "Address :" + e.Address + brString;
                    if (!string.IsNullOrEmpty(e.FromDate) || !string.IsNullOrEmpty(e.ToDate))
                        htmlBody += "Dates of Employement: " + e.FromDate + "To" + e.ToDate + brString;
                    if (!string.IsNullOrEmpty(e.Department))
                        htmlBody += "Your Title and Department: " + e.Department + brString;
                    if (!string.IsNullOrEmpty(e.Description))
                        htmlBody += "Describe your Duties:: " + e.Description + brString;
                });
            }

            if (!string.IsNullOrEmpty(empObj.AdditionalNotes) && !string.IsNullOrWhiteSpace(empObj.AdditionalNotes))
                htmlBody += "<p>Additonal Notes </p>: " + empObj.AdditionalNotes;

            return htmlBody;
        }

        public MemoryStream GeneratePDf(Employee employee)
        {

            MemoryStream output = new MemoryStream();
            Document pdfDoc = new Document(PageSize.A4, 25, 10, 25, 10);
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, output);

            pdfDoc.Open();

            PdfPTable pdfPTable = new PdfPTable(2);
            pdfPTable.AddCell("Position Apppling :");
            pdfPTable.AddCell(employee.PositionApplied);
            if (!string.IsNullOrEmpty(employee.Manager))
                pdfPTable.AddCell("Manager :");
                pdfPTable.AddCell(employee.Manager);
            pdfPTable.AddCell("Date started :");
            pdfPTable.AddCell(employee.DateStarted.ToShortDateString());
            if (employee.Education != null)
            {
                if (!string.IsNullOrEmpty(employee.Education.Name) || !string.IsNullOrEmpty(employee.Education.Subject) ||
                   !string.IsNullOrEmpty(employee.Education.CompletedYear) || !string.IsNullOrEmpty(employee.Education.Graduated) ||
                   !string.IsNullOrEmpty(employee.Education.GraduatedDate) || !string.IsNullOrEmpty(employee.Education.Degree))
                {
                    var eduCellHeader = new PdfPCell(new Phrase("Education", new Font(Font.FontFamily.TIMES_ROMAN, 15, Font.BOLD)));
                    eduCellHeader.Colspan = 2;
                    pdfPTable.AddCell(eduCellHeader);
                    if (!string.IsNullOrEmpty(employee.Education.Name))
                    {
                        pdfPTable.AddCell("Name :");
                        pdfPTable.AddCell(employee.Education.Name);
                    }
                    if (!string.IsNullOrEmpty(employee.Education.Subject))
                    {
                        pdfPTable.AddCell("Major Course or Subject: ");
                        pdfPTable.AddCell(employee.Education.Subject);
                    }
                    if (!string.IsNullOrEmpty(employee.Education.CompletedYear))
                    {
                        pdfPTable.AddCell("Last Year Completed: ");
                        pdfPTable.AddCell(employee.Education.CompletedYear);
                    }
                    if (!string.IsNullOrEmpty(employee.Education.Graduated))
                    {
                        pdfPTable.AddCell("Graduated? ");
                        pdfPTable.AddCell(employee.Education.Graduated);
                    }
                    if (!string.IsNullOrEmpty(employee.Education.GraduatedDate))
                    {
                        pdfPTable.AddCell("Graduated Month/Year");
                        pdfPTable.AddCell(employee.Education.GraduatedDate);
                    }
                    if (!string.IsNullOrEmpty(employee.Education.Degree))
                    {
                        pdfPTable.AddCell("Degree: ");
                        pdfPTable.AddCell(employee.Education.Degree);
                    }
                }
            }


            if (employee.Colleges != null && employee.Colleges.Count > 0)
            {
                employee.Colleges.ForEach(o =>
                       {
                           if (o != null)
                           {
                               if (!string.IsNullOrEmpty(o.Name) || !string.IsNullOrEmpty(o.Subject) ||
                                   !string.IsNullOrEmpty(o.CompletedYear) || !string.IsNullOrEmpty(o.Graduated) ||
                                   !string.IsNullOrEmpty(o.GraduatedDate) || !string.IsNullOrEmpty(o.Degree))
                               {

                                   var collegeCellHeader = new PdfPCell(new Phrase("College", new Font(Font.FontFamily.TIMES_ROMAN, 15, Font.BOLD)));
                                   collegeCellHeader.Colspan = 2;
                                   pdfPTable.AddCell(collegeCellHeader);
                                   if (!string.IsNullOrEmpty(o.Name))
                                   {
                                       pdfPTable.AddCell("Name :");
                                       pdfPTable.AddCell(o.Name);
                                   }
                                   if (!string.IsNullOrEmpty(o.Subject))
                                   {
                                       pdfPTable.AddCell("Major Course or Subject: ");
                                       pdfPTable.AddCell(o.Subject);
                                   }
                                   if (!string.IsNullOrEmpty(o.CompletedYear))
                                   {
                                       pdfPTable.AddCell("Last Year Completed: ");
                                       pdfPTable.AddCell(o.CompletedYear);
                                   }
                                   if (!string.IsNullOrEmpty(o.Graduated))
                                   {
                                       pdfPTable.AddCell("Graduated? ");
                                       pdfPTable.AddCell(o.Graduated);
                                   }
                                   if (!string.IsNullOrEmpty(o.GraduatedDate))
                                   {
                                       pdfPTable.AddCell("Graduated Month/Year");
                                       pdfPTable.AddCell(o.GraduatedDate);
                                   }
                                   if (!string.IsNullOrEmpty(o.Degree))
                                   {
                                       pdfPTable.AddCell("Degree: ");
                                       pdfPTable.AddCell(o.Degree);
                                   }
                               }
                           }
                       });
            }

            if (employee.Languages != null && employee.Languages.Count > 0)
            {
                var langCellHeader = new PdfPCell(new Phrase("Languages", new Font(Font.FontFamily.TIMES_ROMAN, 15, Font.BOLD)));
                langCellHeader.Colspan = 2;
                pdfPTable.AddCell(langCellHeader);
                employee.Languages.ForEach(o =>
                {
                    if (!string.IsNullOrEmpty(o.Name) || !string.IsNullOrEmpty(o.Skills))
                    {
                        var langCell = new PdfPCell(new Phrase(o.Name + ":  " + o.Skills));
                        langCell.Colspan = 2;
                        langCell.HorizontalAlignment = Element.ALIGN_LEFT;
                        langCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        pdfPTable.AddCell(langCell);
                    }
                });
            }
            if (!string.IsNullOrEmpty(employee.Comments))
            {
                var otherCell1 = new PdfPCell(new Phrase("Other:  ", new Font(Font.FontFamily.TIMES_ROMAN, 15, Font.BOLD)));
                otherCell1.Colspan = 2;
                pdfPTable.AddCell(otherCell1);
                var otherCell2 = new PdfPCell(new Phrase(employee.Comments));
                otherCell2.Colspan = 2;
                pdfPTable.AddCell(otherCell2);
            }

            if (employee.EmploymentHistory != null && employee.EmploymentHistory.Count > 0)
            {
                var emptCellHeader = new PdfPCell(new Phrase("Employment History:", new Font(Font.FontFamily.TIMES_ROMAN, 15, Font.BOLD)));
                emptCellHeader.Colspan = 2;
                pdfPTable.AddCell(emptCellHeader);
                employee.EmploymentHistory.ForEach(o =>
                {
                    var compCellHeader = new PdfPCell(new Phrase("Company", new Font(Font.FontFamily.TIMES_ROMAN, 12, Font.BOLD)));
                    compCellHeader.Colspan = 2;
                    compCellHeader.HorizontalAlignment = Element.ALIGN_LEFT;
                    compCellHeader.VerticalAlignment = Element.ALIGN_MIDDLE;
                    pdfPTable.AddCell(compCellHeader);
                    if (!string.IsNullOrEmpty(o.Name))
                    {
                        pdfPTable.AddCell("Name :");
                        pdfPTable.AddCell(o.Name);
                    }
                    if (!string.IsNullOrEmpty(o.Address))
                    {
                        pdfPTable.AddCell("Address: ");
                        pdfPTable.AddCell(o.Address);
                    }
                    if (!string.IsNullOrEmpty(o.FromDate) || !string.IsNullOrEmpty(o.ToDate))
                    {
                        var emptCellDate = new PdfPCell(new Phrase("Dates of Employement: " + o.FromDate + "To" + o.ToDate));
                        emptCellDate.Colspan = 2;
                        emptCellDate.HorizontalAlignment = Element.ALIGN_LEFT;
                        emptCellDate.VerticalAlignment = Element.ALIGN_MIDDLE;
                        pdfPTable.AddCell(emptCellDate);
                    }
                    if (!string.IsNullOrEmpty(o.Department))
                    {
                        pdfPTable.AddCell("Your Title and Department: ");
                        pdfPTable.AddCell(o.Department);
                    }
                    if (!string.IsNullOrEmpty(o.Description))
                    {
                        pdfPTable.AddCell("Describe your Duties: ");
                        pdfPTable.AddCell(o.Description);
                    }
                });
            }
            if (!string.IsNullOrEmpty(employee.AdditionalNotes) && !string.IsNullOrWhiteSpace(employee.AdditionalNotes))
            {
                var emptyCellHeader = new PdfPCell(new Phrase("Additonal Notes", new Font(Font.FontFamily.TIMES_ROMAN, 15, Font.BOLD)));
                emptyCellHeader.Colspan = 2;
                pdfPTable.AddCell(emptyCellHeader);
                var addtionalNotesCell = new PdfPCell(new Phrase(employee.AdditionalNotes));
                addtionalNotesCell.Colspan = 2;
                addtionalNotesCell.HorizontalAlignment = Element.ALIGN_LEFT;
                addtionalNotesCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                pdfPTable.AddCell(addtionalNotesCell);
            }
            pdfDoc.Add(pdfPTable);
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            return output;

        }
    }
}