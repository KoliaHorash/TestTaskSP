using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
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
            return View(db.FileInformations);
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
                var data = new byte[uploadFile.ContentLength];
                uploadFile.InputStream.Read(data, 0, uploadFile.ContentLength);
                using (var sw = new FileStream(Path.Combine(Server.MapPath("~/Files/"), uploadFile.FileName), FileMode.Create))
                {
                    sw.Write(data, 0, data.Length);
                }

                string temp = "";
                string[] arrWord;
                file.DateDownload = DateTime.Now;
                file.Path = Server.MapPath("~/Files");
                var dir = new DirectoryInfo(file.Path);
                file.FileName = uploadFile.FileName.Substring(0, uploadFile.FileName.IndexOf('.'));

                file.Size = uploadFile.ContentLength;
                file.Type = uploadFile.FileName.Substring(uploadFile.FileName.IndexOf('.') + 1);

                file.Ip = Request.UserHostAddress;

                file.CountSymbols = CountDate("symbols", file.Path, uploadFile.FileName);
                file.CountWords = CountDate("words", file.Path, uploadFile.FileName);

                
                db.FileInformations.Add(file);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(file);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FileInformation fileInformation = db.FileInformations.Find(id);
            if (fileInformation == null)
            {
                return HttpNotFound();
            }
            return View(fileInformation);
        }

        [HttpPost]
        public ActionResult Edit(FileInformation fileInformation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(fileInformation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(fileInformation);
        }

        protected int CountDate(string key, string path, string fileName)
        {
            string temp = "";
            string[] arrWord;
            StreamReader sr = new StreamReader(Path.Combine(path, fileName));

            if (0 == String.Compare(key, "symbols"))
            {
                int countSymbols = sr.ReadToEnd().Length;
                sr.Close();
                return countSymbols;
            }
            else if (0 == String.Compare(key, "words"))
            {
                int countWords;
                while (sr.EndOfStream != true)
                {
                    temp = sr.ReadToEnd();
                }
                arrWord = temp.Split(' ');
                countWords = arrWord.Length;
                sr.Close();
                return countWords;
            }
            else
                return 0;
        }
    }
}