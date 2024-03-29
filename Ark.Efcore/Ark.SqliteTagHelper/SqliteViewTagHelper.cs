﻿using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Ark.View
{
    [HtmlTargetElement("sqlite-view")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SqliteViewTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(context.TagName, "sqlite-view",
                              StringComparison.OrdinalIgnoreCase))
            {
                output.TagName = "table";
                output.Attributes.Add("style", "border-radius: 16px 16px 0px 0px;width: 100%;border-collapse: collapse;");
                if (output.Attributes.ContainsName("connection-string") && output.Attributes.ContainsName("data-qry"))
                {
                    StringBuilder bb = new StringBuilder();
                    var cstr = (output.Attributes["connection-string"].Value ?? "").ToString();
                    var dqry = (output.Attributes["Data-Qry"].Value ?? "").ToString();
                    if (string.IsNullOrEmpty(cstr) || string.IsNullOrEmpty(dqry)) return;
                    var dyn = new Ark.Sqlite.SqliteManager($"Data Source=./{cstr}").Select(dqry);
                    bool first_executed = true;
                    foreach (dynamic v in dyn)
                    {
                        if (first_executed)
                        {
                            bb.Append("<thead><tr style='background-color: #071665;color: #fff;'>");
                            foreach (var p in (IDictionary<string, object>)v)
                            {
                                bb.Append($"<td style='padding: 15px;white-space: nowrap;'>{p.Key}</td>");
                            }
                            bb.Append("</tr></thead>");
                            bb.Append("<tbody>");
                            first_executed = false;
                        }
                        else
                        {
                            bb.Append("<tr>");
                            foreach (var p in (IDictionary<string, object>)v)
                            {
                                bb.Append($"<td style='border: 1px solid #ddd;'>{p.Value}</td>");
                            }
                            bb.Append("</tr>");
                        }
                    }
                    bb.Append("</tbody>");
                    output.PostContent.AppendHtml(bb.ToString() + "<lord-jesus-my-master></lord-jesus-my-master>");
                }
            }

        }
    }
}