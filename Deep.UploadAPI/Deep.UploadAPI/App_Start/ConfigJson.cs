using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Deep.UploadAPI.App_Start
{
    public class ConfigJson
    {
        [JsonProperty("items")]
        public List<ConfigItem> Items { get; set; } = new List<ConfigItem>();

        [JsonProperty("white_list")]
        public List<string> WhiteList = new List<string>();

        public class ConfigItem
        {
            private ConfigItem()
            {

            }

            public static ConfigItem Create(string type, string allowTypes, int maxlength, string basedir,
                bool yearDir = true, bool monthDir = true, bool dayDir = true)
            {
                var item = new ConfigItem();
                item.Type = type;
                item.AllowTypes = allowTypes;
                item.BaseDir = basedir;
                item.YearDir = yearDir;
                item.MonthDir = monthDir;
                item.DayDir = dayDir;
                item.MaxLength = maxlength;
                return item;
            }
            /// <summary>
            /// 接口类型
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; }
            /// <summary>
            /// 允许上传的文件类型
            /// </summary>
            [JsonProperty("allow_types")]
            public string AllowTypes { get; set; }
            /// <summary>
            /// 允许上传的文件长度
            /// </summary>
            [JsonProperty("max_length")]
            public int MaxLength { get; set; }
            /// <summary>
            /// 上传的基本目录
            /// </summary>
            [JsonProperty("base_dir")]
            public string BaseDir { get; set; }
            /// <summary>
            /// 使用年份目录
            /// </summary>
            [JsonProperty("year_dir")]
            public bool YearDir { get; set; }
            /// <summary>
            /// 使用月份目录
            /// </summary>
            [JsonProperty("month_dir")]
            public bool MonthDir { get; set; }
            /// <summary>
            /// 使用日期目录
            /// </summary>
            [JsonProperty("day_dir")]
            public bool DayDir { get; set; }
        }

        private static ConfigJson _singleton;
        private static object _lock = new object();
        private ConfigJson()
        {

        }

        public static ConfigJson Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    lock (_lock)
                    {
                        if (_singleton == null)
                        {
                            var jsonFile = ConfigurationManager.AppSettings["ConfigJson"];
                            if (string.IsNullOrWhiteSpace(jsonFile) || !System.IO.File.Exists(jsonFile))
                            {
                                jsonFile = HttpContext.Current.Server.MapPath("config.json");
                            }                             
                            if (System.IO.File.Exists(jsonFile))
                            {
                                var json = System.IO.File.ReadAllText(jsonFile);
                                _singleton = JsonConvert.DeserializeObject<ConfigJson>(json);
                            }
                            else
                            {
                                _singleton = new ConfigJson()
                                {
                                    Items = new List<ConfigItem>()
                                    {
                                        ConfigItem.Create("image","*.jpeg|*.jpg|*.png|*.gif",102400,"/uploads/images"),
                                        ConfigItem.Create("video","*.mp4|*.avi",1024*1024*10,"uploads/video")
                                    },
                                    WhiteList = new List<string>()
                                    {
                                        "127.0.0.1","localhost"
                                    }
                                };
                                System.IO.File.WriteAllText(jsonFile,JsonConvert.SerializeObject(_singleton));
                            }
                        }
                    }                     
                }
                return _singleton;
            }
        }

    }
}