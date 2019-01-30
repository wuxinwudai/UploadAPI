using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Deep.UploadAPI.App_Start
{
    /// <summary>
    /// CheckUploadedFileResult 枚举上传文件检验的结果
    /// </summary>    
    public enum CheckUploadedFileResult
    {
        /// <summary>
        /// 安全，允许上传
        /// </summary>
        
        AllowUpload,
        /// <summary>
        /// 文件为 null 或长度为0
        /// </summary>
        
        NullOrEmpty,
        /// <summary>
        /// 文件大小超出限制
        /// </summary>
       FileToLarge,
        /// <summary>
        /// 不是允许上传的文件类型
        /// </summary>
         NotAllowedFileType,
    }
}