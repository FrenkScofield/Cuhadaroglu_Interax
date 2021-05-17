using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;

namespace CMSSite.Controllers
{
    public class BaseController : Controller
    {
        IHttpContextAccessor _httpContextAccessor;
        IHostingEnvironment _IHostingEnvironment;
        IContentPageService _IContentPageService;
        IDocumentsService _IDocumentsService;
        ISiteConfigService _ISiteConfigService;
        IUserService _IUserService;
        IFormsService _IFormsService;
        ISpecService _ISpecService;
        ISendMail _ISendMail;


        public BaseController(
       ISendMail _ISendMail,
         IHostingEnvironment _IHostingEnvironment,
          IContentPageService _IContentPageService,
        IDocumentsService _IDocumentsService,
        ISiteConfigService _ISiteConfigService,
        IHttpContextAccessor _httpContextAccessor,
         IUserService _IUserService,
         IFormsService _IFormsService,
         ISpecService _ISpecService
            )
        {
            this._ISendMail = _ISendMail;
            this._IHostingEnvironment = _IHostingEnvironment;
            this._ISiteConfigService = _ISiteConfigService;
            this._IDocumentsService = _IDocumentsService;
            this._IContentPageService = _IContentPageService;
            this._httpContextAccessor = _httpContextAccessor;
            this._IUserService = _IUserService;
            this._IFormsService = _IFormsService;
            this._ISpecService = _ISpecService;

        }


