using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace Ark.View
{
    [HtmlTargetElement("dir-list")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class DirListTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(context.TagName, "dir-list",
                              StringComparison.OrdinalIgnoreCase))
            {
                output.TagName = "table";
                output.Attributes.Add("style", "border-radius: 16px 16px 0px 0px;width: 100%;border-collapse: collapse;");
                if (!output.Attributes.ContainsName("dir-path")) return;
                var dpath = (output.Attributes["dir-path"].Value ?? "").ToString();
                bool nested = bool.Parse((output.Attributes["nested"]?.Value ?? "false").ToString());
                bool deletable = bool.Parse((output.Attributes["deletable"]?.Value ?? "false").ToString());
                if (string.IsNullOrEmpty(dpath)) return;
                dpath = Path.Combine(Environment.CurrentDirectory, dpath);
                if (!Directory.Exists(dpath))
                {
                    output.PostContent.AppendHtml("<tr><td>Invalid Directory</td></tr>");
                    return;
                }
                var files = new List<string>();
                if (nested) files = Directory.EnumerateFiles(dpath, "*", SearchOption.AllDirectories).ToList();
                else files = Directory.EnumerateFiles(dpath, "*", SearchOption.TopDirectoryOnly).ToList();
                StringBuilder bb = new StringBuilder();
                bb.Append("<thead><tr style='background-color: #071665;color: #fff;'>");
                bb.Append($"<td style='padding: 15px;white-space: nowrap;'>File Name</td>");
                bb.Append($"<td style='padding: 15px;white-space: nowrap;'>Dir</td>");
                bb.Append($"<td style='padding: 15px;white-space: nowrap;'>Size</td>");
                if (deletable) bb.Append($"<td style='padding: 15px;white-space: nowrap;'>Action</td>");
                bb.Append("</tr></thead>");
                bb.Append("<tbody>");
                var uqq = TagExtn.RandomStr();
                int idx = 0;
                foreach (var file in files)
                {
                    var ff = new FileInfo(file);
                    bb.Append($"<tr>");
                    bb.Append($"<td style='border: 1px solid #ddd;'>{ff.Name}</td>");
                    bb.Append($"<td style='border: 1px solid #ddd;'>{ff.DirectoryName}</td>");
                    bb.Append($"<td style='border: 1px solid #ddd;'>{GetSize(ff.Length)}</td>");
                    if (deletable) bb.Append(@$"<td style='border: 1px solid #ddd;'><a onclick='del_{uqq}(this, ""{HttpUtility.UrlEncode(ff.FullName)}"")' href='javascript:void(0);'>delete</a></td>");
                    bb.Append($"</tr>");
                }
                bb.Append("</tbody>");
                output.PostContent.AppendHtml(bb.ToString() + "<lord-jesus-my-master></lord-jesus-my-master>");
                output.PostContent.AppendHtml(@$"<script>
function del_{uqq}(ele, file_uri){{
console.log(ele);
fetch('/ark/delete/' + file_uri).then(resp => resp.json()).then(data => {{ 
                        console.log(data);
                        if (!data.errored) ele.closest('tr').remove();
                        else alert('delete failed.')
                    }})
        }}
</script>");
            }

        }
        string GetSize(long input)
        {
            string output;
            switch (input.ToString().Length)
            {
                case > 12:
                    output = input / 1000000000000 + " Tb";
                    break;
                case > 9:
                    output = input / 1000000000 + " Gb";
                    break;
                case > 6:
                    output = input / 1000000 + " Mb";
                    break;
                case > 3:
                    output = input / 1000 + " Kb";
                    break;
                default:
                    output = input + " b";
                    break;
            }
            return output;
        }
    }
}