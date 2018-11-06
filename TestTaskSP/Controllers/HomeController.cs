using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestTaskSP.Models;

namespace TestTaskSP.Controllers
{
    public class HomeController : Controller
    {
        FileCotext db = new FileCotext();

        public ActionResult Index()
        {
            return View(db.FIleInformations);
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

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(FileInformation file, HttpPostedFileBase uploadFile)
        {
            if (ModelState.IsValid && uploadFile != null)
            {
                string temp = "";
                string[] arrWord;
                file.DateDownload = DateTime.Now;
                file.Path = Server.MapPath("~/Files");
                var dir = new DirectoryInfo(file.Path);
                file.FileName = uploadFile.FileName;
                //Convert.ToString(dir.EnumerateFiles().Select(c => c.Name));
                file.Size = uploadFile.ContentLength;
                file.Type = uploadFile.ContentType;

                file.Ip = Request.UserHostAddress;
                file.UserName = Request.UserHostName;


                StreamReader sr = new StreamReader(Path.Combine(file.Path, uploadFile.FileName));

                file.CountSymbols = sr.ReadToEnd().Length;
                while (sr.EndOfStream != true)
                {
                    temp = sr.ReadToEnd();
                }
                arrWord = temp.Split(' ');

                file.CountWords = arrWord.Length;

                db.FIleInformations.Add(file);
                db.SaveChanges();

                return RedirectToAction("UploadFile");
            }
            return View(file);
        }
    }
}