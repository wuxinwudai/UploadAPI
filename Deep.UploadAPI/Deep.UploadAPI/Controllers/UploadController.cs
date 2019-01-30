using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Deep.UploadAPI.App_Start;

namespace Deep.UploadAPI.Controllers
{
    public class UploadController : Controller
    {        
        [HttpPost]
        [Route("{type}")]
        public ActionResult Index(string type)
        {
            var config = ConfigJson.Singleton;
            var item = config.Items.SingleOrDefault(p => p.Type == type);
            if (item == null)
            {
                return Json(new {ok = false, message = "尚未完成文件上传接口配置"});
            }

            var file = Request.Files[0];
            if (file == null || file.ContentLength == 0)
            {
                return Json(new {ok = false, message = "不允许上传空文件"});
            }

            if (file.ContentLength > item.MaxLength)
            {
                return Json(new {ok = false, message = $"上传的文件大小超出限制，最大为{item.MaxLength / 1024}K"});
            }

            var ext = Path.GetExtension(file.FileName)?.ToLower();
            if (!item.AllowTypes.ToLower().Contains(ext))
            {
                return Json(new {ok = false, message = $"该文件类型不受支持！"});
            }
            var folder = Request["folder"];
            var fileName = DateTime.Now.ToString("yyMMddHHmmssfff") + ext;
            folder = folder == null ? "": folder.Trim('/');
            var relativePath = folder.Length == 0 ? item.BaseDir +"/" : item.BaseDir +"/"+ folder + "/";
            if (item.YearDir)
            {
                relativePath += $"{DateTime.Now.Year}/";
            }
            if (item.MonthDir)
            {
                relativePath += $"{DateTime.Now.Month}/";
            }
            if (item.DayDir)
            {
                relativePath += $"{DateTime.Now.Day}/";
            }
            var filePath = Server.MapPath("~" + relativePath);
            try
            {
                if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);
                file.SaveAs(filePath + fileName);
            }
            catch
            {
                try
                {
                    System.IO.File.Delete(filePath + fileName);
                }
                catch
                {

                }

            }
            return Json(new {ok = true, filename = relativePath + fileName});
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (String.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            if (!ConfigJson.Singleton.WhiteList.Contains(Request.Url.Host) && !ConfigJson.Singleton.WhiteList.Contains(ip))
            {
                filterContext.Result = new JsonResult(){Data = new { ok = false, message = "客户端IP或域名不在白名单内" }};
            }
           
            base.OnActionExecuting(filterContext);
        }
    }


}