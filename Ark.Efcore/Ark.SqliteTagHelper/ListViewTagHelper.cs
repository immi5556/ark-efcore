using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel;

namespace Ark.View
{
    [HtmlTargetElement("list-view")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class ListViewTagHelper : TagHelper
    {
        //public override void Process(TagHelperContext context, TagHelperOutput output)
        //{
        //    if (string.Equals(context.TagName, "list-view",
        //                      StringComparison.OrdinalIgnoreCase))
        //    {
        //        output.TagName = "table";
        //        output.Attributes.Add("style", "border-radius: 16px 16px 0px 0px;width: 100%;border-collapse: collapse;");
        //    }
        //}
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            base.ProcessAsync(context, output);
        }
    }
}
