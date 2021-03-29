﻿using System;
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

        IFormTypeService _IFormTypeService;
        IHttpContextAccessor _httpContextAccessor;
        IContentPageService _IContentPageService;
        ISpecService _ISpecService;
        IDocumentsService _IDocumentsService;
        IProjectProductService _IProjectProductService;
        public ContentView(
            IContentPageService _IContentPageService,
            ISpecService _ISpecService,
        IDocumentsService _IDocumentsService,
        IHttpContextAccessor httpContextAccessor,
        IFormTypeService _IFormTypeService,
        IProjectProductService _IProjectProductService
            )
        {
            this._IContentPageService = _IContentPageService;
            this._ISpecService = _ISpecService;
            this._httpContextAccessor = httpContextAccessor;
            this._IDocumentsService = _IDocumentsService;
            this._IFormTypeService = _IFormTypeService;
            this._IProjectProductService = _IProjectProductService;
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
            var content = _IContentPageService.Where(o => o.Link.ToLower() == link.ToLower() && o.IsActive == true, true, false,
                o => o.ContentPageChilds, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.TechnicalProperties, o => o.TechnicalDocuments,
                o => o.CadDatas, o => o.BIMFiles, o => o.ThumbImage, o => o.Picture, o => o.BannerImage, o => o.SpecContentValue, o => o.FormType
                )
                .Result.FirstOrDefault();




            //if (content.FormTypeId)
            //{
            //    int fid = content.FormTypeId.Value;
            //    FormType formtip  _IFormTypeService.Where(o => o.Id == fid).Result.FirstOrDefault();
            //    ViewBag.formtip = formtip;

            //}
            if (content.ContentTypesId == 5)
            {
                ViewBag.ProductList = _IProjectProductService.Where(o => o.ProjectId == content.Id, true, false, o => o.Product).Result.ToList();
            }
            if (content.ContentTypesId == 3)
            {
                ViewBag.ProjectList = _IProjectProductService.Where(o => o.ProductId == content.Id, true, false, o => o.Project, o => o.Project.ThumbImage).Result.ToList();
            }

            var Specs = _ISpecService.Where(null, true, false, o => o.Parent,o=>o.SpecAttrs).Result.ToList();
            ViewBag.Specs = Specs;
            int langID = content != null ? content.LangId : 1;
            if (content != null && _httpContextAccessor.HttpContext.Session.GetInt32("LanguageID") != content.LangId)
            {
                _httpContextAccessor.HttpContext.Session.SetInt32("LanguageID", content.LangId);
                langID = content.LangId;
            }

            ViewBag.content = content;

            Regex reg = new Regex("[*'\",_&#^@]");
            var currState = reg.Replace(_httpContextAccessor.HttpContext.Session.GetString("currState") ?? "-", string.Empty);

            ViewBag.currState = currState;
            bool isBayi = false;
            bool isEndustri = false;
            bool isMimar = false;
            bool isBireysel = false;

            contentPages = _IContentPageService.Where(x => x.LangId == langID && x.IsDeleted == null && x.IsActive == true && x.IsInteral == true, true, false,
                o => o.ContentPageChilds, o => o.SpecContentValue, o => o.Parent, o => o.Gallery, o => o.Documents, o => o.ThumbImage, o => o.Picture, o => o.BannerImage).Result.ToList();

            _httpContextAccessor.HttpContext.Session.Set("contentPages", contentPages.Where(o => o.LangId == langID));
            ViewBag.contentPages = contentPages;


            ViewBag.LanguageID = langID;
            ViewBag.Pages = contentPages.ToList();
            switch (currState)
            {
                case "Uygulamacılar":
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




            if (TemplateType == TemplateType.ProjeListeleme)
            {
                List<ContentPage> currList = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).ToList();
                List<int> currSpecList = currList.SelectMany(x => x.SpecContentValue).Select(s => s.SpecId).Distinct().ToList();
                //ViewBag.contentPages = contentPages.Where(x => currContentList.Contains(x.ContentPageId ?? 0) && x.IsDeleted == null).OrderBy(o => o.ContentOrderNo).ToList();
              
                ViewBag.subPages = currList;
                ViewBag.currSpecList = currSpecList;
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
            else if (TemplateType == TemplateType.KategoriListeleme)
            {

                List<ContentPage> currList = contentPages.Where(x => x.ContentPageId == content.Id && x.IsDeleted == null).ToList();
                List<int> currSpecList = currList.SelectMany(x => x.SpecContentValue).Select(s => s.SpecId).Distinct().ToList();
                ViewBag.categories = contentPages.Where(x => x.ContentPageId == content.Parent?.Id && x.IsDeleted == null).ToList();
                //ViewBag.contentPages = contentPages.Where(x => currContentList.Contains(x.ContentPageId ?? 0) && x.IsDeleted == null).OrderBy(o => o.ContentOrderNo).ToList();
                ViewBag.subPages = currList;
                ViewBag.currSpecList = currSpecList;

                //select Distinct SpecId from SpecContentValue
            }
            return View(TemplateType.ToString());
        }
    }
}