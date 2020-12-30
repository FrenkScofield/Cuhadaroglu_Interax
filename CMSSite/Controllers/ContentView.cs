using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMSSite.Components
{

    public class ContentView : ViewComponent
    {

        IHttpContextAccessor _httpContextAccessor;
        IContentPageService _IContentPageService;
        IDocumentsService _IDocumentsService;
        public ContentView(
            IContentPageService _IContentPageService,
            IDocumentsService _IDocumentsService,
        IHttpContextAccessor httpContextAccessor
            )
        {
            this._IContentPageService = _IContentPageService;
            this._httpContextAccessor = httpContextAccessor;
            this._IDocumentsService = _IDocumentsService;
        }

        public IViewComponentResult Invoke(TemplateType TemplateType)
        {
            var link = HttpContext.Request.Path.Value.Trim('/').ToStr();

            List<ContentPage> contentPages = new List<ContentPage>();
            var content = _IContentPageService.Where(o => o.Link.ToLower() == link.ToLower()  , true, false, o => o.ContentPageChilds, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage).Result.FirstOrDefault();
            int langID = content != null ? content.LangId : 1;
            if (content != null && _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") != content.LangId)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", content.LangId);
                langID = content.LangId;
            }

            contentPages = _IContentPageService.Where(o => o.LangId == langID, true, false, o => o.ContentPageChilds, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage).Result.ToList();

            _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages.Where(o => o.LangId == langID));

            ViewBag.currContent = content;
            ViewBag.content = content;
            ViewBag.LanguageID = langID;


            if (TemplateType == TemplateType.DikeySayfa)
            {
                if (TemplateType == TemplateType.DikeySayfa)
                {
                    List<int> currContentList = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).Select(x => x.Id).ToList();
                    ViewBag.filters = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).OrderBy(o => o.CreaDate).ToList();
                    ViewBag.contents = contentPages.Where(x => currContentList.Contains(x.ContentPageId ?? 0) && x.IsDeleted == null).OrderBy(o => o.ContentOrderNo).ToList();
                }
               
            }
            return View(TemplateType.ToString());
        }
    }
}