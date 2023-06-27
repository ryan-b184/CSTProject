using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CSTScheduling.Data.Context;
using CSTScheduling.Data.Models;
using CSTScheduling.Data.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CSTScheduling.Pages
{
    public class PdfDownloadModel : PageModel
    {
        public readonly CstScheduleDbService db;
        public readonly HttpRequest req;
        public readonly PdfHelper ph = new();
        public IConfiguration Configuration { get; }
        private string htmlHeader;

        public int semID;
        public Instructor inst = new();
        public Room rms = new();
        public List<int> insIDs = new();
        public List<int> roomIDBindables = new();
        public List<CISR> cisrList = new();

        public IList<CISR> CISR { get; set; }

        // Constructor that help us to get the url as well as set configuration for css styling
        public PdfDownloadModel(IConfiguration configuration, CstScheduleDbService dbService, IHttpContextAccessor webContext)
        {
            Configuration = configuration;
            htmlHeader = String.Format(@"<html><head><style> {0} </style></head><body>", Configuration["ReportCSS"]);
            db = dbService;
            req = webContext.HttpContext.Request;
        }

        public bool ParseQueryStrings()
        {
            // Get value of sem from url if exists and parse and assign it to semID
            if (req.Query.TryGetValue("sem", out var sem) && Int32.TryParse(sem, out semID))
            {
                // Get the value of ins from the url if exists
                if (req.Query.TryGetValue("ins", out var ins))
                {
                    // Get the comma separated distinct values and parse it and put them in insIDs list
                    insIDs = ins.First().Split(',').Select(s => Int32.TryParse(s, out var i) ? i : 0).Distinct().ToList();
                }

                // Get the value of rms from the url if exists
                if (req.Query.TryGetValue("rms", out var rms))
                {
                    // Get the comma separated distinct values and parse it and put them in rmsIDs list
                    roomIDBindables = rms.First().Split(',').Select(s => Int32.TryParse(s, out var i) ? i : 0).Distinct().ToList();
                }
            }

            // Return true if we get at least one semID and, insID or rmsID
            return semID > 0 && (insIDs.Count > 0 || roomIDBindables.Count > 0);
        }

        public IActionResult OnGet()
        {
            // Dictionary to store name of the pdf and pdf file itself as a key-value pair
            Dictionary<string, byte[]> pdfs = new();

            if (ParseQueryStrings())
            {
                foreach (int id in insIDs)
                {
                    StringBuilder sb = new();
                    sb.Append(htmlHeader);

                    // Get the instructor from the database
                    inst = db.GetInstructorsAsync(id).Result;
                    if (inst != null)
                    {
                        List<DateTime> insDates = db.GetDatesForIns(inst, semID);

                        // Call getCisr methods if we get dates for the instructor in the semester
                        if (insDates.Count > 0)
                        {                            
                            for (int i = 0; i < insDates.Count - 2; i++)
                            {
                                // Append Header of instructor name and start and end dates of the semester to the html for each page
                                sb.Append("<h1>Primary/Secondary Schedule For: " + inst.fName + " " + inst.lName +  "<span>Report Generated: " + DateTime.Today.ToShortDateString() + "</span></h1>");
                                cisrList = db.GetInsCISR(inst, insDates[i], insDates[i + 1]);
                                sb.Append("<h2>Dates: <b>" + insDates[i].ToShortDateString() + "</b> to <b>" + insDates[i + 1].ToShortDateString() + "</b></h2>");
                                sb.Append(ph.ConvertToHtml(cisrList));
                            }
                            sb.Append("</body></html>");
                            // Set name for the pdf according to the instructor first and last name
                            pdfs[inst.fName + " " + inst.lName + ".pdf"] = ph.GetPdfFromHtml(sb.ToString());
                        }
                        // If we don't get dates back for the instructor in the semester, name the pdf accordingly as invalid pdf
                        else
                        {
                            pdfs["invalid sem" + semID + ".pdf"] = ph.GetPdfFromHtml(sb.ToString());
                        }
                    }
                    // If we don't get the instructor back from the database, name the pdf accordingly as invalid pdf
                    else
                    {
                        pdfs["Invalid ins" + id + ".pdf"] = ph.GetPdfFromHtml(sb.ToString());
                    }                    
                }

                foreach (int id in roomIDBindables)
                {
                    StringBuilder sb = new();
                    sb.Append(htmlHeader);

                    // Get the room from the database
                    rms = db.GetRoomOnIdAsync(id).Result;
                    if (rms != null)
                    {
                        List<DateTime> rmsDates = db.GetDatesForRoom(rms, semID);

                        // Call getCisr methods if we get dates for the room in the semester
                        if (rmsDates.Count > 0)
                        {                            
                            for (int i = 0; i < rmsDates.Count - 2; i++)
                            {
                                // Append Header of room number and start and end dates of the semester to the html for each page
                                sb.Append("<h1>Lab Schedule For: " + rms.roomNumber + "<span>Report Generated: " + DateTime.Today.ToShortDateString() + "</span></h1>");
                                cisrList = db.GetRoomCISR(rms, rmsDates[i], rmsDates[i + 1]);
                                sb.Append("<h2>From:     " + rmsDates[i].ToShortDateString() + " to      " + rmsDates[i + 1].ToShortDateString() + "</h2>");
                                sb.Append(ph.ConvertToHtml(cisrList));
                            }
                            sb.Append("</body></html>");
                            // Set name for the pdf according to the room number
                            pdfs["Room " + rms.roomNumber + ".pdf"] = ph.GetPdfFromHtml(sb.ToString());
                        }
                        // If we don't get dates back for the room in the semester, name the pdf accordingly as invalid pdf
                        else
                        {
                            pdfs["Invalid sem" + semID + ".pdf"] = ph.GetPdfFromHtml(sb.ToString());
                        }
                    }
                    // If we don't get the room back from the database, name the pdf accordingly as invalid pdf
                    else
                    {
                        pdfs["Invalid room" + id + ".pdf"] = ph.GetPdfFromHtml(sb.ToString());
                    }
                }              
            }

            /*
             * Purpose: create a zip folder using compression method and put each pdf file into it
             */
            byte[] zipBytes;            
            using (var ms = new MemoryStream())
            {
                using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    foreach (string filename in pdfs.Keys)
                    {
                        var zipEntry = archive.CreateEntry(filename, CompressionLevel.Fastest);
                        using var zipStream = zipEntry.Open();
                        zipStream.Write(pdfs[filename]);
                    }
                }
                zipBytes = ms.ToArray();
            }
            return File(zipBytes, "application/force-download", "Schedule.zip");
        }
    }
}