        //[Route("Search/{search?}")]
        public IActionResult Search(string search)
        {
            var langID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID");
            ViewBag.LanguageID = langID;

            setData();

            var contentPages = _IContentPageService.Where(x =>
            x.Parent != null &&
            x.ContentPageId != null &&

           (x.Name.ToLower().Contains(search.ToLower())
            || x.Description.ToLower().Contains(search.ToLower())
            || x.ContentShort.ToLower().Contains(search.ToLower())
            || x.ContentData.ToLower().Contains(search.ToLower())


            ) &&
            x.LangId == langID && x.IsDeleted == null && x.IsPublish == true && x.IsInteral == true, true, false,
                o => o.ContentPageChilds, o => o.SpecContentValue, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage).Result.ToList();

            ViewBag.contentData = contentPages;

            var Specs = _ISpecService.Where(null, true, false, o => o.Parent).Result.ToList();
            ViewBag.Specs = Specs;


            List<ContentPage> currList = contentPages.Where(x => x.IsDeleted == null).ToList();
            List<int> currSpecList = currList.SelectMany(x => x.SpecContentValue).Select(s => s.SpecId).Distinct().ToList();
            ViewBag.categories = contentPages.Where(x => x.ContentPageId == 2 && x.IsDeleted == null).ToList();
            //ViewBag.contentPages = contentPages.Where(x => currContentList.Contains(x.ContentPageId ?? 0) && x.IsDeleted == null).OrderBy(o => o.ContentOrderNo).ToList();
            ViewBag.subPages = currList;
            ViewBag.currSpecList = currSpecList;


            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult FormSave(IFormCollection postModel)
        {
            if (!string.IsNullOrEmpty(postModel["Custom3"]) && postModel["Custom3"] == "true")
            {
                Forms mF = new Forms();
                mF.NameSurname = postModel["NameSurname"];
                mF.Email = postModel["Email"];
                mF.Phone = postModel["Phone"];
                mF.Message = postModel["Message"];
                mF.IsRead = "Okunmadı";
                mF.Subject = postModel["Subject"];
                mF.FormTypeId = Convert.ToInt32(postModel["FormTypeId"]);
                var result = _IFormsService.InsertOrUpdate(mF);
                var headLine = postModel["headLine"];

                var config = _ISiteConfigService.Where(null, true, false, null).Result.FirstOrDefault();

                config.Mail = result.ResultRow.FormType.Mail ?? config.Mail;
                config.SmtpMail = config.SmtpMail;
                config.SmtpMailPass = config.SmtpMailPass;
                config.SmtpHost = config.SmtpHost;
                config.SmtpPort = "587";
                config.MailGorunenAd = config.MailGorunenAd;
                config.SmtpSSL = false;

                string[] ccListStr;
                if (string.IsNullOrEmpty(result.ResultRow.FormType.MailCC))
                {
                    ccListStr = null;
                }
                else
                {
                    ccListStr = result.ResultRow.FormType.MailCC.Split(',');

                }


                var mailStr = "<!DOCTYPE html><html><head><style>th, td {  border: 1px solid #eee;}table{max-width:800px;}td{color:#000;}</style></head><body>";

                mailStr += "<table  width'100%'>";

                //http://interalcms.zonagency.com


                mailStr += "  <tr>";
                mailStr += "    <td><strong>Konu:</strong></td>";
                mailStr += "    <td>" + mF.Subject + "</td>";
                mailStr += "  </tr>";

                mailStr += "  <tr>";
                mailStr += "    <td><strong>Ad Syod:</strong></td>";
                mailStr += "    <td>" + mF.NameSurname + "</td>";
                mailStr += "  </tr>";

                mailStr += "  <tr>";
                mailStr += "    <td><strong>E-Posta:</strong></td>";
                mailStr += "    <td>" + mF.Email + "</td>";
                mailStr += "  </tr>";

                mailStr += "  <tr>";
                mailStr += "    <td><strong>Telefon:</strong></td>";
                mailStr += "    <td>" + mF.Phone + "</td>";
                mailStr += "  </tr>";

                mailStr += "  <tr>";
                mailStr += "    <td><strong>Mesaj:</strong></td>";
                mailStr += "    <td>" + mF.Message + "</td>";
                mailStr += "  </tr>";


                mailStr += "</table></body></html>";

                string mailResult = _ISendMail.Send(new MailModelCustom { Alicilar = new string[] { config.Mail }, cc = ccListStr, Icerik = mailStr, Konu = headLine, MailGorunenAd = config.MailGorunenAd, SmtpHost = config.SmtpHost, SmtpMail = config.SmtpMail, SmtpMailPass = config.SmtpMailPass, SmtpPort = config.SmtpPort, SmtpSSL = config.SmtpSSL, SmtpUseDefaultCredentials = false });

                return Json("OK");
            }
            else
            {
                return Json("NO");
            }
        }


        public IActionResult BaseContent()
        {
            var link = HttpContext.Items["cmspage"].ToString();

            var config = _ISiteConfigService.Where().Result.FirstOrDefault();
            _httpContextAccessor.HttpContext.Session.Set("config", config);

            if (!string.IsNullOrEmpty(link))
            {
                var menu = _IContentPageService.Where(o => o.Link == link, true, false, o => o.Documents).Result.FirstOrDefault();
                if (menu != null)
                {
                    ViewBag.content = menu;
                    return View();
                }
                else
                {
                    //if (link.Contains("search"))
                    //{
                    //    return View();
                    //}
                    return Redirect(SessionRequest.config.BaseUrl);
                }
            }
            else
            {
                return Redirect(SessionRequest.config.BaseUrl);
            }
        }


        public IActionResult Validate(string user, string pass)
        {
            var _user = _IUserService.Where(o => o.IsActive == true && o.IsApproved == true && (o.Mail1 == user || o.UserName == user || o.Name == user) && (o.Pass == pass), true, false).Result.FirstOrDefault();
            if (_user != null)
            {
                _user.LoginCount = _user.LoginCount == null ? null : _user.LoginCount++;
                _httpContextAccessor.HttpContext.Session.Set("fUser", _user);
                ViewBag.User = _user;
            }
            return Json(_user);
        }

        public IActionResult Create()
        {
            setData();
            return View();
        }

        public IActionResult Recover(string token)
        {
            setData();
            var row = _IUserService.Where(o => o.UserToken == token).Result.FirstOrDefault();

            if (row != null)
            {
                UserModel um = new UserModel();
                um.Name = row.Name;
                um.Surname = row.Surname;
                um.Mail1 = row.Mail1;
                um.UserToken = row.UserToken;
                ViewBag.UserRecover = um;
            }
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult UserRecover(IFormCollection postModel)
        {
            Guid guidOutput = new Guid();
            bool isValid = Guid.TryParse(postModel["userToken"], out guidOutput);
            if (isValid)
            {
                var row = _IUserService.Where(o => o.UserToken == postModel["userToken"].ToString()).Result.FirstOrDefault();
                if (row != null)
                {
                    row.UserToken = "";
                    row.Pass = postModel["Pass"];
                    _IUserService.InsertOrUpdate(row);
                    ViewBag.UserRecover = row;

                    return Json("OK");
                }
                else
                {
                    return Json("NOK");
                }
            }
            else
            {
                return Json("NOK");
            }
            return View();
        }

        public IActionResult UserCreate(User postModel)
        {
            var row = _IUserService.Where(o => o.UserName == postModel.UserName).Result.FirstOrDefault();


            if (row != null)
            {
                return Json("Duplicate");
            }
            else
            {
                postModel.IsActive = true;
                postModel.IsApproved = false;
                var rs = _IUserService.InsertOrUpdate(postModel);
                var config = _ISiteConfigService.Where(null, true, false, null).Result.FirstOrDefault();

                config.Mail = config.Mail;

                config.SmtpMail = config.SmtpMail;
                config.SmtpMailPass = config.SmtpMailPass;
                config.SmtpHost = config.SmtpHost;
                config.SmtpPort = "587";
                config.MailGorunenAd = config.MailGorunenAd;
                config.SmtpSSL = false;


                //string[] ccListStr = { "ali_senatli@cuhadaroglu.com", "saniye.aktas@hybrid.rocks" };

                var mailStr = "<!DOCTYPE html><html><head><style>th, td {  border: 1px solid #eee;}table{max-width:800px;}td{color:#000;}</style></head><body>";

                mailStr += "<table  width'100%'>";

                //http://interalcms.zonagency.com
                mailStr += "  <tr>";
                mailStr += "    <td>Yeni bir kullanıcı üye oldu, aşağıdaki linkten tıklayarak, üyelik bilgileirni kontrol edip, üyeliği onaylayabilirsiniz.</td>";
                mailStr += "    <td></td>";
                mailStr += "  </tr>";
                mailStr += "  <tr>";
                mailStr += "    <td><a target='_blank' href='" + config.ImageUrl + "/User/InsertOrUpdatePage?id=" + rs.ResultRow.Id + "'>Yeni Üye</a></td>";
                mailStr += "  </tr>";
                mailStr += "</table></body></html>";

                string result = _ISendMail.Send(new MailModelCustom { Alicilar = new string[] { config.Mail }, cc = null, Icerik = mailStr, Konu = "Yeni bir üyelik var!", MailGorunenAd = config.MailGorunenAd, SmtpHost = config.SmtpHost, SmtpMail = config.SmtpMail, SmtpMailPass = config.SmtpMailPass, SmtpPort = config.SmtpPort, SmtpSSL = config.SmtpSSL, SmtpUseDefaultCredentials = false });

                return Json(rs);
            }
        }
        public IActionResult UserForgot(string mail)
        {
            var row = _IUserService.Where(o => o.UserName == mail).Result.FirstOrDefault();
            if (row != null)
            {
                try
                {
                    var guid = Guid.NewGuid().ToString();
                    row.UserToken = guid;

                    _IUserService.InsertOrUpdate(row);


                    var config = _ISiteConfigService.Where(null, true, false, null).Result.FirstOrDefault();
                    config.Mail = row.UserName;
                    config.SmtpMail = config.SmtpMail;
                    config.SmtpMailPass = config.SmtpMailPass;
                    config.SmtpHost = config.SmtpHost;
                    config.SmtpPort = "587";
                    config.MailGorunenAd = config.MailGorunenAd;
                    config.SmtpSSL = false;

                    //   config.Mail = row.Mail1;
                    //   config.SmtpMail = "admin@hybro.systems";
                    //   config.SmtpMailPass = "@sK5ng8=mY3B=E7-";
                    //   config.SmtpHost = "mail.hybro.systems";
                    //   config.SmtpPort = "587";
                    //   config.MailGorunenAd = "admin@hybro.systems";
                    //   config.SmtpSSL = false;

                    //  string[] ccListStr = { "ali_senatli@cuhadaroglu.com", "saniye.aktas@hybrid.rocks" };
                    var mailStr = "<!DOCTYPE html><html><head><style>th, td {  border: 1px solid #eee;}table{max-width:800px;}td{color:#000;}</style></head><body>";

                    mailStr += "<table  width'100%'>";
                    //http://interalcms.zonagency.com
                    mailStr += "  <tr>";
                    mailStr += "    <td><strong>Ad Soyad:</strong></td>";
                    mailStr += "    <td>" + row.Name + " " + row.Surname + "</td>";
                    mailStr += "  </tr>";
                    mailStr += "  <tr>";
                    mailStr += "    <td><strong>Sıfırlama Linki:</strong></td>";
                    mailStr += "    <td><a target='_blank' href='" + config.BaseUrl + "/Base/Recover?token=" + guid + "'>Şifreyi sıfırla.</a></td>";
                    mailStr += "  </tr>";
                    mailStr += "</table></body></html>";

                    string result = _ISendMail.Send(new MailModelCustom { Alicilar = new string[] { mail }, cc = null, Icerik = mailStr, Konu = "Şifremi Unuttum", MailGorunenAd = config.MailGorunenAd, SmtpHost = config.SmtpHost, SmtpMail = config.SmtpMail, SmtpMailPass = config.SmtpMailPass, SmtpPort = config.SmtpPort, SmtpSSL = config.SmtpSSL, SmtpUseDefaultCredentials = false });

                    return Json("OK");
                }
                catch (Exception ex)
                {
                    return Json("NOK");
                }


            }
            else
            {

                return Json("NOK");
            }
        }

        public IActionResult Login()
        {
            if (_httpContextAccessor.HttpContext.Session.Get("fUser") != null)
            {
                return Redirect("/");
            }
            else
            {
                setData();
                return View();
            }

        }
        public IActionResult Index()
        {

            setData();
            return View();
        }
        public IActionResult Content()
        {
            var link = HttpContext.Items["cmspage"].ToString();
            var config = _ISiteConfigService.Where().Result.FirstOrDefault();
            _httpContextAccessor.HttpContext.Session.Set("config", config);
            if (!string.IsNullOrEmpty(link))
            {
                var menu = _IContentPageService.Where(o => o.Link == link, true, false, o => o.Documents, o => o.TemplateType).Result.FirstOrDefault();
                if (menu != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", menu.LangId);
                    ViewBag.LanguageID = menu.LangId;
                    ViewBag.page = menu;
                    return View();
                }
                else
                {
                    int langID = 0;
                    if (_httpContextAccessor.HttpContext.Session.GetString("LanguageID") != null)
                    {
                        langID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") ?? 1;
                    }
                    if (_httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") == null || langID == 0)
                    {
                        _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", 1);
                    }
                    ViewBag.LanguageID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID");
                    return Redirect(SessionRequest.config.BaseUrl);
                }
            }
            else
            {
                return Redirect(SessionRequest.config.BaseUrl);
            }
        }
        public IActionResult SideContent(string link)
        {
            getPageData(link);
            return View();
        }
        private List<ContentPage> getPageData(string link)
        {
            var fullLink = HttpContext.Request.Path.Value.Trim().ToStr();
            int langID = 0;
            if (_httpContextAccessor.HttpContext.Session.GetString("LanguageID") != null)
            {
                langID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") ?? 1;
            }
            if (_httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") == null || langID == 0)
            {
                langID = 1;
                _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", 1);
            }

            var contentPages = _IContentPageService.Where(o => o.LangId == langID, true, false).Result.ToList();

            ViewBag.contentPages = contentPages.ToList();
            ViewBag.Pages = contentPages.ToList();
            var config = _ISiteConfigService.Where().Result.FirstOrDefault();
            _httpContextAccessor.HttpContext.Session.Set("config", config);
            if (!string.IsNullOrEmpty(link))
            {
                var menu = _IContentPageService.Where(o => o.Link == link, true, false).Result.FirstOrDefault();
                if (menu != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", menu.LangId);
                    ViewBag.page = menu;
                }
                else
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", 1);
                    Redirect(SessionRequest.config.BaseUrl);
                }
                ViewBag.LanguageID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID");
            }
            else
            {
                Redirect(SessionRequest.config.BaseUrl);
            }
            return contentPages;
        }
        public IActionResult Header()
        {
            #region dynamicContent
            int langID = 0;
            if (_httpContextAccessor.HttpContext.Session.GetString("LanguageID") != null)
            {
                langID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") ?? 1;
            }
            if (_httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") == null || langID == 0)
            {
                langID = 1;
                _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", 1);
            }
            var contentPages = _IContentPageService.Where(o => o.LangId == langID, true, false, o => o.ContentPageChilds, o => o.Documents).Result.ToList();
            contentPages.ForEach(o =>
            {
                o.ContentPageChilds = contentPages.Where(oo => oo.ContentPageId == o.Id).ToList();
            });
            ViewBag.contentPages = contentPages;
            //   ViewBag.fikirler = _IFikirService.Where(o => o.FikirStatus != FikirDurumu.Ondegerlendirme,
            //      true, false,
            //      o => o.AtananDepartman, o => o.AtananKullanici, o => o.FikirBegen).Result.ToList();
            var config = _ISiteConfigService.Where().Result.FirstOrDefault();
            _httpContextAccessor.HttpContext.Session.Set("config", config);

            var _user = _IUserService.Where(o => (o.Name == "admin"), true, false).Result.FirstOrDefault();

            _httpContextAccessor.HttpContext.Session.Set("_user", _user);

            #endregion
            return View();
        }
        public IActionResult Footer()
        {
            return View();
        }
        public void setData()
        {
            #region dynamicContent
            var link = HttpContext.Request.Path.Value.Trim().ToStr();
            List<ContentPage> contentPages = new List<ContentPage>();

            if (_httpContextAccessor.HttpContext.Session.Get("fUser") != null)
            {
                ViewBag.User = _httpContextAccessor.HttpContext.Session.Get("fUser");
            }
            else
            {
                ViewBag.User = null;
            }



            int langID = 0;
            if (_httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") != null)
            {
                langID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") ?? 1;
            }

            if (_httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") == null || langID == 0)
            {
                langID = 1;
                _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", 1);
            }

            var menu = _IContentPageService.Where(o => o.Link == link, true, false, null).Result.FirstOrDefault();
            if (menu != null)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", menu.LangId);
                ViewBag.LanguageID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID");
            }
            else
            {
                if (link == "/" || link == "/Base/Index")
                {
                    if (_httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") != null)
                    {
                        ViewBag.LanguageID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") ?? 1;
                    }
                    else
                    {
                        ViewBag.LanguageID = 1;
                    }

                }
                else
                {
                    _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", 1);
                    ViewBag.LanguageID = _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID");
                }
            }

            Regex reg = new Regex("[*'\",_&#^@]");

            var currState = reg.Replace(_httpContextAccessor.HttpContext.Session.GetString("currState") ?? "-", string.Empty);

            ViewBag.currState = currState;
            bool isBayi = false;
            bool isEndustri = false;
            bool isMimar = false;
            bool isBireysel = false;




            contentPages = _IContentPageService.Where(x => x.LangId == langID && x.IsDeleted == null && x.IsPublish == true && x.IsInteral == true, true, false, o => o.ContentPageChilds, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage).Result.ToList();
            _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages);
            //if (_httpContextAccessor.HttpContext.Session.Get("contentPages") == null)
            //{
            //    contentPages = _IContentPageService.Where(null, true, false, o => o.ContentPageChilds, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Image, o => o.BannerImage).Result.ToList();
            //    _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages);
            //}
            //else
            //{
            //    contentPages =
            //    _httpContextAccessor.HttpContext.Session.Get<List<ContentPage>>("contentPages");
            //    _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages);
            //}
            // ViewBag.IsHeaderMenu = contentPages.Where(o => o.IsHeaderMenu == true).OrderBy(o => o.ContentOrderNo).ThenBy(o => o.Name).ToList();
            // ViewBag.IsFooterMenu = contentPages.Where(o => o.IsFooterMenu == true).OrderBy(o => o.ContentOrderNo).ThenBy(o => o.Name).ToList();
            switch (currState)
            {
                case "Uygulayıcı":
                    isBayi = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsBayi == isBayi).ToList();
                    break;
                case "Endüstriyel":
                    isEndustri = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsEndustri == isEndustri).ToList();
                    break;
                case "Mimarlar":
                    isMimar = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsMimar == isMimar).ToList();
                    break;
                case "Bireysel":
                    isBireysel = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsBireysel == isBireysel).ToList();
                    break;
                case "-":
                    isMimar = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsMimar == isMimar).ToList();
                    break;
                default:
                    isMimar = true;
                    break;
            }


            ViewBag.Pages = contentPages.ToList();
            //   ViewBag.fikirler = _IFikirService.Where(o => o.FikirStatus != FikirDurumu.Ondegerlendirme,
            //      true, false,
            //      o => o.AtananDepartman, o => o.AtananKullanici, o => o.FikirBegen).Result.ToList();



            var config = _ISiteConfigService.Where().Result.FirstOrDefault();
            _httpContextAccessor.HttpContext.Session.Set("config", config);
            var _user = _IUserService.Where(o => (o.Name == "admin"), true, false).Result.FirstOrDefault();
            _httpContextAccessor.HttpContext.Session.Set("_user", _user);
            #endregion
        }
        public IActionResult GetTemplateType()
        {
            var list = Enum.GetValues(typeof(TemplateType)).Cast<int>().Select(x => new { value = x.ToString(), text = ((TemplateType)x).ExGetDescription() }).ToArray();
            return Json(list);
        }

