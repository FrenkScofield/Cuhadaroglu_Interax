using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CMS.Controllers
{
    public class BaseController : Controller
    {
        ILangService _ILangService;
        IDocumentsService _IDocumentsService;

        IUserService _IUserService;
#pragma warning disable CS0618 // 'IHostingEnvironment' artık kullanılmıyor: 'This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.AspNetCore.Hosting.IWebHostEnvironment.'
        IHostingEnvironment _IHostingEnvironment;
#pragma warning restore CS0618 // 'IHostingEnvironment' artık kullanılmıyor: 'This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.AspNetCore.Hosting.IWebHostEnvironment.'
        IServiceConfigService _IServiceConfigService;
        IHttpContextAccessor _IHttpContextAccessor;

        public BaseController(
#pragma warning disable CS0618 // 'IHostingEnvironment' artık kullanılmıyor: 'This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.AspNetCore.Hosting.IWebHostEnvironment.'
            IHostingEnvironment _IHostingEnvironment,
#pragma warning restore CS0618 // 'IHostingEnvironment' artık kullanılmıyor: 'This type is obsolete and will be removed in a future version. The recommended alternative is Microsoft.AspNetCore.Hosting.IWebHostEnvironment.'
            IServiceConfigService _IServiceConfigService,
            IHttpContextAccessor _IHttpContextAccessor,
            ILangService _ILangService,
            IUserService _IUserService,
            IDocumentsService _IDocumentsService
            )
        {
            this._ILangService = _ILangService;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._IServiceConfigService = _IServiceConfigService;
            this._IHttpContextAccessor = _IHttpContextAccessor;
            this._IUserService = _IUserService;
            this._IDocumentsService = _IDocumentsService;
        }


        public IActionResult Index()
        {
            if (SessionRequest._User == null)
            {
                return RedirectToAction("Login1", "Login");
            }
            ViewBag.pageTitle = "Dashboard";
            var menus = _IServiceConfigService.Where().Result.ToList();
            var menuler = new List<string>();
            try
            {
                var filePath = _IHostingEnvironment.ContentRootPath + @"\Views";
                //menuler = Directory.EnumerateFiles(filePath, "*", SearchOption.AllDirectories).Select(o =>
                //o.Split("\\")[8].ToStr()

                //).Where(o =>
                //!o.ToStr().Contains("Base")
                //&& !o.ToStr().Contains("Shared")
                //&& !o.ToStr().Contains("Login")
                //&& !o.ToStr().Contains("_")
                //).Distinct().OrderBy(o => o).ToList();
            }
#pragma warning disable CS0168 // ex' değişkeni ifade edilir ancak hiçbir zaman kullanılmaz
            catch (Exception ex)
#pragma warning restore CS0168 // ex' değişkeni ifade edilir ancak hiçbir zaman kullanılmaz
            {
                //throw ex;
            }
            //var fark = menuler.Where(oo => !menus.Select(o => o.Name).Contains(oo)).ToList(); 
            //if (fark.Count > 0)
            //{ 
            //    fark.ForEach(o =>
            //    {
            //        menus.Add(new ServiceConfig()
            //        {
            //            Name = o,
            //            Description = o,
            //            Url = "/" + o,
            //            ServiceName = o
            //        }); 
            //    });
            //    _IServiceConfigService.AddBulk(menus);
            //    _IServiceConfigService.SaveChanges(); 
            //} 
            _IHttpContextAccessor.HttpContext.Session.Set("menus", menus);
            return View();
        }

        public IActionResult Logout()
        {
            _IHttpContextAccessor.HttpContext.Session.Clear();
            return RedirectToAction("Login1", "Login");
        }

        public IActionResult Report()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetAuth(string token, string filename)
        {
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(filename))
            {

                User currUser = _IUserService.Where(x => x.UserDocToken == token && x.IsDeleted == null && x.IsActive == true).Result.FirstOrDefault();
                if (currUser != null)
                {

                    // var encodedLocationName = WebUtility.UrlEncode(filename);
                    // var encodedLocationName2 = HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8);
                    var encodedUri = Uri.EscapeUriString(filename);
                    currUser.UserDocToken = "";
                    _IUserService.InsertOrUpdate(currUser);
                    _IUserService.SaveChanges();

                    _IHttpContextAccessor.HttpContext.Session.Set("_docuser", currUser);
                    return Redirect("/CadFiles/" + encodedUri);
                    //TODO redirect to file
                }
                else
                {
                    return Json("Auth");
                }
            }
            else
            {
                return Json("Parameter");
            }

        }
      public class filesModel
      {
          public string token { get; set; }
          //public int[] files { set; get; }
          public string files { set; get; }
      
      }

        [HttpPost]
        public async Task<IActionResult> DownloadZip(filesModel _filesModel)
        {
            string token = _filesModel.token;
            string files = _filesModel.files;
            var currUser = _IUserService.Where(x => x.UserDocToken == token && x.IsDeleted == null && x.IsActive == true).Result.FirstOrDefault();
            if (currUser != null)
            {
                _IHttpContextAccessor.HttpContext.Session.Set("_docuser", currUser);

                if (files != "")
                {
                    List<int> fileArr = new List<int>();
                    if (files.IndexOf(",") > -1)
                    {
                        fileArr = files.Split(',').Select(Int32.Parse).ToList();
                    }
                    else
                    {
                        fileArr.Add(Convert.ToInt32(files));
                    }

                    List<Documents> docsToDown = _IDocumentsService.Where(x => fileArr.Contains(x.Id) && x.IsDeleted == null).Result.ToList();
                    List<ZipDocs> myZipFile = new List<ZipDocs>();
                    string fileNamePure = "";
                    List<string> myArr = new List<string>();
                    myArr.Add(Directory.GetCurrentDirectory());
                    myArr.Add("wwwroot\\fileupload\\UserFiles\\Folders");
                    //{ Directory.GetCurrentDirectory(),, "fileupload", "UserFiles", "Folders" };
                    string[] myArrTemp;
                    string rootPath = "";
                    foreach (var item in docsToDown)
                    {

                        if (item.Link.Contains('/'))
                        {
                            myArrTemp = item.Link.Split('/');
                            for (int i = 0; i < myArrTemp.Length; i++)
                            {
                                myArr.Add(myArrTemp[i]);
                            }

                        }
                        else
                        {
                            myArr.Append(item.Link);
                        }


                        if (myArr.Count() > 2)
                        {
                            var path = Path.Combine(myArr.ToArray());

                            var memory = new MemoryStream();

                            var str = path;
                           
                            using (var stream = new FileStream(path, FileMode.Open, 
            FileAccess.ReadWrite))
                            {
                                await stream.CopyToAsync(memory);
                            }
                            memory.Position = 0;

                            ZipDocs myFile = new ZipDocs(item.Name+".dwg", memory);
                            myZipFile.Add(myFile);
                            myArr.RemoveRange(2, myArr.Count() - 2);
                        }

                    }

                    //return File(memory, GetContentType(path), Path.GetFileName(path));
                    //return File(Helpers.Zipper.Zip(myZipFile), "application/zip", "myZipFile.zip");
                    return File(Helpers.Zipper.Zip(myZipFile), "application/octet-stream", "myZipFile.zip");
                }
                else
                {
                    return Json("NOK");
                }
            }
            else
            {
                return Json("NOK1");
            }


        }
        //[HttpPost]
        //public async Task<IActionResult> DownloadZip(filesModel _filesModel)
        //{
        //    var currUser = _IUserService.Where(x => x.UserDocToken == _filesModel.token && x.IsDeleted == null && x.IsActive == true).Result.FirstOrDefault();
        //    if (currUser != null)
        //    {
        //        _IHttpContextAccessor.HttpContext.Session.Set("_docuser", currUser);

        //        if (_filesModel.files.Count() > 0)
        //        {
        //            List<Documents> docsToDown = _IDocumentsService.Where(x => _filesModel.files.Contains(x.Id) && x.IsDeleted == null).Result.ToList();
        //            List<ZipDocs> myZipFile = new List<ZipDocs>();
        //            string fileNamePure = "";
        //            List<string> myArr = new List<string>();
        //            myArr.Add(Directory.GetCurrentDirectory());
        //            myArr.Add("wwwroot\\fileupload\\UserFiles\\Folders");
        //            //{ Directory.GetCurrentDirectory(),, "fileupload", "UserFiles", "Folders" };
        //            string[] myArrTemp;
        //            string rootPath = "";
        //            foreach (var item in docsToDown)
        //            {

        //                if (item.Link.Contains('/'))
        //                {
        //                    myArrTemp = item.Link.Split('/');
        //                    for (int i = 0; i < myArrTemp.Length; i++)
        //                    {
        //                        myArr.Add(myArrTemp[i]);
        //                    }

        //                }
        //                else
        //                {
        //                    myArr.Append(item.Link);
        //                }


        //                if (myArr.Count() > 2)
        //                {
        //                    var path = Path.Combine(myArr.ToArray());

        //                    var memory = new MemoryStream();

        //                    var str = path;
        //                    myArr.RemoveRange(2, myArr.Count() - 2);
        //                    using (var stream = new FileStream(path, FileMode.Open))
        //                    {
        //                        await stream.CopyToAsync(memory);
        //                    }
        //                    memory.Position = 0;

        //                    ZipDocs myZipFilae = new ZipDocs(item.Name, memory);
        //                    myZipFile.Add(myZipFilae);
        //                }

        //            }
        //            //return File(memory, GetContentType(path), Path.GetFileName(path));
        //            return File(Helpers.Zipper.Zip(myZipFile), "application/octet-stream");
        //        }
        //        else
        //        {
        //            return Json("NOK");
        //        }

        //    }
        //    else
        //    {
        //        return Json("NOK1");
        //    }


        //}

    }
}
