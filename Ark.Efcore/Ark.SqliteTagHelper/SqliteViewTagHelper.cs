using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Ark.Immanuel
{
    [HtmlTargetElement("sqlite-view")]
    public class SqliteViewTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "a";    // Replaces <email> with <a> tag
        }
    }
}