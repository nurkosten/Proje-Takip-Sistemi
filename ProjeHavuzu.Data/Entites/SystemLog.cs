using ProjeHavuzu.Data.Entites.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProjeHavuzu.Data.Entites
{
    public class SystemLog : BaseEntity
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string HttpMethod { get; set; } // GET, POST, PUT, DELETE
        public string Url { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; } // Tarayıcı Bilgisi
        public string Detail { get; set; } // Detaylı Açıklama
        public string LogType { get; set; } // Info, Error, Warning
        public string? Exception { get; set; } // Hata Detayı system log
    }
}
