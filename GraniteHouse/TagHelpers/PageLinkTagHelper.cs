using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraniteHouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace GraniteHouse.TagHelpers
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory _urlHelperFactory;

        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; }
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i <= PageModel.totalPage; i++)
            {
                TagBuilder ancorTag = new TagBuilder("a");
                string url = PageModel.UrlParam.Replace(":", i.ToString());
                ancorTag.Attributes["href"] = url;

                if (PageClassesEnabled)
                {
                    ancorTag.AddCssClass(PageClass);
                    ancorTag.AddCssClass(i==PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }

                ancorTag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(ancorTag);
            }

            output.Content.AppendHtml(result.InnerHtml);
        }
    }
}
