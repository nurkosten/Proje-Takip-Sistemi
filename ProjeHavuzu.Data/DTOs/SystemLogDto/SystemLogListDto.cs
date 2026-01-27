using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.DTOs.SystemLogDto
{
    public class SystemLogListDto
    {
        public Guid Id { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string HttpMethod { get; set; }
        public string Url { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Detail { get; set; }
        public string LogType { get; set; }
        public string Exception { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedByUserName { get; set; } // Join ile alabiliriz veya CreatedBy
    }
}
