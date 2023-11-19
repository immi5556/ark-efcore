using Microsoft.AspNetCore.Razor.TagHelpers;
using System.ComponentModel;

namespace Ark.View
{
    [HtmlTargetElement("upload-view")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class UploadViewTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (string.Equals(context.TagName, "upload-view",
                              StringComparison.OrdinalIgnoreCase))
            {
                output.TagName = "div";
                output.PostContent.AppendHtml(string.Format(template_html, template_css, template_Script));
            }
        }

        string template_html = $@"<div class=""ark-upl-container"">
  <form class=""ark-upl-form"">
    <div class=""ark-upl-file-upload-wrapper"" data-text=""Select your file!"">
      <input onchange=""ark_upl_change(this)"" name=""file-upload-field"" type=""file"" class=""file-upload-field"" value="""">
    </div>
    <div class=""ark-container-rest""></div>
  </form>
<style>{{0}}
</style>{{1}}
</div>";
        string template_Script = $@"<script>
            var ark_upl_change = (ele) => {{
                event.preventDefault();
                ele.closest('.ark-upl-file-upload-wrapper').setAttribute('data-text', document.querySelector('.file-upload-field').value.replace(/.*(\/|\\)/, ''));
                const formData = new FormData();
                formData.append('file',ele.files[0]);    
                fetch('/ark/upload',{{
                    method:'POST',
                    body:formData
                }}).then(rest => rest.json())
                .then(result => {{
                    console.log(result);
                    var ddm = ele.closest('.ark-upl-form').querySelector('.ark-container-rest');
                    //(result.msg || []).forEach(tt => ddm.innerHTML = ddm.innerHTML + '<br /><a href=' + tt.file_url + ' target=""_blank"">view</a>');
                }})
                .catch(e => console.log(e));
            }}
</script>";

        string template_css = $@".ark-upl-container {{
  -webkit-box-align: center;
  -moz-box-align: center;
  box-align: center;
  -webkit-align-items: center;
  -moz-align-items: center;
  -ms-align-items: center;
  -o-align-items: center;
  align-items: center;
  -ms-flex-align: center;
  -webkit-box-pack: center;
  -moz-box-pack: center;
  box-pack: center;
  -webkit-justify-content: center;
  -moz-justify-content: center;
  -ms-justify-content: center;
  -o-justify-content: center;
  justify-content: center;
  -ms-flex-pack: center;
  background-color: #bf7a6b;
  background-image: -webkit-linear-gradient(bottom left, #bf7a6b 0%, #e6d8a7 100%);
  background-image: linear-gradient(to top right,#bf7a6b 0%, #e6d8a7 100%);
}}

.form {{
  width: 400px;
}}

.ark-upl-file-upload-wrapper {{
  position: relative;
  width: 100%;
  height: 60px;
}}
.ark-upl-file-upload-wrapper:after {{
  content: attr(data-text);
  font-size: 18px;
  position: absolute;
  top: 0;
  left: 0;
  background: #fff;
  padding: 10px 15px;
  display: block;
  width: calc(100% - 40px);
  pointer-events: none;
  z-index: 20;
  height: 40px;
  line-height: 40px;
  color: #999;
  border-radius: 5px 10px 10px 5px;
  font-weight: 300;
}}
.ark-upl-file-upload-wrapper:before {{
  content: ""Upload"";
  position: absolute;
  top: 0;
  right: 0;
  display: inline-block;
  height: 60px;
  background: #4daf7c;
  color: #fff;
  font-weight: 700;
  z-index: 25;
  font-size: 16px;
  line-height: 60px;
  padding: 0 15px;
  text-transform: uppercase;
  pointer-events: none;
  border-radius: 0 5px 5px 0;
}}
.ark-upl-file-upload-wrapper:hover:before {{
  background: #3d8c63;
}}
.ark-upl-file-upload-wrapper input {{
  opacity: 0;
  position: absolute;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 99;
  height: 40px;
  margin: 0;
  padding: 0;
  display: block;
  cursor: pointer;
  width: 100%;
}}
";
    }
}
