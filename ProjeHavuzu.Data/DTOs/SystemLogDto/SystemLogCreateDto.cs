using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.SystemLogDto
{
    public class SystemLogCreateDto
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Detail { get; set; }
        public string LogType { get; set; } = "Info";
        public string Exception { get; set; }
        public string IpAddress { get; set; }
        public string Url { get; set; }
        public string HttpMethod { get; set; }
        public string UserAgent { get; set; }
        public Guid? CreatedBy { get; set; } // Kullanıcı ID'si de kaybolabilir
    }
}
