using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.ComponentModel;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Ark.View
{
    [HtmlTargetElement("file-view")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class FileViewTagHelper : TagHelper
    {
        private async IAsyncEnumerable<string> ReadStream(string file_path)
        {
            using (var reader = new StreamReader(file_path))
            {
                while (!reader.EndOfStream)
                {
                    yield return await reader.ReadLineAsync();
                }
            }
        }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(context.TagName, "file-view",
                              StringComparison.OrdinalIgnoreCase))
            {
                output.TagName = "div";
                output.Attributes.Add("style", "height: 600px;overflow: auto;");
                var uq_id = TagExtn.RandomStr();
                output.Attributes.Add("id", "tbl_" + uq_id);
                output.PostContent.AppendHtml($"<Table id='tbl_{uq_id}'> style='border-radius: 16px 16px 0px 0px;width: 100%;border-collapse: collapse;'");
                if (!output.Attributes.ContainsName("file-path"))
                {
                    output.PostContent.AppendHtml("<tr><td>file-path attribute missing</td></tr>");
                    return;
                }
                var file_path = (output.Attributes["file-path"].Value ?? "").ToString();
                if (string.IsNullOrEmpty(file_path))
                {
                    output.PostContent.AppendHtml("<tr><td>file-path empty</td></tr>");
                    return;
                }
                var full_file_path = Path.Combine(Environment.CurrentDirectory, file_path);
                if (!System.IO.File.Exists(full_file_path))
                {
                    output.PostContent.AppendHtml("<tr><td>invalid file-path uri</td></tr>");
                    return;
                }

                var delimiter = output.Attributes.ContainsName("delimiter") ? (output.Attributes["delimiter"].Value ?? "").ToString() : "";
                delimiter = string.IsNullOrEmpty(delimiter) ? "," : delimiter.Trim();

                bool streaming = bool.Parse((output.Attributes["streaming"]?.Value ?? "false").ToString());

                bool heading = bool.Parse((output.Attributes["heading"]?.Value ?? "false").ToString());
                List<List<string>> lst = new List<List<string>>();
                StringBuilder bb = new StringBuilder();
                int cnt = 0;
                await foreach (var item in ReadStream(full_file_path))
                {
                    if (heading)
                    {
                        bb.Append("<thead><tr style='background-color: #071665;color: #fff;'>");
                        foreach (var p in (item ?? "").Split(delimiter))
                        {
                            bb.Append($"<td style='padding: 15px;white-space: nowrap;'>{p}</td>");
                        }
                        bb.Append("</tr></thead>");
                        bb.Append($"<tbody id='tbody_{uq_id}'>");
                        heading = false;
                    }
                    else
                    {
                        if (streaming) break;
                        bb.Append("<tr>");
                        var lg = (item ?? "").Split(delimiter);
                        lst.Add(lg.ToList());
                        foreach (var p in lg)
                        {
                            bb.Append($"<td style='border: 1px solid #ddd;'>{p}</td>");
                        }
                        bb.Append("</tr>");
                    }
                }
                bb.Append("</tbody>");
                output.PostContent.AppendHtml(bb.ToString() + "</Table>");
                if (streaming)
                    output.PostContent.AppendHtml($@"<script>
                fetch('/ark/file/stream/{HttpUtility.UrlEncode(file_path)}').then(rest => rest.json())
                .then(result => {{
                    console.log(result.length);
                    var tbb = document.getElementById('tbody_{uq_id}');
                    let intr = 100;
                    (result || []).forEach((tt, idx) => {{
                        setTimeout(() => {{
                            var trr = '<tr>';
                            console.log('log : ', idx, tt); 
                            ((tt || '').split(',') || []).forEach(tg => {{
                                trr = trr + `<td>${{tg}}</td>`
                            }});
                            trr = trr + '</tr>';
                            tbb.insertAdjacentHTML('beforeend', trr);
                        }}, intr);
                        intr = intr + 100;
                    }});
                }})
                .catch(err => console.warn('file stream: ' + err));
</script>");
            }
        }
    }
}