        [Route("profil")]
        public IActionResult profil()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangeType(string ID)
        {
            string currState = "-";
            if (_httpContextAccessor.HttpContext.Session.GetString("currState") != null)
            {
                currState = _httpContextAccessor.HttpContext.Session.GetString("currState");
            }
            //_httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages);   
            switch (ID)
            {
                case "Uygulayıcı":
                    currState = "Uygulayıcı";
                    break;
                case "Endüstriyel":
                    currState = "Endüstriyel";
                    break;
                case "Mimarlar":
                    currState = "Mimarlar";
                    break;
                case "Bireysel":
                    currState = "Bireysel";
                    break;
                default:
                    currState = "-";
                    break;
            }
            ViewBag.currState = currState;
            _httpContextAccessor.HttpContext.Session.Set("currState", currState);
            return Json("");
        }
        [HttpPost]
        public IActionResult ChangeLanguage(string LanguageID, string PageUrl, string PageData)
        {
            int SiteLanguage = Convert.ToInt32(LanguageID);
            _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", SiteLanguage);
            var contentPages = _IContentPageService.Where(x => x.LangId == SiteLanguage, true, false).Result.ToList();
            _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages);

            ContentPage OldPage = _IContentPageService.Where(x => x.IsDeleted == null && x.Link == PageUrl, true, false, null).Result.FirstOrDefault();
            if (OldPage != null)
            {
                int orgID = OldPage.OrjId ?? 0;
                ContentPage NewPage = _IContentPageService.Where(x => x.IsDeleted == null && x.LangId == SiteLanguage && x.OrjId == orgID, true, false, null).Result.FirstOrDefault();
                if (NewPage == null)
                {
                    return Json("/");
                }
                else
                {
                    return Json(NewPage.Link);
                }
            }
            else
            {
                return Json("");
            }
            //_IContentPageService.Where(o => o.Link == link, true, false, null).Result.FirstOrDefault(); 
        }

