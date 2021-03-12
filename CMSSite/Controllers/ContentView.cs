using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace CMSSite.Components
{

    public class ContentView : ViewComponent
    {

        IHttpContextAccessor _httpContextAccessor;
        IContentPageService _IContentPageService;
        ISpecService _ISpecService;
        IDocumentsService _IDocumentsService;
        public ContentView(
            IContentPageService _IContentPageService,
            ISpecService _ISpecService,
        IDocumentsService _IDocumentsService,
        IHttpContextAccessor httpContextAccessor
            )
        {
            this._IContentPageService = _IContentPageService;
            this._ISpecService = _ISpecService;
            this._httpContextAccessor = httpContextAccessor;
            this._IDocumentsService = _IDocumentsService;
        }

        public IViewComponentResult Invoke(TemplateType TemplateType)
        {
            var link = HttpContext.Request.Path.Value.Trim('/').ToStr();

            if (_httpContextAccessor.HttpContext.Session.Get("fUser") != null)
            {
                User currUser = _httpContextAccessor.HttpContext.Session.Get<User>("fUser");
                ViewBag.User = currUser;
            }
            else
            {
                ViewBag.User = null;
            }

            List<ContentPage> contentPages = new List<ContentPage>();
            var content = _IContentPageService.Where(o => o.Link.ToLower() == link.ToLower() && o.IsPublish == true, true, false, o => o.ContentPageChilds, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.TechnicalProperties, o => o.TechnicalDocuments, o => o.CadDatas, o => o.BIMFiles, o => o.ThumbImage, o => o.Picture, o => o.BannerImage, o => o.SpecContentValue).Result.FirstOrDefault();
            var Specs = _ISpecService.Where(null, true, false, o => o.Parent).Result.ToList();
            ViewBag.Specs = Specs;
            int langID = content != null ? content.LangId : 1;
            if (content != null && _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") != content.LangId)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", content.LangId);
                langID = content.LangId;
            }
            Regex reg = new Regex("[*'\",_&#^@]");

            var currState = reg.Replace(_httpContextAccessor.HttpContext.Session.GetString("currState") ?? "-", string.Empty);


            ViewBag.currState = currState;
            bool isBayi = false;
            bool isEndustri = false;
            bool isMimar = false;
            bool isBireysel = false;



            contentPages = _IContentPageService.Where(x => x.LangId == langID && x.IsDeleted == null && x.IsPublish == true && x.IsInteral == true, true, false, o => o.ContentPageChilds, o => o.SpecContentValue, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage).Result.ToList();
            // contentPages = _IContentPageService.Where(o => o.LangId == langID, true, false, o => o.ContentPageChilds, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage,o=>o.SpecContentValue).Result.OrderBy(o => o.ContentOrderNo).ToList();

            _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages.Where(o => o.LangId == langID));
            ViewBag.contentPages = contentPages;


            ViewBag.content = content;
            ViewBag.LanguageID = langID;

            ViewBag.Pages = contentPages.ToList();
            switch (currState)
            {
                case "Bayi":
                    isBayi = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsBayi == isBayi).ToList();
                    break;
                case "Endustri":
                    isEndustri = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsEndustri == isEndustri).ToList();
                    break;
                case "Mimar":
                    isMimar = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsMimar == isMimar).ToList();
                    break;
                case "Bireysel":
                    isBireysel = true;
                    ViewBag.contentPages = contentPages.Where(x => x.IsBireysel == isBireysel).ToList();
                    break;
                default:
                    isMimar = true;
                    break;
            }


            if (TemplateType == TemplateType.ProjeListeleme)
            {
                List<int> currContentList = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).Select(x => x.Id).ToList();
                ViewBag.subPages = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).ToList();
            }
            else if (TemplateType == TemplateType.BlogListeleme)
            {
                ViewBag.subPages = contentPages.Where(x => x.ContentPageId == 16).ToList();
            }
            else if (TemplateType == TemplateType.UrunListeleme)
            {
                
                List<ContentPage> currList = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).ToList();
                List<int> currSpecList = currList.SelectMany(x => x.SpecContentValue).Select(s => s.SpecId).Distinct().ToList();
                ViewBag.categories = contentPages.Where(x => x.ContentPageId == content.Parent.Id && x.IsDeleted == null).ToList();
                //ViewBag.contentPages = contentPages.Where(x => currContentList.Contains(x.ContentPageId ?? 0) && x.IsDeleted == null).OrderBy(o => o.ContentOrderNo).ToList();
                ViewBag.subPages = currList;
                ViewBag.currSpecList = currSpecList;

                //select Distinct SpecId from SpecContentValue
            }
            return View(TemplateType.ToString());
        }
    }
}