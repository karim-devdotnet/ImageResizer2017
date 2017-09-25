using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ImageResizer.Web.Controllers
{
    public class HomeController : Controller
    {
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
        public ActionResult ResizeImage()
        {
            ViewBag.Message = "Upload image";

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ResizeImage(HttpPostedFileBase imageToUpload)
        {
            if(imageToUpload != null)
            {
                //save image
                string filename = Path.GetFileNameWithoutExtension(imageToUpload.FileName);
                filename = DateTime.Now.ToString("yyyyMMddhhmmss") + filename;
                using (var image = Image.FromFile(imageToUpload.FileName))
                using (var newImage = ImageCreator.ScaleImage(image, 800, 600))
                {
                    newImage.Save(Server.MapPath("../Images/" + filename + ".jpg"), ImageFormat.Jpeg);
                    //newImage.Save(@"c:\test.png", ImageFormat.Png);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    imageToUpload.InputStream.CopyTo(ms);
                    //return File(ImageCreator.ResizeImage(ms.GetBuffer(), 800, 600),"image /jpeg");
                    return File(ImageCreator.ScaleImage(ms.GetBuffer(), 800, 600,ImageFormat.Jpeg), "image/jpeg");
                }
            }

            return View();
        }
    }
}