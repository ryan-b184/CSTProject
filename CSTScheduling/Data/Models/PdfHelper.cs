using Microsoft.AspNetCore.Mvc;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CSTScheduling.Data.Models
{
    public class PdfHelper
    {
        public string ConvertToHtml(List<CISR> cisrList)
        {
            // Make a stringbuilder and append html to it
            StringBuilder sb = new StringBuilder();

            sb.Append("<table><thead>" +
                "<th>Time</th>" +
                "<th>Monday</th>" +
                "<th>Tuesday</th>" +
                "<th>Wednesday</th>" +
                "<th>Thrusday</th>" +
                "<th>Friday</th>" +
                "</thead><tbody>");

            // For each time between 8AM to 3PM
            for (int i = 8; i <= 15; i++)
            {
                // Modify time to start from 0 from noon
                sb.Append("<tr><th>" + (i > 12 ? i-12 : i) + ":00" + "</th>");
                // For each day from Monday to Friday
                for (DayOfWeek day = DayOfWeek.Monday; day < DayOfWeek.Saturday; day++)
                {
                    // checks if there is any cisr object associated with this day and time
                    var cisr = cisrList.FirstOrDefault(c => c.Day == day && c.Time == i);
                    sb.Append("<td>");
                    if (cisr != null)
                    {
                        // If cisr is not null, add cisr data
                        sb.Append("<p><b>" + (cisr.course != null ? cisr.course.courseAbbr : "") + "</b></p>" +
                        "<p>" + (cisr.room != null ? cisr.room.roomNumber : "") + "</p>" +
                        "<p>" + (cisr.primaryInstructor != null ? cisr.primaryInstructor.calcName : "") + "</p>");

                        if (cisr.secondaryInstructor != null)
                        { 
                            sb.Append("<p>" + cisr.secondaryInstructor.calcName + "</p>"); 
                        }
                    }
                    sb.Append("</td>");
                }
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table>");

            return sb.ToString();
        }            

        public byte[] GetPdfFromHtml(String html)
        {
            HtmlToPdf converter = new();

            // set converter options
            converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.MarginLeft = 35;
            converter.Options.MarginRight = 10;
            converter.Options.MarginTop = 20;
            converter.Options.MarginBottom = 20;

            // convert html string to pdf
            PdfDocument document = converter.ConvertHtmlString(html);
            // Save the pdf
            var pdf = document.Save();
            // close the pdf
            document.Close();

            return pdf;
        }
    }
}