        public IActionResult Logout()
        {

            var currState = _httpContextAccessor.HttpContext.Session.Get("currState");
            _httpContextAccessor.HttpContext.Session.Clear();
            _httpContextAccessor.HttpContext.Session.Set("currState", currState);
            return Redirect("/");
        }


        public IActionResult SendMail(IFormCollection form, string type)
        {

            var config = _ISiteConfigService.Where(null, true, false, null).Result.FirstOrDefault();

            config.Mail = config.Mail;
            config.SmtpMail = config.SmtpMail;
            config.SmtpMailPass = config.SmtpMailPass;
            config.SmtpHost = config.SmtpHost;
            config.SmtpPort = "587";
            config.MailGorunenAd = config.MailGorunenAd;
            config.SmtpSSL = false;

            long size = 0;
            string mailContent = "";
            string links = "";
            var test = "";


            foreach (var item in form)
            {
                if (item.Key != "subject" && item.Key != "CheckPolicy" && item.Key != "__RequestVerificationToken" && item.Key != "formType" && item.Key != "emailR" && item.Key != "recaptcha" && item.Key.Trim() != "")
                {
                    mailContent += "  <tr>                                                  ";
                    mailContent += "<td><b>" + item.Key + "&nbsp;:&nbsp;</b></td>";
                    mailContent += "<td>" + item.Value + "</td>";
                    mailContent += "  </tr>                                                   ";
                }

            }

            var mailStr = "<!DOCTYPE html><html><head><style>th, td {  border: 1px solid #eee;}table{max-width:800px;}td{color:#000;}</style></head><body>";

            mailStr += "<table  width'100%'>";
            var keyStr = "";
            var keyVal = "";
            string ccMail = "";
            foreach (var item in form)
            {
                mailStr += "  <tr>";
                mailStr += "    <td><strong>" + keyStr + ":</strong></td>";
                mailStr += "    <td>" + item.Value + "</td>";
                mailStr += "  </tr>";
            }
            mailStr += links;
            mailStr += "</table></body></html>";

            //
            string result = _ISendMail.Send(new MailModelCustom { Alicilar = new string[] { config.Mail }, cc = null, Icerik = mailStr, Konu = "", MailGorunenAd = config.MailGorunenAd, SmtpHost = config.SmtpHost, SmtpMail = config.SmtpMail, SmtpMailPass = config.SmtpMailPass, SmtpPort = config.SmtpPort, SmtpSSL = config.SmtpSSL, SmtpUseDefaultCredentials = true });
            return Json(result);

        }


    }
}
//web@cuhadaroglu.com